using GuimoSoft.Cache.Utils;

namespace GuimoSoft.Cache.Delegates
{
    internal delegate void ValueCreated<TKey, TValue>(TKey key, CacheState cacheState, TValue value);
}
