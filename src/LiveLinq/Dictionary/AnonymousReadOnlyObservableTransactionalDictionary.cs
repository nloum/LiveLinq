using System;
using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary
{
    public class
        AnonymousReadOnlyObservableTransactionalDictionary<TKey, TValue> :
            IReadOnlyObservableTransactionalDictionary<TKey, TValue>
    {
        private Func<IDisposableReadOnlyDictionary<TKey, TValue>> _beginRead;
        private Func<IDictionaryChangesStrict<TKey, TValue>> _toLiveLinq;

        public AnonymousReadOnlyObservableTransactionalDictionary(Func<IDisposableReadOnlyDictionary<TKey, TValue>> beginRead, Func<IDictionaryChangesStrict<TKey, TValue>> toLiveLinq)
        {
            _beginRead = beginRead;
            _toLiveLinq = toLiveLinq;
        }

        public IDisposableReadOnlyDictionary<TKey, TValue> CreateReader()
        {
            return _beginRead();
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _toLiveLinq();
        }
    }
}