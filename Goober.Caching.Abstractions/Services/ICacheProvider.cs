using Goober.Caching.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Goober.Caching.Services
{
    public interface ICacheProvider
    {
        void Remove(string cacheKey);

        /// <summary>
        /// Удалить сущности из кэша на основе предиката
        /// </summary>
        /// <param name="predicate">Функция для проверки каждого кэш-ключа</param>
        /// <returns>Количество удаленных сущностей</returns>
        int Remove(Func<string, bool> predicate);

        Task<T> GetAsync<T>(string cacheKey, int? refreshTimeInMinutes, int? expirationTimeInMinutes, Func<Task<T>> func, bool useLock = false);

        Task<List<R>> GetManyAsync<P, R>(string cachePrefix, List<P> parameters, int? refreshTimeInMinutes, int? expirationTimeInMinutes, Func<List<P>, Task<Dictionary<P, R>>> func);

        CachedEntryInfo GetCachedEnty(string key);

        Dictionary<string, CachedEntryInfo> GetCachedEntries();
        
        void RemoveAll();

        Task<IEnumerable<KeyValuePair<TKeyMap, T>>> GetManyAsync<T, TKeyMap>(
            Dictionary<string, TKeyMap> cacheKeys,
            int? refreshTimeInMilliseconds,
            int? expirationTimeInMilliseconds,
            Func<TKeyMap[], CancellationToken?, Task<IEnumerable<KeyValuePair<TKeyMap, T>>>> getItemsFunc,
            CancellationToken? cancellationToken = null,
            bool useLock = false);

        T Get<T>(string key);

        void Add<T>(string cacheKey, T value, int? refreshTimeInMinutes, int? expirationTimeInMinutes);

    }
}
