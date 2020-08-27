using System;
using ComposableCollections.Dictionary.Adapters;
using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Adapters
{
    public class ObservableReadOnlyDictionaryAdapter<TKey, TValue> : DisposableReadOnlyDictionaryAdapter<TKey, TValue>,
        IObservableReadOnlyDictionary<TKey, TValue>
    {
        private readonly Func<IDictionaryChangesStrict<TKey, TValue>> _toLiveLinq;

        public ObservableReadOnlyDictionaryAdapter(IComposableReadOnlyDictionary<TKey, TValue> source, IDisposable disposable, Func<IDictionaryChangesStrict<TKey, TValue>> toLiveLinq) : base(source, disposable)
        {
            _toLiveLinq = toLiveLinq;
        }

        protected ObservableReadOnlyDictionaryAdapter()
        {
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _toLiveLinq();
        }
    }
}