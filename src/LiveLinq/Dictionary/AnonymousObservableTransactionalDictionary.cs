using System;
using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    public class
        AnonymousObservableTransactionalDictionary<TKey, TValue> : IObservableTransactionalDictionary<TKey, TValue>
    {
        private readonly Func<IDictionaryChangesStrict<TKey, TValue>> _toLiveLinq;
        private readonly Func<IDisposableDictionary<TKey, TValue>> _beginWrite;
        private readonly Func<IDisposableReadOnlyDictionary<TKey, TValue>> _beginRead;

        public AnonymousObservableTransactionalDictionary(Func<IDisposableReadOnlyDictionary<TKey, TValue>> beginRead, Func<IDisposableDictionary<TKey, TValue>> beginWrite, Func<IDictionaryChangesStrict<TKey, TValue>> toLiveLinq)
        {
            _toLiveLinq = toLiveLinq;
            _beginWrite = beginWrite;
            _beginRead = beginRead;
        }

        public IDisposableReadOnlyDictionary<TKey, TValue> BeginRead()
        {
            return _beginRead();
        }

        public IDisposableDictionary<TKey, TValue> BeginWrite()
        {
            return _beginWrite();
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _toLiveLinq();
        }
    }
}