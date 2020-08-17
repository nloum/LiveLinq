using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    public class DelegateObservableDictionaryWithBuiltInKey<TKey, TValue> : DelegateDictionaryWithBuiltInKey<TKey, TValue>, IObservableDictionaryWithBuiltInKey<TKey, TValue>
    {
        private IObservableDictionaryWithBuiltInKey<TKey, TValue> _wrapped;

        public DelegateObservableDictionaryWithBuiltInKey(IObservableDictionaryWithBuiltInKey<TKey, TValue> wrapped) : base(wrapped)
        {
            _wrapped = wrapped;
        }

        protected DelegateObservableDictionaryWithBuiltInKey()
        {
        }
        
        protected void Initialize(IObservableDictionaryWithBuiltInKey<TKey, TValue> wrapped)
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