using MoreCollections;

namespace LiveLinq.Dictionary
{
    /// <summary>
    /// An observable dictionary that allows you to add items when they are requested.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <remarks>
    /// This class is thread-safe.
    /// </remarks>
    public class ObservableDictionaryGetOrDefault<TKey, TValue> : ObservableDictionary<TKey, TValue>
    {
        private readonly GetDefaultValue<TKey, TValue> _getDefaultValue;

        public ObservableDictionaryGetOrDefault(GetDefaultValue<TKey, TValue> getDefaultValue)
        {
            _getDefaultValue = getDefaultValue;
        }

        public override bool TryGetValue(TKey key, out TValue value)
        {
            lock (Lock)
            {
                if (!base.TryGetValue(key, out value))
                {
                    _getDefaultValue(key, out var maybeValue, out var persist);

                    if (maybeValue.HasValue)
                    {
                        if (persist)
                        {
                            AddInternal(key, maybeValue.Value);
                        }
                    
                        value = maybeValue.Value;
                        return true;
                    }

                    value = default;
                    return false;
                }
            }

            return true;
        }
    }
}