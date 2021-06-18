using GuimoSoft.Cache.Utils;

namespace GuimoSoft.Cache.Delegates
{
    internal delegate bool TryGetCachedItem<TKey, TValue>(TKey key, out CacheState cacheState, out TValue value);
}
