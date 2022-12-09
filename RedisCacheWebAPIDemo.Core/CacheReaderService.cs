using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedisCacheWebAPIDemo.Core.contracts;
using StackExchange.Redis;
using System.Text.Json.Serialization;

namespace RedisCacheWebAPIDemo.Core;

public class CacheReaderService : ICacheReaderService
{
    IDatabase _cacheDb;
    public CacheReaderService()
    {
        var redis = ConnectionMultiplexer.Connect("localhost:6379");
        _cacheDb=redis.GetDatabase();
    }

    public T ReadCache<T>(string key)
    {
        var value = _cacheDb.StringGet(key);

        if (!value.HasValue)
            return default;
        return JsonConvert.DeserializeObject<T>(value);

    }

    public object RemoveCacheItem(string key)
    {
        if(_cacheDb.KeyExists(key))
            return _cacheDb.KeyDelete(key);
        return false;
    }

    public bool SetCacheItem<T>(string key, T modelToCache, DateTimeOffset expirationDate)
    {
        var expireyTime = expirationDate.DateTime.Subtract(DateTime.Now);
        if (_cacheDb.KeyExists(key))
        {
            var data=_cacheDb.StringGet(key);
            JsonConvert.DeserializeObject<T>(data);
            _cacheDb.StringAppend(key, JsonConvert.SerializeObject(modelToCache), CommandFlags.None);
        }

            
        return _cacheDb.StringSet(key, JsonConvert.SerializeObject(modelToCache), expireyTime);
    }
}