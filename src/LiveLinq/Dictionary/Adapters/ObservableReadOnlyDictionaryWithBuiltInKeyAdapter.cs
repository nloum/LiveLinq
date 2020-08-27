using System;
using ComposableCollections.Dictionary.WithBuiltInKey;
using LiveLinq.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Adapters
{
    public class ObservableReadOnlyDictionaryWithBuiltInKeyAdapter<TKey, TValue> :
        DisposableReadOnlyDictionaryWithBuiltInKeyAdapter<TKey, TValue>,
        IObservableReadOnlyDictionaryWithBuiltInKey<TKey, TValue>
    {
        private readonly IObservableReadOnlyDictionary<TKey, TValue> _source;

        public ObservableReadOnlyDictionaryWithBuiltInKeyAdapter(IObservableReadOnlyDictionary<TKey, TValue> source, Func<TValue, TKey> getKey) : base(source, getKey)
        {
            _source = source;
        }

        protected ObservableReadOnlyDictionaryWithBuiltInKeyAdapter()
        {
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _source.ToLiveLinq();
        }
    }
}