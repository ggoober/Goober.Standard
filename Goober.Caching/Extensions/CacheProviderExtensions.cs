using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Goober.Base.Extensions;
using Goober.Caching.Services;

namespace Goober.Caching.Extensions
{
    public static class CacheProviderExtensions
    {
        public static CacheProviderQueryBuilder GetBuilder(this ICacheProvider cacheProvider)
        {
            var result = new CacheProviderQueryBuilder(cacheProvider);
            return result;
        }

        public static CacheProviderQueryBuilder WithLifetimeInMilliseconds(this CacheProviderQueryBuilder builder,
            int? refreshTimeInMilliseconds = null,
            int? expirationTimeInMilliseconds = null)
        {
            builder.RefreshTimeInMilliseconds = refreshTimeInMilliseconds;
            builder.ExpirationTimeInMilliseconds = expirationTimeInMilliseconds;
            return builder;
        }

        public static CacheProviderQueryBuilder WithLifetimeInMinutes(this CacheProviderQueryBuilder builder,
            int? refreshTimeInMinutes = null,
            int? expirationTimeInMinutes = null)
        {
            builder.RefreshTimeInMilliseconds = refreshTimeInMinutes * 60000;
            builder.ExpirationTimeInMilliseconds = expirationTimeInMinutes * 60000;
            return builder;
        }

        public static CacheProviderQueryBuilder WithLock(this CacheProviderQueryBuilder builder,
            bool useLock = true)
        {
            builder.UseLock = useLock;
            return builder;
        }

        public static CacheProviderQueryBuilder WithCancellationToken(this CacheProviderQueryBuilder builder,
            CancellationToken cancellationToken)
        {
            builder.CancellationToken = cancellationToken;
            return builder;
        }

        public static CacheProviderQueryBuilder WithCacheKeys<TKey>(this CacheProviderQueryBuilder builder,
            params string[] keys)
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            builder.CacheKeys = keys.Distinct().ToList();

            return builder;
        }

        public static CacheProviderQueryBuilder WithKeys<TKey>(this CacheProviderQueryBuilder builder,
            IEnumerable<TKey> keys, Func<TKey, string> mapKeyFunc = null)
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            var strongBuilder = builder as CacheProviderQueryBuilderWithKey<TKey>
                                ?? new CacheProviderQueryBuilderWithKey<TKey>(builder);

            strongBuilder.Keys = keys.Distinct().ToList();

            if (mapKeyFunc != null)
                strongBuilder.CacheKeys = keys
                    .Select(mapKeyFunc)
                    .Distinct()
                    .ToList();

