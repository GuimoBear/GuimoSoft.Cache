using GuimoSoft.Cache.Utils;
using System;
using System.Threading.Tasks;

namespace GuimoSoft.Cache
{

    public class CacheItemBuilder<TKey, TValue>
    {
        public delegate TValue Factory();
        public delegate Task<TValue> AsyncFactory();

        internal delegate bool TryGetCachedItem(TKey key, out CacheState cacheState, out TValue value);
        internal delegate void ValueCreated(TKey key, CacheState cacheState, TValue value);

        private readonly TKey _key;
        private readonly TryGetCachedItem _tryGetCachedValueFunc;
        private readonly ValueCreated _onValueCreated;

        private Factory _valueFactory = default;

        internal CacheItemBuilder(
            TKey key, 
            TryGetCachedItem tryGetCachedValueFunc, 
            ValueCreated onValueCreated)
        {
            _key = key;
            _tryGetCachedValueFunc = tryGetCachedValueFunc;
            _onValueCreated = onValueCreated;
        }

        public TValue OrAdd(Factory factory)
        {
            _valueFactory = factory;
            return this;
        }

        public async Task<TValue> OrAddAsync(AsyncFactory asyncFactory)
        {
            if (_tryGetCachedValueFunc(_key, out var cacheState, out var value))
                return value;
            if (asyncFactory is not null)
            {
                value = await asyncFactory();
                _onValueCreated(_key, cacheState, value);
                return value;
            }
            return default;
        }

        private TValue Build()
        {
            if (_tryGetCachedValueFunc(_key, out var cacheState, out var value))
                return value;
            if (_valueFactory is not null)
            {
                value = _valueFactory();
                _onValueCreated(_key, cacheState, value);
                return value;
            }
            return default;
        }

        public static implicit operator TValue(CacheItemBuilder<TKey, TValue> cacheBuilder)
        {
            if (cacheBuilder is not null)
                return cacheBuilder.Build();
            return default;
        }
    }
}
