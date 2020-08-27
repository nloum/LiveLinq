using System;
using ComposableCollections.Dictionary.WithBuiltInKey;
using LiveLinq.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Adapters
{
    public class ObservableDictionaryWithBuiltInKeyAdapter<TKey, TValue> : DisposableDictionaryWithBuiltInKeyAdapter<TKey, TValue>, Interfaces.IObservableDictionaryWithBuiltInKey<TKey, TValue>
    {
        private readonly IObservableDictionary<TKey, TValue> _source;
        private Func<IDictionaryChangesStrict<TKey, TValue>> _toLiveLinq;
        
        public ObservableDictionaryWithBuiltInKeyAdapter(IObservableDictionary<TKey, TValue> source, Func<TValue, TKey> getKey, Func<IDictionaryChangesStrict<TKey, TValue>> toLiveLinq) : base(source, getKey)
        {
            _source = source;
            _toLiveLinq = toLiveLinq;
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _toLiveLinq();
        }

        public Interfaces.IObservableDictionary<TKey, TValue> AsObservableDictionary()
        {
            return _source;
        }
    }
}