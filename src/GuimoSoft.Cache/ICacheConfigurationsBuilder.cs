using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace GuimoSoft.Cache
{
    public interface ICacheConfigurationsBuilder<TKey, TValue>
    {
        ICacheConfigurationsBuilder<TKey, TValue> WithTTL(TimeSpan ttl);
        ICacheConfigurationsBuilder<TKey, TValue> WithKeyEqualityComparer(IEqualityComparer<TKey> equalityComparer);
        ICacheConfigurationsBuilder<TKey, TValue> ShareValuesBetweenKeys(IEqualityComparer<TValue> equalityComparer);
        ICacheConfigurationsBuilder<TKey, TValue> WithCleaner(TimeSpan cleaningInterval);
        ICacheConfigurationsBuilder<TKey, TValue> AddLogging(ILogger logger);
    }
}
