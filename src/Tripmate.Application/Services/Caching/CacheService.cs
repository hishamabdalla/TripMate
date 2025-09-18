
using StackExchange.Redis;
using System.Text.Json;

namespace Tripmate.Application.Services.Caching
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _database;
        public CacheService(IConnectionMultiplexer redis)
        {
           _database= redis.GetDatabase();
        }
        public async Task<T?> GetAsync<T>(string key)
        {
            var cacheResponse = await _database.StringGetAsync(key);

            if (cacheResponse.IsNullOrEmpty) return default;
            return JsonSerializer.Deserialize<T>(cacheResponse);

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

            var jsonData = JsonSerializer.Serialize(value,options);
            await _database.StringSetAsync(key, jsonData, expiration);
        }
        public Task RemoveAsync(string key)
        {
            
            return _database.KeyDeleteAsync(key);

        }
    }
}
