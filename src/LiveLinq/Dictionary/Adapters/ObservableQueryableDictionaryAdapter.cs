using System;
using System.Linq;
using ComposableCollections.Dictionary.Adapters;
using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Adapters
{
    public class ObservableQueryableDictionaryAdapter<TKey, TValue> : DisposableQueryableDictionaryAdapter<TKey, TValue>,
        IObservableQueryableDictionary<TKey, TValue>
    {
        private readonly Func<IDictionaryChangesStrict<TKey, TValue>> _toLiveLinq;

        public ObservableQueryableDictionaryAdapter(IComposableDictionary<TKey, TValue> source, IDisposable disposable, IQueryable<TValue> values, Func<IDictionaryChangesStrict<TKey, TValue>> toLiveLinq) : base(source, disposable, values)
        {
            _toLiveLinq = toLiveLinq;
        }

        protected ObservableQueryableDictionaryAdapter()
        {
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _toLiveLinq();
        }
    }
}