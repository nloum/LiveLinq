using System;
using ComposableCollections.Dictionary.Base;
using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Adapters
{
    public class ObservableDictionaryAdapter<TKey, TValue> : DelegateDictionaryBase<TKey, TValue>, Interfaces.IObservableDictionary<TKey, TValue>
    {
        private readonly Func<IDictionaryChangesStrict<TKey, TValue>> _liveLinq;
        private readonly IDisposable _disposable;

        public ObservableDictionaryAdapter(IComposableDictionary<TKey, TValue> source, IDisposable disposable, Func<IDictionaryChangesStrict<TKey, TValue>> liveLinq) : base(source)
        {
            _disposable = disposable;
            _liveLinq = liveLinq;
        }

        public ObservableDictionaryAdapter()
        {
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _liveLinq();
        }
    }
}