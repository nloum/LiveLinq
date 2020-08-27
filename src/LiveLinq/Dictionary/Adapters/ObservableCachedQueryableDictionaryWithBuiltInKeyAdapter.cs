using System;
using ComposableCollections.Dictionary.WithBuiltInKey;
using LiveLinq.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Adapters
{
    public class ObservableCachedQueryableDictionaryWithBuiltInKeyAdapter<TKey, TValue> :
        CachedDisposableQueryableDictionaryWithBuiltInKeyAdapter<TKey, TValue>,
        IObservableCachedQueryableDictionaryWithBuiltInKey<TKey, TValue>
    {
        private readonly IObservableCachedQueryableDictionary<TKey, TValue> _source;

        public ObservableCachedQueryableDictionaryWithBuiltInKeyAdapter(IObservableCachedQueryableDictionary<TKey, TValue> source, Func<TValue, TKey> getKey) : base(source, getKey)
        {
            _source = source;
        }

        protected ObservableCachedQueryableDictionaryWithBuiltInKeyAdapter()
        {
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

        public IObservableQueryableDictionary<TKey, TValue> AsObservableQueryableDictionary()
        {
            return _source;
        }

        public IObservableCachedQueryableDictionary<TKey, TValue> AsObservableCachedQueryableDictionary()
        {
            return _source;
        }
    }
}