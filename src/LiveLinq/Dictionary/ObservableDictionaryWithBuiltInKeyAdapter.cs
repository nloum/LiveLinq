using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    public abstract class ObservableDictionaryWithBuiltInKeyAdapter<TKey, TValue> : DictionaryWithBuiltInKeyAdapter<TKey, TValue>, IObservableDictionaryWithBuiltInKey<TKey, TValue>
    {
        private IObservableDictionary<TKey, TValue> _wrapped;

        public ObservableDictionaryWithBuiltInKeyAdapter(IObservableDictionary<TKey, TValue> wrapped) : base(wrapped)
        {
            _wrapped = wrapped;
        }

        protected ObservableDictionaryWithBuiltInKeyAdapter()
        {
        }

        public void Initialize(IObservableDictionary<TKey, TValue> wrapped)
        {
            _wrapped = wrapped;
            base.Initialize(wrapped);
        }
        
        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _wrapped.ToLiveLinq();
        }

        public virtual void Dispose()
        {
            _wrapped.Dispose();
        }
    }
}