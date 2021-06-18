using GuimoSoft.Cache.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace GuimoSoft.Cache
{
    internal class CacheConfigurations<TKey, TValue>
    {
        public TimeSpan TTL { get; init; }
        public IEqualityComparer<TKey> KeyEqualityComparer { get; init; }
        public IValueFactoryProxy<TValue> ValueFactoryProxy { get; init; }
        public ILogger Logger { get; init; }

        internal class CacheConfigurationsBuilder : ICacheConfigurationsBuilder<TKey, TValue>
        {
            internal TimeSpan? _ttl;
            internal IEqualityComparer<TKey> _keyEqualityComparer = EqualityComparer<TKey>.Default;
            internal IValueFactoryProxy<TValue> _valueFactoryProxy = DefaultValueFactoryProxy<TValue>.Instance;
            internal ILogger _logger;

            public ICacheConfigurationsBuilder<TKey, TValue> WithTTL(TimeSpan ttl)
            {
                if (ttl == default)
                    throw new ArgumentException($"O {nameof(ttl)} deve ser maior do que 0", nameof(ttl));
                _ttl = ttl;
                return this;
            }

            public ICacheConfigurationsBuilder<TKey, TValue> WithKeyEqualityComparer(IEqualityComparer<TKey> equalityComparer)
            {
                _keyEqualityComparer = equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer));
                return this;
            }

            public ICacheConfigurationsBuilder<TKey, TValue> AddLogging(ILogger logger)
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                return this;
            }

            public ICacheConfigurationsBuilder<TKey, TValue> UsingValueFactoryProxy(IValueFactoryProxy<TValue> valueFactoryProxy)
            {
                _valueFactoryProxy = valueFactoryProxy ?? throw new ArgumentNullException(nameof(valueFactoryProxy));
                return this;
            }

            internal CacheConfigurations<TKey, TValue> Build()
            {
                if (!_ttl.HasValue)
                    throw new Exception("É necessário informar o TTL do item do cache");
                return new CacheConfigurations<TKey, TValue>
                {
                    TTL = _ttl.Value,
                    KeyEqualityComparer = _keyEqualityComparer,
                    ValueFactoryProxy = _valueFactoryProxy,
                    Logger = _logger
                };
            }
        }
    }
}
