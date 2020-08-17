using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    public abstract class ReadOnlyObservableDictionaryWithBuiltInKeyAdapter<TKey, TValue> : ReadOnlyDictionaryWithBuiltInKeyAdapter<TKey, TValue>, IReadOnlyObservableDictionaryWithBuiltInKey<TKey, TValue>
    {
        private IReadOnlyObservableDictionary<TKey, TValue> _wrapped;

        public ReadOnlyObservableDictionaryWithBuiltInKeyAdapter(IReadOnlyObservableDictionary<TKey, TValue> wrapped) : base(wrapped)
        {
            _wrapped = wrapped;
        }

        protected ReadOnlyObservableDictionaryWithBuiltInKeyAdapter()
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
    }
}