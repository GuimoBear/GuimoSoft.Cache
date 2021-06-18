using GuimoSoft.Cache.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GuimoSoft.Cache.InMemory
{
    public partial class Cache<TKey, TValue>
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private void Add(TKey key, TValue value, DateTime ttl)
        {
            using var locker = new DisposableLock(_lock);
            _cache.TryAdd(key, new (GetExistingOrUseCreated(value), ttl));
        }

        private void Update(TKey key, TValue value, DateTime ttl)
        {
            using var locker = new DisposableLock(_lock);
            _cache[key] = new (GetExistingOrUseCreated(value), ttl);
        }

        private TValue GetExistingOrUseCreated(TValue newValue)
        {
            if (_configs.ShareValuesBetweenKeys)
            {
                var existingValue = _cache.Values.FirstOrDefault(i => _configs.ValueEqualityComparer.Equals(newValue, i.Value));
                if (existingValue is not null)
                    return existingValue;
            }
            return newValue;
        }
    }
}
