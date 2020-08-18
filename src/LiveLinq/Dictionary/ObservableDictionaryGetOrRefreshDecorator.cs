using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    public class ObservableDictionaryGetOrRefreshDecorator<TKey, TValue> : DictionaryGetOrRefreshDecorator<TKey, TValue>, IObservableDictionary<TKey, TValue>
    {
        private IObservableDictionary<TKey, TValue> _wrapped;

        public ObservableDictionaryGetOrRefreshDecorator(IObservableDictionary<TKey, TValue> wrapped, RefreshValue<TKey, TValue> refreshValue) : base(wrapped, refreshValue)
        {
            _wrapped = wrapped;
        }

        protected ObservableDictionaryGetOrRefreshDecorator()
        {
        }

        protected void Initialize(IObservableDictionary<TKey, TValue> wrapped, RefreshValue<TKey, TValue> refreshValue)
        {
            _wrapped = wrapped;
            base.Initialize(wrapped, refreshValue);
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _wrapped.ToLiveLinq();
        }
    }
}