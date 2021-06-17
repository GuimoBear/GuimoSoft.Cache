using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace GuimoSoft.Cache
{
    internal class CacheConfigurations<TKey, TValue>
    {
        public TimeSpan TTL { get; init; }
        public bool ShareValuesBetweenKeys { get; init; }
        public IEqualityComparer<TKey> KeyEqualityComparer { get; init; }
        public IEqualityComparer<TValue> ValueEqualityComparer { get; init; }
        public bool UseCleaner { get; init; }
        public TimeSpan CleaningInterval { get; init; }
        public TimeSpan DelayToNextCancellationRequestedCheck { get; } = TimeSpan.FromSeconds(2);
        public ILogger Logger { get; init; }

        internal class CacheConfigurationsBuilder : ICacheConfigurationsBuilder<TKey, TValue>
        {
            private TimeSpan? _ttl;
            private bool _shareValuesBetweenKeys = false;
            private IEqualityComparer<TKey> _keyEqualityComparer = EqualityComparer<TKey>.Default;
            private IEqualityComparer<TValue> _valueEqualityComparer = EqualityComparer<TValue>.Default;
            private bool _useCleaner = false;
            private TimeSpan _cleaningInterval = TimeSpan.FromMinutes(5);
            private ILogger _logger;

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

            public ICacheConfigurationsBuilder<TKey, TValue> ShareValuesBetweenKeys(IEqualityComparer<TValue> equalityComparer)
            {
                _valueEqualityComparer = equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer));
                _shareValuesBetweenKeys = true;
                return this;
            }

            public ICacheConfigurationsBuilder<TKey, TValue> WithCleaner(TimeSpan cleaningInterval)
            {
                if (cleaningInterval == default)
                    throw new ArgumentException("É necessário informar o intervalo entre as limpezas do cache", nameof(cleaningInterval));
                _useCleaner = true;
                _cleaningInterval = cleaningInterval;
                return this;
            }

            public ICacheConfigurationsBuilder<TKey, TValue> AddLogging(ILogger logger)
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                    ShareValuesBetweenKeys = _shareValuesBetweenKeys,
                    ValueEqualityComparer = _valueEqualityComparer,
                    UseCleaner = _useCleaner,
                    CleaningInterval = _cleaningInterval, 
                    Logger = _logger
                };
            }
        }
    }
}