            return strongBuilder;
        }

        public static CacheProviderQueryBuilder MapToCacheKeys<TKey>(this CacheProviderQueryBuilder builder,
            Func<TKey, string> mapKeyFunc)
        {
            if (mapKeyFunc == null)
                throw new ArgumentNullException(nameof(mapKeyFunc));

            var strongBuilder = builder as CacheProviderQueryBuilderWithKey<TKey>
                                ?? new CacheProviderQueryBuilderWithKey<TKey>(builder);

            strongBuilder.CacheKeys = strongBuilder.Keys
                .Select(mapKeyFunc)
                .Distinct()
                .ToList();

            return strongBuilder;
        }

        public static async Task<TValue> GetAsync<TValue>(this CacheProviderQueryBuilder builder,
            Func<Task<TValue>> getItemfunc)
        {
            if (getItemfunc == null)
                throw new ArgumentNullException(nameof(getItemfunc));

            var result = builder.CacheKeys.Count > 0 
                ? await builder.CacheProvider.GetAsync(
                    cacheKey: builder.CacheKeys[0],
                    refreshTimeInMinutes: builder.RefreshTimeInMilliseconds / 60000,
                    expirationTimeInMinutes: builder.ExpirationTimeInMilliseconds / 60000,
                    func: getItemfunc,
                    useLock: builder.UseLock)
                : default(TValue);

            return result;
        }

        public static async Task<IEnumerable<KeyValuePair<TKey, TValue>>> GetManyAsync<TKey, TValue>(this CacheProviderQueryBuilder builder,
            Func<TKey[], CancellationToken?, Task<IEnumerable<KeyValuePair<TKey, TValue>>>> getItemsFunc)
        {
            if (getItemsFunc == null)
                throw new ArgumentNullException(nameof(getItemsFunc));

            var keyBuilder = builder as CacheProviderQueryBuilderWithKey<TKey>;
            if (keyBuilder == null)
                throw new ArgumentException(nameof(keyBuilder), $"Необходимо предварительно вызвать метод {nameof(WithKeys)}, добавив ключи");

            var keys = new Dictionary<string, TKey>();
            for (var i = 0; i < keyBuilder.Keys.Count; i++)
                keys.Add(keyBuilder.CacheKeys[i], keyBuilder.Keys[i]);

            var result = await builder.CacheProvider.GetManyAsync(
                cacheKeys: keys,
                refreshTimeInMilliseconds: builder.RefreshTimeInMilliseconds,
                expirationTimeInMilliseconds: builder.ExpirationTimeInMilliseconds,
                getItemsFunc: getItemsFunc,
                useLock: builder.UseLock);

            return result;
        }

        public static async Task<IEnumerable<TValue>> GetValuesForOneOrManyKeysAsync<TKey, TValue>(this CacheProviderQueryBuilder builder,
            Func<Task<TValue>> getItemFunc,
            Func<TKey[], CancellationToken?, Task<IEnumerable<KeyValuePair<TKey, TValue>>>> getItemsFunc = null)
        {
            if (getItemFunc == null)
                throw new ArgumentNullException(nameof(getItemFunc));

            var keyBuilder = builder as CacheProviderQueryBuilderWithKey<TKey>;

            var result = new List<TValue>();

            if (keyBuilder != null && keyBuilder.Keys.Count > 0 && builder.CacheKeys.Count == 0)
                builder.CacheKeys = keyBuilder.Keys
                    .Select(s => s.ToString())
                    .Distinct()
                    .ToList();

            if (builder.CacheKeys.Count > 1)
                if (getItemsFunc != null && keyBuilder != null)
                {
                    var keys = new Dictionary<string, TKey>();
                    for (var i = 0; i < keyBuilder.Keys.Count; i++)
                        keys.Add(keyBuilder.CacheKeys[i], keyBuilder.Keys[i]);

                    result.AddRange((await builder.CacheProvider.GetManyAsync(
                            cacheKeys: keys,
                            refreshTimeInMilliseconds: builder.RefreshTimeInMilliseconds,
                            expirationTimeInMilliseconds: builder.ExpirationTimeInMilliseconds,
                            getItemsFunc: getItemsFunc,
                            useLock: builder.UseLock))
                        .Select(s => s.Value));
                } else
                {
                    var split = builder.CacheKeys.Select(async key => await builder.CacheProvider.GetAsync(
                            cacheKey: key,
                            refreshTimeInMinutes: builder.RefreshTimeInMilliseconds / 60000,
                            expirationTimeInMinutes: builder.ExpirationTimeInMilliseconds / 60000,
                            func: getItemFunc,
                            useLock: builder.UseLock))
                        .SplitByCount(10);

                    foreach (var getValueTasks in split) 
                        result.AddRange(await Task.WhenAll(getValueTasks));
                }
            else if (builder.CacheKeys.Count == 1)
            {
                result.Add(await builder.CacheProvider.GetAsync(
                    cacheKey: builder.CacheKeys[0],
                    refreshTimeInMinutes: builder.RefreshTimeInMilliseconds / 60000,
                    expirationTimeInMinutes: builder.ExpirationTimeInMilliseconds / 60000,
                    func: getItemFunc,
                    useLock: builder.UseLock));
            }

            return result;
        }

        public class CacheProviderQueryBuilder
        {
            internal ICacheProvider CacheProvider { get; private set; }

            public CacheProviderQueryBuilder(ICacheProvider cacheProvider)
            {
                CacheProvider = cacheProvider;
            }

            public CacheProviderQueryBuilder(CacheProviderQueryBuilder builder)
            {
                Populate(builder);
            }

            internal List<string> CacheKeys { get; set; } = new List<string>();

            internal int? RefreshTimeInMilliseconds { get; set; }

            internal int? ExpirationTimeInMilliseconds { get; set; }

            internal bool UseLock { get; set; }

            internal CancellationToken CancellationToken { get; set; }

            internal object OneGetValueFunc
            { get; set; }

            internal virtual void Populate(CacheProviderQueryBuilder builder)
            {
                CacheProvider = builder.CacheProvider;
                RefreshTimeInMilliseconds = builder.RefreshTimeInMilliseconds;
                ExpirationTimeInMilliseconds = builder.ExpirationTimeInMilliseconds;
                UseLock = builder.UseLock;
                CancellationToken = builder.CancellationToken;
                OneGetValueFunc = builder.OneGetValueFunc;
            }
        }

        public class CacheProviderQueryBuilderWithKey<TKey> : CacheProviderQueryBuilder
        {
            public CacheProviderQueryBuilderWithKey(CacheProviderQueryBuilder builder)
                : base(builder)
            { }

            internal List<TKey> Keys { get; set; } = new List<TKey>();

            internal override void Populate(CacheProviderQueryBuilder builder)
            {
                base.Populate(builder);

                if (builder is CacheProviderQueryBuilderWithKey<TKey> strongBuilder)
                {
                    Keys = strongBuilder.Keys;
                }
            }
        }
    }
}
