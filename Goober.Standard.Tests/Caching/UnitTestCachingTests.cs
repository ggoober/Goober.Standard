using Goober.Caching;
using Goober.Caching.Extensions;
using Goober.Caching.Services;
using Goober.Standard.Tests.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Goober.Standard.Tests.Caching
{
    public class UnitTestCachingTests
    {
        [Fact]
        public async Task AddAndGet_ShouldReturnObject()
        {
            using (var testTools = TestTools.GetTestScope(services => services.AddIndusoftCaching(null)))
            {
                var cacheProvider = testTools.ServiceProvider.GetService<ICacheProvider>();

                var cacheKey = Guid.NewGuid().ToString();

                var value = Guid.NewGuid();

                cacheProvider.Add(cacheKey, value, refreshTimeInMinutes: 5, expirationTimeInMinutes: 5);

                var cacheResult = cacheProvider.Get<Guid>(cacheKey);

                Assert.Equal(value, cacheResult);
            }
        }

        [Fact]
        public async Task GetManyAsync_10ItemsCached_10ItemsReturns()
        {
            using (var testTools = TestTools.GetTestScope(services => services.AddIndusoftCaching(null)))
            {
                var cacheProvider = testTools.ServiceProvider.GetService<ICacheProvider>();

                KeyValuePair<string, Guid> getCacheKeyPair()
                {
                    var key = Guid.NewGuid();
                    return new KeyValuePair<string, Guid>($"id-{key}", key);
                };

                var cacheKeys = Enumerable.Range(0, 10)
                    .Select(s => getCacheKeyPair())
                    .ToDictionary(s => s.Key, s => s.Value);

                var cntGetItems = 0;

                var firstResponse = (await cacheProvider.GetManyAsync<int, Guid>(
                        cacheKeys: cacheKeys,
                        refreshTimeInMilliseconds: null,
                        expirationTimeInMilliseconds: null,
                        getItemsFunc: async (guids, token) =>
                        {
                            cntGetItems++;
                            return guids.Select(s => new KeyValuePair<Guid, int>(s, cntGetItems)).ToList();
                        },
                        cancellationToken: null,
                        useLock: false))
                    .ToDictionary(s => s.Key, s => s.Value);

                var secondResponse = (await cacheProvider.GetManyAsync<int, Guid>(
                        cacheKeys: cacheKeys,
                        refreshTimeInMilliseconds: null,
                        expirationTimeInMilliseconds: null,
                        getItemsFunc: async (guids, token) =>
                        {
                            cntGetItems++;
                            return guids.Select(s => new KeyValuePair<Guid, int>(s, cntGetItems)).ToList();
                        },
                        cancellationToken: null,
                        useLock: false))
                    .ToDictionary(s => s.Key, s => s.Value);

                Assert.Equal(1, cntGetItems);
                Assert.Equal(10, firstResponse.Count);
                Assert.Equal(secondResponse.Count, firstResponse.Count);
                Assert.True(secondResponse.All(s => firstResponse[s.Key] == s.Value));
            }
        }

        [Fact]
        public async Task GetManyAsync_Request8Items_Returns5Items_ShouldCacheAndReturn8Items()
        {
            using (var testTools = TestTools.GetTestScope(services => services.AddIndusoftCaching(null)))
            {
                var cacheProvider = testTools.ServiceProvider.GetService<ICacheProvider>();

                KeyValuePair<string, Guid> getCacheKeyPair()
                {
                    var key = Guid.NewGuid();
                    return new KeyValuePair<string, Guid>($"id-{key}", key);
                };

                var cacheKeys = Enumerable.Range(0, 8)
                    .Select(s => getCacheKeyPair())
                    .ToDictionary(s => s.Key, s => s.Value);

                var responseGuids = cacheKeys.Take(5);

                var response = (await cacheProvider.GetManyAsync<int, Guid>(
                        cacheKeys: cacheKeys,
                        refreshTimeInMilliseconds: null,
                        expirationTimeInMilliseconds: null,
                        getItemsFunc: async (guids, token) =>
                        {
                            return responseGuids.Select(s => new KeyValuePair<Guid, int>(s.Value, 1)).ToList();
                        },
                        cancellationToken: null,
                        useLock: false))
                    .ToDictionary(s => s.Key, s => s.Value);

                Assert.Equal(8, response.Count);
                Assert.True(response.All(s => response[s.Key] == s.Value));
            }
        }


        [Fact]
        public async Task GetManyAsync_10ItemsRequested_5ItemsReturnedFirstTime_8ItemsFromSecond_ShouldCacheAndReturn10Items()
        {
            using (var testTools = TestTools.GetTestScope(services => services.AddIndusoftCaching(null)))
            {
                var cacheProvider = testTools.ServiceProvider.GetService<ICacheProvider>();

                KeyValuePair<string, Guid> getCacheKeyPair()
                {
                    var key = Guid.NewGuid();
                    return new KeyValuePair<string, Guid>($"id-{key}", key);
                };

                var cacheKeys = Enumerable.Range(0, 10)
                    .Select(s => getCacheKeyPair())
                    .ToDictionary(s => s.Key, s => s.Value);

                var firstReturnItems = cacheKeys.Take(5);

                var secondReturnItems = cacheKeys.Skip(5).Take(3);

                var cntGetItems = 0;

                var firstResponse = (await cacheProvider.GetManyAsync<int, Guid>(
                        cacheKeys: cacheKeys,
                        refreshTimeInMilliseconds: 10,
                        expirationTimeInMilliseconds: null,
                        getItemsFunc: async (guids, token) =>
                        {
                            cntGetItems++;
                            return firstReturnItems.Select(s => new KeyValuePair<Guid, int>(s.Value, 1)).ToList();
                        },
                        cancellationToken: null,
                        useLock: false))
                    .ToDictionary(s => s.Key, s => s.Value);

                await Task.Delay(10);

                var secondResponse = (await cacheProvider.GetManyAsync<int, Guid>(
                        cacheKeys: cacheKeys,
                        refreshTimeInMilliseconds: null,
                        expirationTimeInMilliseconds: null,
                        getItemsFunc: async (guids, token) =>
                        {
                            cntGetItems++;
                            return secondReturnItems.Select(s => new KeyValuePair<Guid, int>(s.Value, 2)).ToList();
                        },
                        cancellationToken: null,
                        useLock: false))
                    .ToDictionary(s => s.Key, s => s.Value);

                var cachedEntries = cacheProvider.GetCachedEntries();
                Assert.Equal(2, cntGetItems);
                Assert.Equal(10, firstResponse.Count);
                Assert.Equal(10, secondResponse.Count);
                Assert.Equal(10, cachedEntries.Count);
            }
        }


        [Fact]
        public async Task CacheProvider_GetManyAsync_10ItemsCached_10ItemsRefresh()
        {
            using (var testTools = TestTools.GetTestScope(services => services.AddIndusoftCaching(null)))
            {
                var cacheProvider = testTools.ServiceProvider.GetService<ICacheProvider>();

                KeyValuePair<string, Guid> getCacheKeyPair()
                {
                    var key = Guid.NewGuid();
                    return new KeyValuePair<string, Guid>($"id-{key}", key);
                };

                var cacheKeys = Enumerable.Range(0, 10)
                    .Select(s => getCacheKeyPair())
                    .ToDictionary(s => s.Key, s => s.Value);

                var firstResponse = (await cacheProvider.GetManyAsync<int, Guid>(
                        cacheKeys: cacheKeys,
                        refreshTimeInMilliseconds: 10,
                        expirationTimeInMilliseconds: null,
                        getItemsFunc: async (guids, token) =>
                        {
                            return guids.Select(s => new KeyValuePair<Guid, int>(s, 1)).ToList();
                        },
                        cancellationToken: null,
                        useLock: false))
                    .ToDictionary(s => s.Key, s => s.Value);

                await Task.Delay(10);

                var secondResponse = (await cacheProvider.GetManyAsync<int, Guid>(
                        cacheKeys: cacheKeys,
                        refreshTimeInMilliseconds: 1000,
                        expirationTimeInMilliseconds: null,
                        getItemsFunc: async (guids, token) =>
                        {
                            return guids.Select(s => new KeyValuePair<Guid, int>(s, 2)).ToList();
                        },
                        cancellationToken: null,
                        useLock: false))
                    .ToDictionary(s => s.Key, s => s.Value);

                Assert.Equal(10, firstResponse.Count);
                Assert.Equal(secondResponse.Count, firstResponse.Count);
                Assert.True(firstResponse.All(s => secondResponse[s.Key] == 2));
            }
        }

        [Fact]
        public async Task CacheProvider_GetManyAsync_10ItemsCached_10ItemsExpired()
        {
            using (var testTools = TestTools.GetTestScope(services => services.AddIndusoftCaching(null)))
            {
                var cacheProvider = testTools.ServiceProvider.GetService<ICacheProvider>();

                KeyValuePair<string, Guid> getCacheKeyPair()
                {
                    var key = Guid.NewGuid();
                    return new KeyValuePair<string, Guid>($"id-{key}", key);
                };

                var cacheKeys = Enumerable.Range(0, 10)
                    .Select(s => getCacheKeyPair())
                    .ToDictionary(s => s.Key, s => s.Value);

                var firstResponse = (await cacheProvider.GetManyAsync<int, Guid>(
                        cacheKeys: cacheKeys,
                        refreshTimeInMilliseconds: null,
                        expirationTimeInMilliseconds: 10,
                        getItemsFunc: async (guids, token) =>
                        {
                            return guids.Select(s => new KeyValuePair<Guid, int>(s, 1)).ToList();
                        },
                        cancellationToken: null,
                        useLock: false))
                    .ToDictionary(s => s.Key, s => s.Value);

                await Task.Delay(10);

                var secondResponse = (await cacheProvider.GetManyAsync<int, Guid>(
                        cacheKeys: cacheKeys,
                        refreshTimeInMilliseconds: null,
                        expirationTimeInMilliseconds: null,
                        getItemsFunc: async (guids, token) =>
                        {
                            return guids.Select(s => new KeyValuePair<Guid, int>(s, 2)).ToList();
                        },
                        cancellationToken: null,
                        useLock: false))
                    .ToDictionary(s => s.Key, s => s.Value);

                Assert.Equal(10, firstResponse.Count);
                Assert.Equal(secondResponse.Count, firstResponse.Count);
                Assert.True(firstResponse.All(s => secondResponse[s.Key] == s.Value + 1));
            }
        }

        [Fact]
        public async Task CacheProviderQueryBuilder_GetValuesForOneOrManyKeysAsync_10ItemsCached_10ItemsWithoutRefresh()
        {
            using (var testTools = TestTools.GetTestScope(services => services.AddIndusoftCaching(null)))
            {
                var cacheProvider = testTools.ServiceProvider.GetService<ICacheProvider>();

                KeyValuePair<string, Guid> getCacheKeyPair()
                {
                    var key = Guid.NewGuid();
                    return new KeyValuePair<string, Guid>($"id-{key}", key);
                };

                var cacheKeys = Enumerable.Range(1, 10)
                    .Select(s => getCacheKeyPair())
                    .ToDictionary(s => s.Key, s => s.Value);

                var cntGetItems = 0;

                var firstResponse = (await cacheProvider.GetBuilder()
                    .WithKeys(cacheKeys.Values, key => $"id-{key}")
                    .GetValuesForOneOrManyKeysAsync<Guid, int>(() => Task.FromResult(Interlocked.Increment(ref cntGetItems))))
                    .ToList();

                await Task.Delay(10);

                var secondResponse = (await cacheProvider.GetBuilder()
                        .WithKeys(cacheKeys.Values, key => $"id-{key}")
                        .WithLifetimeInMilliseconds(10)
                        .GetValuesForOneOrManyKeysAsync<Guid, int>(() => Task.FromResult(Interlocked.Increment(ref cntGetItems))))
                    .ToList();

                var sum = Enumerable.Range(1, 10).Sum();

                Assert.Equal(10, firstResponse.Count);
                Assert.Equal(secondResponse.Count, firstResponse.Count);
                Assert.Equal(sum, firstResponse.Sum());
                Assert.Equal(sum, secondResponse.Sum());
            }
        }

        [Fact]
        public async Task CacheProviderQueryBuilder_GetManyAsync_10ItemsCached_10ItemsExpired()
        {
            using (var testTools = TestTools.GetTestScope(services => services.AddIndusoftCaching(null)))
            {
                var cacheProvider = testTools.ServiceProvider.GetService<ICacheProvider>();

                await Assert.ThrowsAsync<ArgumentException>(async () => await cacheProvider.GetBuilder().GetManyAsync<object, object>(
                    async (objects, token) => new List<KeyValuePair<object, object>>()));
            }
        }

        [Fact]
        public async Task GetManyAsync_2ItemsRequested_CacheExpired_FirstRequest2SomeItemsReturns_SecondRequest2SomeItemsReturns()
        {
            using (var testTools = TestTools.GetTestScope(services => services.AddIndusoftCaching(null)))
            {
                var cacheProvider = testTools.ServiceProvider.GetService<ICacheProvider>();

                var cacheKeys = new Dictionary<string, Guid>()
                {
                    {"key1" , Guid.NewGuid()},
                    {"key2" , Guid.NewGuid()}
                };

                var result1 = await cacheProvider.GetManyAsync<int, Guid>(
                    cacheKeys: cacheKeys,
                    refreshTimeInMilliseconds: null,
                    expirationTimeInMilliseconds: 1000,
                    getItemsFunc: async (values, token) =>
                    {
                        var resultItems = values.Select(s => new KeyValuePair<Guid, int>(s, 1)).ToList();
                        return resultItems;
                    },
                    cancellationToken: null,
                    useLock: false);

                await Task.Delay(1500);

                var result2 = await cacheProvider.GetManyAsync<int, Guid>(
                   cacheKeys: cacheKeys,
                   refreshTimeInMilliseconds: null,
                   expirationTimeInMilliseconds: 864000000,
                   getItemsFunc: async (values, token) =>
                   {
                       var resultItems = values.Select(s => new KeyValuePair<Guid, int>(s, 2)).ToList();
                       return resultItems;
                   },
                   cancellationToken: null,
                   useLock: false);

                var result3 = await cacheProvider.GetManyAsync<int, Guid>(
                   cacheKeys: cacheKeys,
                   refreshTimeInMilliseconds: null,
                   expirationTimeInMilliseconds: 864000000,
                   getItemsFunc: async (values, token) =>
                   {
                       var resultItems = values.Select(s => new KeyValuePair<Guid, int>(s, 3)).ToList();
                       return resultItems;
                   },
                   cancellationToken: null,
                   useLock: false);

                Assert.True(result1.All(r => r.Value == 1));
                Assert.True(result2.All(r => r.Value == 2));
                Assert.True(result3.All(r => r.Value == 2));
            }
        }

        [Fact]
        public async Task CacheProvider_Remove_2ItemsCached_1ItemRemovedByPrefix()
        {
            using (var testTools = TestTools.GetTestScope(services => services.AddIndusoftCaching(null)))
            {
                var cacheProvider = testTools.ServiceProvider.GetService<ICacheProvider>();

                var prefix = "Prefix";

                var result1 = await cacheProvider.GetAsync($"{prefix}_first", 1, 1, () => Task.FromResult("Done"));

                var result2 = await cacheProvider.GetAsync($"Second", 1, 1, () => Task.FromResult("Done"));

                var countRemoveKeys = cacheProvider.Remove(cacheKey => cacheKey.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

                Assert.True(countRemoveKeys == 1);
            }
        }
    }
}
