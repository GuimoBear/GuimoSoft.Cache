using System;

namespace GuimoSoft.Cache
{
    public interface ICache<TKey, TValue> : IDisposable
    {
        CacheItemBuilder<TKey, TValue> Get(TKey key);
    }
}
