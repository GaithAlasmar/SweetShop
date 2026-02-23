using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace SweetShop.Services.Caching;

/// <summary>
/// Redis-backed distributed cache implementation using IDistributedCache.
/// Switch to this in multi-server / Azure deployments by changing the
/// DI registration in Program.cs — no consumer code changes needed.
///
/// Setup: dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
///        builder.Services.AddStackExchangeRedisCache(o => o.Configuration = "localhost:6379");
/// </summary>
public class RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
    : ICacheService
{
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(10);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // ── Read ──────────────────────────────────────────────────────────
    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        var bytes = await cache.GetAsync(key);
        if (bytes is null)
        {
            logger.LogDebug("[Redis MISS] key={Key}", key);
            return null;
        }

        logger.LogDebug("[Redis HIT]  key={Key}", key);
        return JsonSerializer.Deserialize<T>(bytes, JsonOptions);
    }

    // ── Write ─────────────────────────────────────────────────────────
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value, JsonOptions);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? DefaultExpiry
        };

        await cache.SetAsync(key, bytes, options);
        logger.LogDebug("[Redis SET]  key={Key} ttl={Ttl}", key, expiry ?? DefaultExpiry);
    }

    // ── Point Invalidation ────────────────────────────────────────────
    public async Task RemoveAsync(string key)
    {
        await cache.RemoveAsync(key);
        logger.LogInformation("[Redis DEL]  key={Key}", key);
    }

    // ── Prefix / Tag Invalidation ─────────────────────────────────────
    /// <summary>
    /// Redis doesn't natively support prefix-based deletion via IDistributedCache.
    /// For full SCAN-based invalidation, inject IConnectionMultiplexer from
    /// StackExchange.Redis directly. This implementation logs a guidance message.
    ///
    /// Production pattern:
    ///   var server = _multiplexer.GetServer(_multiplexer.GetEndPoints().First());
    ///   var keys = server.Keys(pattern: $"{prefix}:*").ToArray();
    ///   await _db.KeyDeleteAsync(keys);
    /// </summary>
    public async Task RemoveByPrefixAsync(string prefix)
    {
        // Workaround for IDistributedCache limitation:
        // Maintain a Redis Set at key "index:{prefix}" containing all full keys.
        // On write (SetAsync), add the full key to this Set.
        // On prefix invalidation, read the Set, delete all its members, then delete the Set.
        logger.LogWarning(
            "[Redis PREFIX-DEL] prefix={Prefix} — use IConnectionMultiplexer for true SCAN-based deletion.",
            prefix);

        // Fallback: remove the index key itself (prevents stale index growth)
        await cache.RemoveAsync($"index:{prefix}");
    }
}
