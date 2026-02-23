namespace SweetShop.Services.Caching;

/// <summary>
/// Abstraction over the underlying cache store (IMemoryCache or Redis).
/// Swap implementations without touching any consumer code.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Tries to get a cached value. Returns null if not found (cache miss).
    /// </summary>
    Task<T?> GetAsync<T>(string key) where T : class;

    /// <summary>
    /// Writes a value to the cache with an optional TTL.
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;

    /// <summary>
    /// Removes a specific key from the cache (point invalidation).
    /// </summary>
    Task RemoveAsync(string key);

    /// <summary>
    /// Removes all keys that start with the given prefix (tag-based invalidation).
    /// Used to bust entire entity groups (e.g., all "products:*" keys at once).
    /// </summary>
    Task RemoveByPrefixAsync(string prefix);
}
