using System;
using ComposableCollections.Dictionary.WithBuiltInKey;
using LiveLinq.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Adapters
{
    public class ObservableQueryableDictionaryWithBuiltInKeyAdapter<TKey, TValue> :
        DisposableQueryableDictionaryWithBuiltInKeyAdapter<TKey, TValue>,
        IObservableQueryableDictionaryWithBuiltInKey<TKey, TValue>
    {
        private readonly IObservableQueryableDictionary<TKey, TValue> _source;

        public ObservableQueryableDictionaryWithBuiltInKeyAdapter(IObservableQueryableDictionary<TKey, TValue> source, Func<TValue, TKey> getKey) : base(source, getKey)
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

        public IObservableQueryableDictionary<TKey, TValue> AsObservableQueryableDictionary()
        {
            return _source;
        }
    }
}