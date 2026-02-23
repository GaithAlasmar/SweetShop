using Microsoft.Extensions.Caching.Memory;

namespace SweetShop.Services.Caching;

/// <summary>
/// In-process cache implementation using IMemoryCache.
/// Best for single-server deployments. No serialization overhead.
/// Switch to RedisCacheService for multi-server / scaled-out environments.
/// </summary>
public class MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
    : ICacheService
{
    // Track all active cache keys by prefix so we can do group invalidation.
    // Dictionary value is a HashSet of full keys that belong to that prefix.
    private readonly Dictionary<string, HashSet<string>> _prefixIndex = new();
    private readonly Lock _lock = new(); // .NET 9 Lock (replaces object + lock())

    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(10);

    // ── Read ──────────────────────────────────────────────────────────
    public Task<T?> GetAsync<T>(string key) where T : class
    {
        cache.TryGetValue(key, out T? value);

        if (value is not null)
            logger.LogDebug("[Cache HIT]  key={Key}", key);
        else
            logger.LogDebug("[Cache MISS] key={Key}", key);

        return Task.FromResult(value);
    }

    // ── Write ─────────────────────────────────────────────────────────
    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? DefaultExpiry,
            // Evict this key when memory pressure is high
            Priority = CacheItemPriority.Normal,
            // Post-eviction callback for observability
            PostEvictionCallbacks =
            {
                new PostEvictionCallbackRegistration
                {
                    EvictionCallback = (k, _, reason, _) =>
                        logger.LogDebug("[Cache EVICT] key={Key} reason={Reason}", k, reason)
                }
            }
        };

        cache.Set(key, value, options);

        // Register the key in the prefix index for group invalidation
        var prefix = ExtractPrefix(key);
        lock (_lock)
        {
            if (!_prefixIndex.TryGetValue(prefix, out var keys))
            {
                keys = [];
                _prefixIndex[prefix] = keys;
            }
            keys.Add(key);
        }

        logger.LogDebug("[Cache SET]  key={Key} ttl={Ttl}", key, expiry ?? DefaultExpiry);
        return Task.CompletedTask;
    }

    // ── Point Invalidation ────────────────────────────────────────────
    public Task RemoveAsync(string key)
    {
        cache.Remove(key);
        logger.LogInformation("[Cache DEL]  key={Key}", key);
        return Task.CompletedTask;
    }

    // ── Prefix / Tag Invalidation ─────────────────────────────────────
    public Task RemoveByPrefixAsync(string prefix)
    {
        HashSet<string> keys;
        lock (_lock)
        {
            if (!_prefixIndex.TryGetValue(prefix, out var found))
            {
                logger.LogDebug("[Cache PREFIX-DEL] no keys for prefix={Prefix}", prefix);
                return Task.CompletedTask;
            }
            keys = [.. found]; // snapshot
            _prefixIndex.Remove(prefix);
        }

        foreach (var key in keys)
            cache.Remove(key);

        logger.LogInformation("[Cache PREFIX-DEL] prefix={Prefix} cleared {Count} key(s)",
            prefix, keys.Count);

        return Task.CompletedTask;
    }

    // ── Helpers ───────────────────────────────────────────────────────
    private static string ExtractPrefix(string key)
    {
        // Convention: keys are formatted as "entity:qualifier"
        // e.g. "products:all", "products:id:5", "products:search:cake"
        // The prefix is the first segment: "products"
        var colonIndex = key.IndexOf(':');
        return colonIndex > 0 ? key[..colonIndex] : key;
    }
}
