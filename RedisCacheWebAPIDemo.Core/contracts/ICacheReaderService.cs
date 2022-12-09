using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisCacheWebAPIDemo.Core.contracts;

public interface ICacheReaderService
{
    T ReadCache<T>(string key);
    bool SetCacheItem<T>(string key, T modelToCache, DateTimeOffset expirationDate);
    object RemoveCacheItem(string key);
}
