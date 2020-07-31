using System;

namespace LiveLinq.Dictionary
{
    public class
        AnonymousObservableDictionaryWithBuiltInKeyAdapter<TKey, TValue> : ObservableDictionaryWithBuiltInKeyAdapter<
            TKey, TValue>
    {
        private readonly Func<TValue, TKey> _getKey;
        
        public AnonymousObservableDictionaryWithBuiltInKeyAdapter(IObservableDictionary<TKey, TValue> wrapped, Func<TValue, TKey> getKey) : base(wrapped)
        {
            _getKey = getKey;
        }

        public override TKey GetKey(TValue value)
        {
            return _getKey(value);
        }
    }
}