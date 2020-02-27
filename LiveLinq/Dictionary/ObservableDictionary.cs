using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Subjects;
using MoreCollections;

namespace LiveLinq.Dictionary
{
    /// <summary>
    /// This class provides a dictionary that can efficiently have LiveLinq run on it.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyObservableDictionary<TKey, TValue>, IDisposable
    {
        internal IDisposable AssociatedSubscription { get; set; } = null;
        private readonly object _lock = new object();
        private ImmutableDictionary<TKey, TValue> _dictionary = ImmutableDictionary<TKey, TValue>.Empty;
        private readonly Subject<IDictionaryChangeStrict<TKey, TValue>> _subject = new Subject<IDictionaryChangeStrict<TKey, TValue>>();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public int Count => _dictionary.Count;

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set { throw new System.NotImplementedException(); }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys.ToList();

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values.ToList();

        public IEnumerable<TKey> Keys => _dictionary.Keys;

        public IEnumerable<TValue> Values => _dictionary.Values;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var item in _dictionary)
            {
                array[arrayIndex] = item;
                arrayIndex++;
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Contains(item);
        }
        
        public bool IsReadOnly => false;

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _subject.ToLiveLinq();
        }

        public void Clear()
        {
            RemoveRange(Keys);
        }

        #region Methods responsible for mutation AND sending livelinq events
        
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            lock (_lock)
            {
                _dictionary = _dictionary.Add(item.Key, item.Value);
                _subject.OnNext(Utility.DictionaryAdd(item));
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            lock (_lock)
            {
                if (!TryGetValue(item.Key, out var value))
                {
                    if (!value.Equals(item.Value))
                    {
                        return false;
                    }

                    _dictionary = _dictionary.Remove(item.Key);
                    _subject.OnNext(Utility.DictionaryRemove(MoreCollections.Utility.KeyValuePair(item.Key, item.Value)));
                    return true;
                }
            }

            return false;
        }

        public void Add(TKey key, TValue value)
        {
            lock (_lock)
            {
                _dictionary = _dictionary.Add(key, value);
                _subject.OnNext(Utility.DictionaryAdd(MoreCollections.Utility.KeyValuePair(key, value)));
            }
        }

        public bool Remove(TKey key)
        {
            lock (_lock)
            {
                if (!TryGetValue(key, out var value))
                {
                    return false;
                }
                
                _dictionary = _dictionary.Remove(key);
                _subject.OnNext(Utility.DictionaryRemove(MoreCollections.Utility.KeyValuePair(key, value)));
                return true;
            }
        }

        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            lock (_lock)
            {
                var pairsList = pairs.ToImmutableList();
                if (pairsList.Count == 0)
                {
                    return;
                }
                _dictionary = _dictionary.AddRange(pairsList);
                _subject.OnNext(Utility.DictionaryAdd(pairsList));
            }
        }

        public void AddRange(IEnumerable<IKeyValuePair<TKey, TValue>> pairs)
        { 
            AddRange(pairs.Select(x => new KeyValuePair<TKey, TValue>(x.Key, x.Value)));
        }

        public void RemoveRange(IEnumerable<TKey> keys)
        {
            lock (_lock)
            {
                var pairs = new List<IKeyValuePair<TKey, TValue>>();
                foreach (var key in keys)
                {
                    pairs.Add(MoreCollections.Utility.KeyValuePair(key, this[key]));
                }

                if (pairs.Count == 0)
                {
                    return;
                }
                _dictionary = _dictionary.RemoveRange(pairs.Select(x => x.Key));
                _subject.OnNext(Utility.DictionaryRemove(pairs));
            }
        }
        
        #endregion

        public void Dispose()
        {
            AssociatedSubscription?.Dispose();
        }
    }
}