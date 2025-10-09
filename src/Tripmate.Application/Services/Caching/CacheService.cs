
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Tripmate.Application.Services.Caching
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _database;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CacheService> _logger;
        public readonly bool _redisIsAvailable;
        public CacheService(IConnectionMultiplexer redis, ILogger<CacheService> logger, IMemoryCache memoryCache)
        {

            _memoryCache = memoryCache;
            _logger = logger;
            _redisIsAvailable = redis?.IsConnected ?? false;

            try
            {
                if (_redisIsAvailable)
                {
                    _database = redis.GetDatabase();
                    _logger.LogInformation("Redis cache is available and will be used as primary cache");
                }
                else
                {
                    _logger.LogWarning("Redis is not connected. Falling back to in-memory cache.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to Redis. Falling back to in-memory cache.");
                _redisIsAvailable = false;
            }
        }




        public async Task<T?> GetAsync<T>(string key)
        {
            if (!_redisIsAvailable)
            {
              var isInMemoryCache = _memoryCache.TryGetValue(key, out T value);
                if (isInMemoryCache)
                {
                    _logger.LogInformation("Data retrieved from in-memory cache.");
                    return value;
                }
                _logger.LogWarning("Data not found in in-memory cache, will be fetched from the primary data source.");

                return default;

            }

            try
            {
                var jsonData = await _database.StringGetAsync(key);
                if (jsonData.IsNullOrEmpty)
                {
                    return default;
                }
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                return JsonSerializer.Deserialize<T>(jsonData, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving data from Redis cache.");
                _logger.LogWarning("Data will be fetched from the primary data source.");
                return default;

            }
        }

        

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration)
        {
            if (value == null)
            {
                return;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            if (!_redisIsAvailable)
            {
                _memoryCache.Set(key, value, expiration ?? TimeSpan.FromHours(1));
                _logger.LogDebug("Data cached in in-memory cache for key: {Key}", key);
                return;
            }

            try
            {
                var jsonData = JsonSerializer.Serialize(value, options);
                await _database.StringSetAsync(key, jsonData, expiration ?? TimeSpan.FromHours(1));
                _logger.LogDebug("Data cached in Redis for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting data in Redis cache for key: {Key}", key);
            }
            return;

        }
        public async Task RemoveAsync(string key)
        {
           
            if (!_redisIsAvailable)
            {
                _memoryCache.Remove(key);
                _logger.LogDebug("Data removed from in-memory cache for key: {Key}", key);
                return ;
            }

            try
            {
                _database.KeyDelete(key);
                _logger.LogDebug("Data removed from Redis cache for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing data from Redis cache for key: {Key}", key);
            }
            return ;

        }
    }
}
