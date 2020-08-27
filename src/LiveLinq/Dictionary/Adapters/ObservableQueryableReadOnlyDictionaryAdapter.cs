using System;
using System.Linq;
using ComposableCollections.Dictionary.Adapters;
using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Adapters
{
    public class ObservableQueryableReadOnlyDictionaryAdapter<TKey, TValue> :
        DisposableQueryableReadOnlyDictionaryAdapter<TKey, TValue>, IObservableQueryableReadOnlyDictionary<TKey, TValue>
    {

        private readonly Func<IDictionaryChangesStrict<TKey, TValue>> _toLiveLinq;
        public ObservableQueryableReadOnlyDictionaryAdapter(IQueryableReadOnlyDictionary<TKey, TValue> source, IDisposable disposable, Func<IDictionaryChangesStrict<TKey, TValue>> toLiveLinq) : base(source, disposable)
        {
            _toLiveLinq = toLiveLinq;
        }

        public ObservableQueryableReadOnlyDictionaryAdapter(IComposableReadOnlyDictionary<TKey, TValue> source, IDisposable disposable, IQueryable<TValue> values, Func<IDictionaryChangesStrict<TKey, TValue>> toLiveLinq) : base(source, disposable, values)
        {
            _toLiveLinq = toLiveLinq;
        }

        protected ObservableQueryableReadOnlyDictionaryAdapter()
        {
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _toLiveLinq();
        }
    }
}