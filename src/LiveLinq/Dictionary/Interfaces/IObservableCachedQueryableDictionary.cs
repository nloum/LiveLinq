using System;
using System.Collections.Generic;
using System.Linq;
using ComposableCollections.Dictionary.Adapters;
using ComposableCollections.Dictionary.Interfaces;
using ComposableCollections.Dictionary.Write;

namespace LiveLinq.Dictionary.Interfaces
{
    public interface IObservableCachedQueryableDictionary<TKey, TValue> : IObservableCachedDictionary<TKey, TValue>,
        IObservableQueryableDictionary<TKey, TValue>,
        ICachedDisposableQueryableDictionary<TKey, TValue>
    {
        
    }

    public class ObservableCachedQueryableDictionaryAdapter<TKey, TValue> :
        CachedDisposableQueryableDictionaryAdapter<TKey, TValue>, IObservableCachedQueryableDictionary<TKey, TValue>
    {
        private readonly Func<IDictionaryChangesStrict<TKey, TValue>> _toLiveLinq;

        public ObservableCachedQueryableDictionaryAdapter(ICachedQueryableDictionary<TKey, TValue> source, IDisposable disposable, Func<IDictionaryChangesStrict<TKey, TValue>> toLiveLinq) : base(source, disposable)
        {
            _toLiveLinq = toLiveLinq;
        }

        public ObservableCachedQueryableDictionaryAdapter(IComposableDictionary<TKey, TValue> source, Func<IComposableReadOnlyDictionary<TKey, TValue>> asBypassCache, Func<IComposableDictionary<TKey, TValue>> asNeverFlush, Action flushCache, Func<bool, IEnumerable<DictionaryWrite<TKey, TValue>>> getWrites, IDisposable disposable, IQueryable<TValue> values, Func<IDictionaryChangesStrict<TKey, TValue>> toLiveLinq) : base(source, asBypassCache, asNeverFlush, flushCache, getWrites, disposable, values)
        {
            _toLiveLinq = toLiveLinq;
        }

        protected ObservableCachedQueryableDictionaryAdapter()
        {
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _toLiveLinq();
        }
    }
}