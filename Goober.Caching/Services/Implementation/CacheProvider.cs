using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goober.Caching.Models;
using System.Threading;

namespace Goober.Caching.Services.Implementation
{
    public class CacheProvider : ICacheProvider
    {
        private class CacheResult<T>
        {
            public T TargetObject { get; set; }

            public DateTime? RefreshTime { get; set; }
        }

        #region fields

        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CacheProvider> _logger;
        private readonly ConcurrentDictionary<string, CachedEntryInfo> _cachedEntriesDict = new ConcurrentDictionary<string, CachedEntryInfo>();

        private readonly ConcurrentDictionary<string, SemaphoreSlim> _lockSemaphoreDict = new ConcurrentDictionary<string, SemaphoreSlim>();

        #endregion

        #region ctor

        public CacheProvider(IMemoryCache memoryCache,
            ILogger<CacheProvider> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        #endregion

        #region ICacheProvider

        public void Remove(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);

            _cachedEntriesDict.TryRemove(key: cacheKey, out var removedCachedEntry);

            _lockSemaphoreDict.TryRemove(key: cacheKey, out var removedLockSemaphore);
        }

        public int Remove(Func<string, bool> predicate)
        {
            var cachedKeys = _cachedEntriesDict.Keys.Where(predicate);

            var countRemovedKeys = 0;

            foreach (var cacheKey in cachedKeys)
            {
                Remove(cacheKey);
                countRemovedKeys++;
            }

            return countRemovedKeys;
        }

        public void RemoveAll()
        {
            var cachedEntriesKeys = _cachedEntriesDict.Select(x => x.Key);
            var lockSemaphoreKeys = _lockSemaphoreDict.Select(x => x.Key);

            var keyListToRemove = new List<string>();

            keyListToRemove.AddRange(cachedEntriesKeys);
            keyListToRemove.AddRange(lockSemaphoreKeys);

            keyListToRemove = keyListToRemove.Distinct().ToList();

            foreach (var iKey in keyListToRemove)
                _memoryCache.Remove(iKey);

            _lockSemaphoreDict.Clear();
            _cachedEntriesDict.Clear();
        }

        private class GetManyCachedResultInternalModel<T, TKeyMap>
        {
            public TKeyMap Map { get; set; }

            public CacheResult<T> CacheResult { get; set; }

            public CachedEntryInfo CacheEntry { get; set; }
        }


