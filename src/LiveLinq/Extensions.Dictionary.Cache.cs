using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;

namespace LiveLinq
{
    public static partial class Extensions
    {
        public static IDictionaryChangesStrict<TKey, TValue> Cache<TKey, TValue>(
            this IDictionaryChanges<TKey, TValue> source, IObservableDictionary<TKey, TValue> cache)
        {
            var disposable = source.Subscribe((key, value) =>
            {
                if (!cache.ContainsKey(key))
                {
                    cache.Add(key, value);
                }
            }, (key, value) =>
            {
                if (cache.ContainsKey(key))
                {
                    cache.Remove(key);
                }
            });
            return cache.ToLiveLinq().AsObservable().OnUnsubscribe(_ => disposable.Dispose()).ToLiveLinq();
        }
    }
}