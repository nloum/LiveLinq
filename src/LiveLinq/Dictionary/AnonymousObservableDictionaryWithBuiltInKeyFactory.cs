using System;
using ComposableCollections.DictionaryWithBuiltInKey.Interfaces;

namespace LiveLinq.Dictionary
{
    public class
        AnonymousObservableDictionaryWithBuiltInKeyFactory<TKey, TValue> :
            IObservableDictionaryWithBuiltInKeyFactory<TKey, TValue>
    {
        private Func<IDisposableReadOnlyDictionaryWithBuiltInKey<TKey, TValue>> _beginRead;
        private Func<IDisposableDictionaryWithBuiltInKey<TKey, TValue>> _beginWrite;
        private Func<IDictionaryChangesStrict<TKey, TValue>> _toLiveLinq;

        public AnonymousObservableDictionaryWithBuiltInKeyFactory(Func<IDisposableReadOnlyDictionaryWithBuiltInKey<TKey, TValue>> beginRead, Func<IDisposableDictionaryWithBuiltInKey<TKey, TValue>> beginWrite, Func<IDictionaryChangesStrict<TKey, TValue>> toLiveLinq)
        {
            _beginRead = beginRead;
            _beginWrite = beginWrite;
            _toLiveLinq = toLiveLinq;
        }

        public IDisposableReadOnlyDictionaryWithBuiltInKey<TKey, TValue> CreateReader()
        {
            return _beginRead();
        }

        public IDisposableDictionaryWithBuiltInKey<TKey, TValue> CreateWriter()
        {
            return _beginWrite();
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _toLiveLinq();
        }
    }
}