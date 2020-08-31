using System;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.WithBuiltInKey.Interfaces;

namespace LiveLinq.Dictionary
{
    public class
        AnonymousReadOnlyObservableTransactionalDictionaryWithBuiltInKey<TKey, TValue> :
            IReadOnlyObservableTransactionalDictionaryWithBuiltInKey<TKey, TValue>
    {
        private Func<IDisposableReadOnlyDictionaryWithBuiltInKey<TKey, TValue>> _beginRead;
        private Func<IDictionaryChangesStrict<TKey, TValue>> _toLiveLinq;

        public AnonymousReadOnlyObservableTransactionalDictionaryWithBuiltInKey(Func<IDisposableReadOnlyDictionaryWithBuiltInKey<TKey, TValue>> beginRead, Func<IDictionaryChangesStrict<TKey, TValue>> toLiveLinq)
        {
            _beginRead = beginRead;
            _toLiveLinq = toLiveLinq;
        }

        public IDisposableReadOnlyDictionaryWithBuiltInKey<TKey, TValue> CreateReader()
        {
            return _beginRead();
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _toLiveLinq();
        }
    }
}