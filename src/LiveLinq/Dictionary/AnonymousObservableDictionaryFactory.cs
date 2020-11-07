using System;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary
{
    public class
        AnonymousObservableDictionaryFactory<TKey, TValue> : IObservableDictionaryFactory<TKey, TValue>
    {
        private readonly Func<IDictionaryChangesStrict<TKey, TValue>> _toLiveLinq;
        private readonly Func<IDisposableDictionary<TKey, TValue>> _beginWrite;
        private readonly Func<IDisposableReadOnlyDictionary<TKey, TValue>> _beginRead;

        public AnonymousObservableDictionaryFactory(Func<IDisposableReadOnlyDictionary<TKey, TValue>> beginRead, Func<IDisposableDictionary<TKey, TValue>> beginWrite, Func<IDictionaryChangesStrict<TKey, TValue>> toLiveLinq)
        {
            _toLiveLinq = toLiveLinq;
            _beginWrite = beginWrite;
            _beginRead = beginRead;
        }

        public IDisposableReadOnlyDictionary<TKey, TValue> CreateReader()
        {
            return _beginRead();
        }

        public IDisposableDictionary<TKey, TValue> CreateWriter()
        {
            return _beginWrite();
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _toLiveLinq();
        }
    }
}