        public async Task<IEnumerable<KeyValuePair<TKeyMap, T>>> GetManyAsync<T, TKeyMap>(
            Dictionary<string, TKeyMap> cacheKeys,
            int? refreshTimeInMilliseconds,
            int? expirationTimeInMilliseconds,
            Func<TKeyMap[], CancellationToken?, Task<IEnumerable<KeyValuePair<TKeyMap, T>>>> getItemsFunc,
            CancellationToken? cancellationToken = null,
            bool useLock = false)
        {
            CachedEntryInfo existedCachedEntry;
            var currentDateTime = DateTime.Now;

            var cachedResults = cacheKeys
                .Select(cacheKey => (cacheKey, _memoryCache.Get(cacheKey.Key) as CacheResult<T>))
                .ToDictionary(s => s.cacheKey.Key, s => new GetManyCachedResultInternalModel<T, TKeyMap>
                {
                    Map = s.cacheKey.Value,
                    CacheResult = s.Item2,
                    CacheEntry = s.Item2 != null && _cachedEntriesDict.TryGetValue(s.cacheKey.Key, out existedCachedEntry)
                        ? existedCachedEntry
                        : null
                });

            if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                throw new TaskCanceledException();

            var requestObjectSet = cachedResults
                .Where(s => s.Value.CacheResult == null
                            || s.Value.CacheResult.RefreshTime <= currentDateTime
                            || s.Value.CacheEntry?.ExpirationDateTime <= currentDateTime)
                .ToList();

            var lockEntries = useLock
                ? requestObjectSet
                : requestObjectSet
                    .Where(s => s.Value.CacheEntry?.UseLock ?? false)
                    .ToList();


            ConcurrentBag<SemaphoreSlim> unlockSemaphores = null;

            try
            {
                if (lockEntries.Count > 0)
                {
                    var lockSemaphores = lockEntries
                        .OrderBy(s => s.Key)
                        .Select(s
                            => _lockSemaphoreDict.GetOrAdd(s.Key, new SemaphoreSlim(1, 1)))
                        .ToList();

                    unlockSemaphores = new ConcurrentBag<SemaphoreSlim>();

                    if (cancellationToken.HasValue)
                        foreach (var lockSemaphore in lockSemaphores)
                        {
                            await lockSemaphore.WaitAsync(cancellationToken.Value);
                            unlockSemaphores.Add(lockSemaphore);
                        }
                    else
                        foreach (var lockSemaphore in lockSemaphores)
                        {
                            await lockSemaphore.WaitAsync();
                            unlockSemaphores.Add(lockSemaphore);
                        }
                }

                if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                    throw new TaskCanceledException();

                var responseObjects = new Dictionary<string, T>();

                if (requestObjectSet.Count > 0)
                {
                    var reverseKeyMap = cacheKeys.ToDictionary(s => s.Value, s => s.Key);

                    var mapCacheKeys = requestObjectSet
                        .Select(s => s.Value.Map)
                        .ToArray();

                    var responseFunc = await getItemsFunc(mapCacheKeys, cancellationToken);

                    foreach (var responseItem in responseFunc)
                    {
                        if (reverseKeyMap.TryGetValue(responseItem.Key, out var cacheKeyOrigin) &&
                            responseObjects.ContainsKey(cacheKeyOrigin) == false)
                        {
                            responseObjects.Add(cacheKeyOrigin, responseItem.Value);
                        }
                    }

                    if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                        throw new TaskCanceledException();

                    foreach (var iRequestObject in requestObjectSet)
                    {
                        if (cachedResults.TryGetValue(iRequestObject.Key, out var cachedResultForMerge) == false)
                        {
                            continue;
                        }

                        var responseValueTyped = responseObjects.TryGetValue(iRequestObject.Key, out var responseValue) == true ? responseValue : default(T);

                        if (cachedResultForMerge.CacheResult != null)
                        {
                            cachedResultForMerge.CacheResult.TargetObject = responseValueTyped;
                            cachedResultForMerge.CacheResult.RefreshTime = refreshTimeInMilliseconds.HasValue ? currentDateTime.AddMilliseconds(refreshTimeInMilliseconds.Value) : (DateTime?)null;

                            cachedResultForMerge.CacheEntry.UseLock = useLock || cachedResultForMerge.CacheEntry.UseLock;
                            cachedResultForMerge.CacheEntry.LastRefreshDateTime = currentDateTime;
                            cachedResultForMerge.CacheEntry.NextRefreshDateTime = cachedResultForMerge.CacheResult.RefreshTime;
                        }
                        else
                        {
                            var addResult = AddNewRecordToCache(
                                cacheKey: iRequestObject.Key,
                                refreshTime: ConvertTimeMillisecondsToTimeSpan(refreshTimeInMilliseconds),
                                expirationTime: ConvertTimeMillisecondsToTimeSpan(expirationTimeInMilliseconds),
                                currentDateTime: currentDateTime,
                                useLock: useLock,
                                expensiveObject: responseValueTyped);

                            cachedResultForMerge.CacheEntry = addResult.CachedEntry;
                            cachedResultForMerge.CacheResult = addResult.CachedResult;
                        }
                    }
                }

                foreach (var cacheSet in cachedResults
                             .Where(s => s.Value.CacheEntry != null && s.Value.CacheResult != null))
                {
                    cacheSet.Value.CacheEntry.LastAccessDateTime = currentDateTime;
                    cacheSet.Value.CacheEntry.IsEmpty = cacheSet.Value.CacheResult.TargetObject == null;
                }

                var result = cachedResults
                    .Select(s => new KeyValuePair<TKeyMap, T>(s.Value.Map, s.Value.CacheResult.TargetObject))
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, message: $"Error while refreshing cache");
                throw;
            }
            finally
            {
                if (unlockSemaphores != null && unlockSemaphores.Count > 0)
                    foreach (var semaphore in unlockSemaphores)
                        semaphore.Release();
            }
        }

