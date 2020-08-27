using System;
using ComposableCollections.Dictionary.WithBuiltInKey;
using LiveLinq.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Adapters
{
    public class ObservableCachedDictionaryWithBuiltInKeyAdapter<TKey, TValue> :
        CachedDisposableDictionaryWithBuiltInKeyAdapter<TKey, TValue>, IObservableCachedDictionaryWithBuiltInKey<TKey, TValue>
    {
        private readonly IObservableCachedDictionary<TKey, TValue> _source;

        public ObservableCachedDictionaryWithBuiltInKeyAdapter(IObservableCachedDictionary<TKey, TValue> source, Func<TValue, TKey> getKey) : base(source, getKey)
        {
            _source = source;
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _source.ToLiveLinq();
        }

        public Interfaces.IObservableDictionary<TKey, TValue> AsObservableDictionary()
        {
            return _source;
        }

        public IObservableCachedDictionary<TKey, TValue> AsObservableCachedDictionary()
        {
            return _source;
        }
    }
}