using System;
using ComposableCollections.Dictionary.WithBuiltInKey;
using LiveLinq.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Adapters
{
    public class ObservableQueryableReadOnlyDictionaryWithBuiltInKeyAdapter<TKey, TValue> :
        DisposableQueryableReadOnlyDictionaryWithBuiltInKeyAdapter<TKey, TValue>,
        IObservableQueryableReadOnlyDictionaryWithBuiltInKey<TKey, TValue>
    {
        private readonly IObservableQueryableReadOnlyDictionary<TKey, TValue> _source;

        public ObservableQueryableReadOnlyDictionaryWithBuiltInKeyAdapter(IObservableQueryableReadOnlyDictionary<TKey, TValue> source, Func<TValue, TKey> getKey) : base(source, getKey)
        {
            _source = source;
        }

        protected ObservableQueryableReadOnlyDictionaryWithBuiltInKeyAdapter()
        {
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _source.ToLiveLinq();
        }

        public IObservableQueryableReadOnlyDictionary<TKey, TValue> AsObservableQueryableReadOnlyDictionary()
        {
            return _source;
        }
    }
}