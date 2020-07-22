using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using UtilityDisposables;

namespace LiveLinq.Dictionary
{
    /// <summary>
    /// This class provides a dictionary that can efficiently have LiveLinq run on it.
    /// </summary>
    /// <typeparam name="TKey">The dictionary key type</typeparam>
    /// <typeparam name="TValue">The dictionary value type</typeparam>
    /// <remarks>
    /// This class is thread-safe.
    /// </remarks>
    public class ObservableDictionary<TKey, TValue> : ObservableDictionaryBase<TKey, TValue>, IDisposable
    {
        internal DisposableCollector AssociatedSubscriptions { get; } = new DisposableCollector();
        private ImmutableDictionary<TKey, TValue> _dictionary = ImmutableDictionary<TKey, TValue>.Empty;
        protected readonly object Lock = new object();

        public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public override int Count => _dictionary.Count;

        public override bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public override bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        protected override void AddInternal(TKey key, TValue value)
        {
            _dictionary = _dictionary.Add(key, value);
        }

        protected override void RemoveInternal(TKey key)
        {
            _dictionary = _dictionary.Remove(key);
        }

        protected override void AddRangeInternal(ImmutableList<KeyValuePair<TKey, TValue>> pairs)
        {
            _dictionary = _dictionary.AddRange(pairs);
        }

        protected override AddOrUpdateResult AddOrUpdateInternal(TKey key, TValue value, out TValue preExistingValue)
        {
            lock (Lock)
            {
                if (_dictionary.TryGetValue(key, out preExistingValue))
                {
                    _dictionary = _dictionary.SetItem(key, value);
                    return AddOrUpdateResult.Update;
                }

                _dictionary = _dictionary.Add(key, value);
                return AddOrUpdateResult.Add;
            }
        }

        protected override void RemoveRangeInternal(IEnumerable<TKey> keys)
        {
            _dictionary = _dictionary.RemoveRange(keys);
        }

        public void Dispose()
        {
            AssociatedSubscriptions.Dispose();
        }
    }
}