        public async Task<T> GetAsync<T>(string cacheKey, int? refreshTimeInMinutes, int? expirationTimeInMinutes, Func<Task<T>> func, bool useLock = false)
        {
            var cachedResult = _memoryCache.Get(cacheKey) as CacheResult<T>;

            var currentDateTime = DateTime.Now;

            SemaphoreSlim lockSemaphore = null;

            if (cachedResult != null)
            {
                _cachedEntriesDict.TryGetValue(cacheKey, out var existedCachedEnty);

                if (cachedResult.RefreshTime <= currentDateTime)
                {
                    if (useLock == true || existedCachedEnty?.UseLock == true)
                    {
                        lockSemaphore = _lockSemaphoreDict.GetOrAdd(cacheKey, new SemaphoreSlim(1, 1));
                        await lockSemaphore.WaitAsync();
                    }

                    try
                    {
                        cachedResult.TargetObject = await func();

                        AddNewRecordToCache(
                            cacheKey: cacheKey,
                            refreshTime: ConvertTimeMinutesToTimeSpan(refreshTimeInMinutes),
                            expirationTime: ConvertTimeMinutesToTimeSpan(expirationTimeInMinutes),
                            currentDateTime: currentDateTime,
                            useLock: useLock,
                            expensiveObject: cachedResult.TargetObject);
                    }
                    catch (Exception exc)
                    {
                        _logger.LogError(exception: exc, message: $"Error while refreshing cache");
                    }
                    finally
                    {
                        if (lockSemaphore != null)
                        {
                            lockSemaphore.Release();
                        }
                    }
                }

                if (existedCachedEnty != null)
                {
                    existedCachedEnty.LastAccessDateTime = currentDateTime;
                    existedCachedEnty.IsEmpty = cachedResult.TargetObject == null;
                }

                if (cachedResult.TargetObject == null)
                    return default(T);

                return cachedResult.TargetObject;
            }

            if (useLock == true)
            {
                lockSemaphore = _lockSemaphoreDict.GetOrAdd(cacheKey, new SemaphoreSlim(1, 1));
                await lockSemaphore.WaitAsync();
            }

            try
            {
                var ret = AddNewRecordToCache(cacheKey: cacheKey,
                    refreshTime: ConvertTimeMinutesToTimeSpan(refreshTimeInMinutes),
                    expirationTime: ConvertTimeMinutesToTimeSpan(expirationTimeInMinutes),
                    currentDateTime: currentDateTime,
                    useLock: useLock,
                    expensiveObject: await func());

                return ret.CachedResult.TargetObject;
            }
            finally
            {
                if (lockSemaphore != null)
                {
                    lockSemaphore.Release();
                }
            }
        }

        public async Task<List<R>> GetManyAsync<P, R>(string cachePrefix,
           List<P> parameters,
           int? refreshTimeInMinutes,
           int? expirationTimeInMinutes,
           Func<List<P>, Task<Dictionary<P, R>>> func)
        {
            var notCachedParameters = new List<P>();

            var dateNow = DateTime.Now;

            var ret = new List<R>();

            foreach (var iParameter in parameters)
            {
                var cacheKey = cachePrefix + iParameter.ToString();
                var cachedResult = _memoryCache.Get(cacheKey) as CacheResult<R>;

                if (cachedResult == null || cachedResult.RefreshTime < dateNow)
                {
                    notCachedParameters.Add(iParameter);
                    continue;
                }

                if (cachedResult.TargetObject == null)
                {
                    continue;
                }

                ret.Add(cachedResult.TargetObject);
            }

            if (notCachedParameters.Count > 0)
            {
                var funcResult = await func(notCachedParameters);

                foreach (var iNotCachedParameter in notCachedParameters)
                {
                    var cacheKey = cachePrefix + iNotCachedParameter.ToString();

                    funcResult.TryGetValue(key: iNotCachedParameter, out var expensiveObject);

                    AddNewRecordToCache(cacheKey: cacheKey,
                        refreshTime: ConvertTimeMinutesToTimeSpan(refreshTimeInMinutes),
                        expirationTime: ConvertTimeMinutesToTimeSpan(expirationTimeInMinutes),
                        currentDateTime: dateNow,
                        useLock: false,
                        expensiveObject: expensiveObject);

                    if (expensiveObject == null)
                    {
                        continue;
                    }

                    ret.Add(expensiveObject);
                }
            }

            return ret;
        }

