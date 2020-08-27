using System;
using System.Collections.Generic;
using ComposableCollections.Dictionary.Adapters;
using ComposableCollections.Dictionary.Interfaces;
using ComposableCollections.Dictionary.Write;
using LiveLinq.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Adapters
{
    public class ObservableCachedDictionaryAdapter<TKey, TValue> : CachedDisposableDictionaryAdapter<TKey, TValue>, IObservableCachedDictionary<TKey, TValue>
    {
        private readonly Func<IDictionaryChangesStrict<TKey, TValue>> _toLiveLinq;

        public ObservableCachedDictionaryAdapter(ICachedDictionary<TKey, TValue> source, IDisposable disposable, Func<IDictionaryChangesStrict<TKey, TValue>> toLiveLinq) : base(source, disposable)
        {
            _toLiveLinq = toLiveLinq;
        }

        public ObservableCachedDictionaryAdapter(IComposableDictionary<TKey, TValue> source, Func<IComposableReadOnlyDictionary<TKey, TValue>> asBypassCache, Func<IComposableDictionary<TKey, TValue>> asNeverFlush, Action flushCache, Func<bool, IEnumerable<DictionaryWrite<TKey, TValue>>> getWrites, IDisposable disposable, Func<IDictionaryChangesStrict<TKey, TValue>> toLiveLinq) : base(source, asBypassCache, asNeverFlush, flushCache, getWrites, disposable)
        {
            _toLiveLinq = toLiveLinq;
        }

        protected ObservableCachedDictionaryAdapter()
        {
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _toLiveLinq();
        }
    }
}