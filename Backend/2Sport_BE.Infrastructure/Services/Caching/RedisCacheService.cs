using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace _2Sport_BE.Services.Caching
{
    public interface IRedisCacheService
    {
        T? GetData<T>(string key);
        void SetData<T>(string key, T data);
    }

    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache? _cache;

        public RedisCacheService(IDistributedCache? cache)
        {
            _cache = cache;
        }


        public T? GetData<T>(string key)
        {
            var data = _cache?.GetString(key);

            if (data is null)
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(data);
        }

        public void SetData<T>(string key, T data)
        {
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
            };
            _cache?.SetString(key, JsonSerializer.Serialize(data), options);
        }
    }
}