        private (CacheResult<T> CachedResult, CachedEntryInfo CachedEntry) AddNewRecordToCache<T>(
            string cacheKey,
            TimeSpan? refreshTime,
            TimeSpan? expirationTime,
            DateTime currentDateTime,
            bool useLock,
            T expensiveObject)
        {
            var absoluteRefreshDateTime = refreshTime.HasValue == true ? currentDateTime.Add(refreshTime.Value) : (DateTime?)null;
            var absoluteExpirationDateTime = expirationTime != null ? currentDateTime.Add(expirationTime.Value) : (DateTime?)null;

            var cachedEntry = _cachedEntriesDict.GetOrAdd(key: cacheKey, valueFactory: (key) =>
            {
                return new CachedEntryInfo
                {
                    RowCreatedDateTime = currentDateTime
                };
            });

            cachedEntry.ExpirationTimeInMilliseconds = expirationTime.HasValue ? ConvertToInt(expirationTime.Value.TotalMilliseconds) : (int?)null;
            cachedEntry.ExpirationDateTime = absoluteExpirationDateTime;

            cachedEntry.RefreshTimeInMilliseconds = refreshTime.HasValue ? ConvertToInt(refreshTime.Value.TotalMilliseconds) : (int?)null;
            cachedEntry.NextRefreshDateTime = absoluteRefreshDateTime;

            cachedEntry.LastAccessDateTime = currentDateTime;
            cachedEntry.UseLock = useLock;
            cachedEntry.IsEmpty = expensiveObject == null;

            var newCachedResult = new CacheResult<T>
            {
                RefreshTime = absoluteRefreshDateTime,
                TargetObject = expensiveObject
            };

            if (expirationTime.HasValue == true)
            {
                var absoluteExpiration = new DateTimeOffset(DateTime.Now.Add(expirationTime.Value));
                cachedEntry.ExpirationDateTime = absoluteExpiration.DateTime;
                _memoryCache.Set(key: cacheKey, value: newCachedResult, absoluteExpiration: absoluteExpiration);
            }
            else
            {
                _memoryCache.Set(key: cacheKey, value: newCachedResult);
            }

            return (CachedResult: newCachedResult, CachedEntry: cachedEntry);
        }

        public CachedEntryInfo GetCachedEnty(string key)
        {
            _cachedEntriesDict.TryGetValue(key: key, out var result);

            return result;
        }

        public T Get<T>(string key)
        {
            var cachedResult = _memoryCache.Get(key) as CacheResult<T>;

            if (cachedResult == null || cachedResult.TargetObject == null)
            {
                return default(T);
            }

            return cachedResult.TargetObject;
        }

        public void Add<T>(string cacheKey, T value, int? refreshTimeInMinutes, int? expirationTimeInMinutes)
        {
            var cachedResult = _memoryCache.Get(cacheKey) as CacheResult<T>;

            var currentDateTime = DateTime.Now;

            if (cachedResult != null)
            {
                _cachedEntriesDict.TryGetValue(cacheKey, out var existedCachedEnty);

                if (refreshTimeInMinutes.HasValue == true
                    && cachedResult.RefreshTime <= currentDateTime)
                {
                    cachedResult.TargetObject = value;
                    cachedResult.RefreshTime = currentDateTime.AddMinutes(refreshTimeInMinutes.Value);

                    if (existedCachedEnty != null)
                    {
                        existedCachedEnty.UseLock = existedCachedEnty.UseLock;
                        existedCachedEnty.LastRefreshDateTime = currentDateTime;
                        existedCachedEnty.NextRefreshDateTime = cachedResult.RefreshTime;
                    }
                }

                if (existedCachedEnty != null)
                {
                    existedCachedEnty.LastAccessDateTime = currentDateTime;
                    existedCachedEnty.IsEmpty = cachedResult.TargetObject == null;
                }
            }

            AddNewRecordToCache(cacheKey: cacheKey,
                refreshTime: ConvertTimeMinutesToTimeSpan(refreshTimeInMinutes),
                expirationTime: ConvertTimeMinutesToTimeSpan(expirationTimeInMinutes),
                currentDateTime: currentDateTime,
                useLock: false,
                expensiveObject: value);
        }

        public Dictionary<string, CachedEntryInfo> GetCachedEntries()
        {
            var currentDateTime = DateTime.Now;

            var expiredEntries = _cachedEntriesDict.Where(x => x.Value?.ExpirationDateTime < currentDateTime).ToList();

            foreach (var iCachedEntryWithKey in expiredEntries)
            {
                _cachedEntriesDict.TryRemove(iCachedEntryWithKey.Key, out var removedCachedEntry);
            }

            return _cachedEntriesDict.ToDictionary(x => x.Key, x => x.Value);
        }

        #endregion

        private static int ConvertToInt(double value)
        {
            var result = value > int.MaxValue ? int.MaxValue : (int)value;
            return result;
        }

        private static TimeSpan? ConvertTimeMinutesToTimeSpan(int? timeInMinutes)
        {
            var result = timeInMinutes.HasValue == true ? TimeSpan.FromMinutes((double)timeInMinutes) : (TimeSpan?)null;
            return result;
        }

        private static TimeSpan? ConvertTimeMillisecondsToTimeSpan(int? timeInMilliseconds)
        {
            var result = timeInMilliseconds.HasValue == true ? TimeSpan.FromMilliseconds((double)timeInMilliseconds) : (TimeSpan?)null;
            return result;
        }
    }
}
