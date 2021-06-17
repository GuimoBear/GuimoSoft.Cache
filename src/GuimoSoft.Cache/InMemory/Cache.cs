using GuimoSoft.Cache.Utils;
using System;
using System.Collections.Generic;
using System.Threading;

namespace GuimoSoft.Cache.InMemory
{
    public partial class Cache<TKey, TValue> : ICache<TKey, TValue>
    {
        private readonly IDictionary<TKey, CacheItem<TValue>> _cache;

        private readonly CacheConfigurations<TKey, TValue> _configs;

        public Cache(Action<ICacheConfigurationsBuilder<TKey, TValue>> configure)
        {
            if (configure is null)
                throw new ArgumentNullException(nameof(configure));
            var builder = new CacheConfigurations<TKey, TValue>.CacheConfigurationsBuilder();
            configure(builder);
            _configs = builder.Build();

            _cache = new Dictionary<TKey, CacheItem<TValue>>(_configs.KeyEqualityComparer);
            if (_configs.UseCleaner)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _clearInstances = ClearInstances();
            }
        }

        private bool TryGetItemInCache(TKey key, out CacheState cacheState, out TValue value)
        {
            value = default;
            var now = DateTime.UtcNow;
            if (_cache.TryGetValue(key, out var item))
            {
                if (item.TTL > now)
                {
                    cacheState = CacheState.FoundedAndValid;
                    value = item.Value;
                    return true;
                }
                cacheState = CacheState.Expired;
                return false;
            }
            cacheState = CacheState.NotFound;
            return false;
        }

        private void OnInstanceCreated(TKey key, CacheState cacheState, TValue value)
        {
            switch (cacheState)
            {
                case CacheState.Expired:
                    Update(key, value, DateTime.UtcNow.Add(_configs.TTL));
                    break;
                case CacheState.NotFound:
                    Add(key, value, DateTime.UtcNow.Add(_configs.TTL));
                    break;
            }
        }

        public CacheItemBuilder<TKey, TValue> Get(TKey key)
            => new CacheItemBuilder<TKey, TValue>(key, TryGetItemInCache, OnInstanceCreated);

        public void Dispose()
        {
            if (_configs.UseCleaner)
            {
                _cancellationTokenSource.Cancel();
                _clearInstances.Wait();
                _cancellationTokenSource.Dispose();
            }
        }
    }
}
