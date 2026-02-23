using SweetShop.Models;
using SweetShop.Models.Interfaces;
using SweetShop.Services.Caching;

namespace SweetShop.Services;

/// <summary>
/// Decorator that wraps IProductRepository with the Cache-Aside pattern.
///
/// Cache-Aside flow:
///   READ  → Check cache → HIT: return cached value
///                       → MISS: call inner repo → store in cache → return value
///   WRITE → Call inner repo (source of truth) → invalidate related cache keys
///
/// This class is registered in DI instead of the raw ProductRepository,
/// so ALL consumers (MediatR handlers, controllers) transparently get caching.
/// </summary>
public class CachedProductRepository(
    IProductRepository inner,        // The real DB-backed repository
    ICacheService cache,
    ILogger<CachedProductRepository> logger)
    : IProductRepository
{
    // ── Cache Key Constants ────────────────────────────────────────────
    // Centralise key names so invalidation is never a guessing game.
    private const string Prefix = "products";
    private const string AllKey = "products:all";
    private const string PreferredKey = "products:preferred";
    private static string IdKey(int id) => $"products:id:{id}";
    private static string SearchKey(string t) => $"products:search:{t.ToLowerInvariant()}";

    // ── TTL Strategy ───────────────────────────────────────────────────
    // The product catalogue changes infrequently → aggressive caching is safe.
    // Individual product pages change less → cache longer.
    // Search results change with stock levels → shorter TTL.
    private static readonly TimeSpan CatalogTtl = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan DetailTtl = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan SearchTtl = TimeSpan.FromMinutes(5);

    // ════════════════════════════════════════════════════════════════
    //  READS — Cache-Aside
    // ════════════════════════════════════════════════════════════════

    /// <summary>
    /// High-traffic endpoint. Served from cache on all subsequent hits.
    /// Cache is busted whenever any product is created, updated, or deleted.
    /// </summary>
    public IEnumerable<Product> GetAllProducts()
    {
        // 1️⃣ Check cache
        var cached = cache.GetAsync<List<Product>>(AllKey).GetAwaiter().GetResult();
        if (cached is not null)
        {
            logger.LogDebug("[CachedRepo] GetAllProducts — HIT");
            return cached;
        }

        // 2️⃣ Cache miss — fetch from database
        logger.LogDebug("[CachedRepo] GetAllProducts — MISS, querying DB");
        var products = inner.GetAllProducts().ToList();

        // 3️⃣ Populate cache for next request
        cache.SetAsync(AllKey, products, CatalogTtl).GetAwaiter().GetResult();

        return products;
    }

    /// <summary>
    /// Homepage "featured" section — cached independently from the full catalogue.
    /// </summary>
    public IEnumerable<Product> GetPreferredProducts()
    {
        var cached = cache.GetAsync<List<Product>>(PreferredKey).GetAwaiter().GetResult();
        if (cached is not null)
            return cached;

        var products = inner.GetPreferredProducts().ToList();
        cache.SetAsync(PreferredKey, products, CatalogTtl).GetAwaiter().GetResult();
        return products;
    }

    /// <summary>
    /// Product detail page. Per-product key so invalidating one product
    /// doesn't flush the entire catalogue cache.
    /// </summary>
    public Product? GetProductById(int productId)
    {
        var key = IdKey(productId);
        var cached = cache.GetAsync<Product>(key).GetAwaiter().GetResult();
        if (cached is not null)
            return cached;

        var product = inner.GetProductById(productId);
        if (product is not null)
            cache.SetAsync(key, product, DetailTtl).GetAwaiter().GetResult();

        return product;
    }

    /// <summary>
    /// Search results are cached per unique search term (lower-cased).
    /// Shorter TTL because stock levels change more frequently.
    /// </summary>
    public IEnumerable<Product> SearchProducts(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Enumerable.Empty<Product>();

        var key = SearchKey(searchTerm);
        var cached = cache.GetAsync<List<Product>>(key).GetAwaiter().GetResult();
        if (cached is not null)
            return cached;

        var results = inner.SearchProducts(searchTerm).ToList();
        cache.SetAsync(key, results, SearchTtl).GetAwaiter().GetResult();
        return results;
    }

    // ════════════════════════════════════════════════════════════════
    //  WRITES — Delegate to inner repo, then INVALIDATE cache
    // ════════════════════════════════════════════════════════════════

    /// <summary>
    /// After a new product is created, every "all products" and "preferred"
    /// key is stale → bust the entire "products" prefix.
    /// </summary>
    public void CreateProduct(Product product)
    {
        inner.CreateProduct(product);
        InvalidateProductCaches("CreateProduct");
    }

    /// <summary>
    /// After an update, bust both the specific product key AND catalogue keys.
    /// </summary>
    public void UpdateProduct(Product product)
    {
        inner.UpdateProduct(product);

        // Point-invalidate the individual product detail key
        cache.RemoveAsync(IdKey(product.Id)).GetAwaiter().GetResult();

        // Bust the catalogue (the product may have changed name/stock/category)
        InvalidateProductCaches("UpdateProduct");
    }

    /// <summary>
    /// After deletion, remove the specific key and flush the catalogue.
    /// </summary>
    public void DeleteProduct(Product product)
    {
        inner.DeleteProduct(product);
        cache.RemoveAsync(IdKey(product.Id)).GetAwaiter().GetResult();
        InvalidateProductCaches("DeleteProduct");
    }

    // ── Private Helpers ────────────────────────────────────────────────

    /// <summary>
    /// Busts the entire "products" cache prefix (all catalogue + search keys).
    /// Called on any write operation to ensure consistency.
    /// </summary>
    private void InvalidateProductCaches(string caller)
    {
        logger.LogInformation("[CachedRepo] {Caller} — invalidating all 'products:*' cache keys", caller);
        cache.RemoveByPrefixAsync(Prefix).GetAwaiter().GetResult();
    }
}
