using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    public class ObservableDictionaryGetOrDefaultDecorator<TKey, TValue> : DictionaryGetOrDefaultDecorator<TKey, TValue>, IObservableDictionary<TKey, TValue>
    {
        private IObservableDictionary<TKey, TValue> _wrapped;

        public ObservableDictionaryGetOrDefaultDecorator(IObservableDictionary<TKey, TValue> wrapped, GetDefaultValue<TKey, TValue> getDefaultValue) : base(wrapped, getDefaultValue)
        {
            _wrapped = wrapped;
        }

        protected ObservableDictionaryGetOrDefaultDecorator()
        {
        }

        protected void Initialize(IObservableDictionary<TKey, TValue> wrapped, GetDefaultValue<TKey, TValue> getDefaultValue)
        {
            _wrapped = wrapped;
            base.Initialize(wrapped, getDefaultValue);
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _wrapped.ToLiveLinq();
        }
    }
}