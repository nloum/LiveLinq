using System;

namespace LiveLinq.Dictionary
{
    public class
        AnonymousReadOnlyObservableDictionaryWithBuiltInKeyAdapter<TKey, TValue> : ReadOnlyObservableDictionaryWithBuiltInKeyAdapter<
            TKey, TValue>
    {
        private readonly Func<TValue, TKey> _getKey;
        
        public AnonymousReadOnlyObservableDictionaryWithBuiltInKeyAdapter(IReadOnlyObservableDictionary<TKey, TValue> wrapped, Func<TValue, TKey> getKey) : base(wrapped)
        {
            _getKey = getKey;
        }

        public override TKey GetKey(TValue value)
        {
            return _getKey(value);
        }
    }
}