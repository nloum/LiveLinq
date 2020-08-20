using System;
using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    public class
        AnonymousObservableTransactionalDictionaryWithBuiltInKey<TKey, TValue> :
            IObservableTransactionalDictionaryWithBuiltInKey<TKey, TValue>
    {
        private Func<IDisposableReadOnlyDictionaryWithBuiltInKey<TKey, TValue>> _beginRead;
        private Func<IDisposableDictionaryWithBuiltInKey<TKey, TValue>> _beginWrite;
        private Func<IDictionaryChangesStrict<TKey, TValue>> _toLiveLinq;

        public AnonymousObservableTransactionalDictionaryWithBuiltInKey(Func<IDisposableReadOnlyDictionaryWithBuiltInKey<TKey, TValue>> beginRead, Func<IDisposableDictionaryWithBuiltInKey<TKey, TValue>> beginWrite, Func<IDictionaryChangesStrict<TKey, TValue>> toLiveLinq)
        {
            _beginRead = beginRead;
            _beginWrite = beginWrite;
            _toLiveLinq = toLiveLinq;
        }

        public IDisposableReadOnlyDictionaryWithBuiltInKey<TKey, TValue> BeginRead()
        {
            return _beginRead();
        }

        public IDisposableDictionaryWithBuiltInKey<TKey, TValue> BeginWrite()
        {
            return _beginWrite();
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _toLiveLinq();
        }
    }
}