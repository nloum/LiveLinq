using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MoreCollections;
using SimpleMonads;

namespace LiveLinq.Dictionary
{
    /// <summary>
    /// This class provides a dictionary-like API that can efficiently have LiveLinq run on it.
    /// This class is useful for creating ObservableDictionary classes that aren't backed by a normal dictionary;
    /// e.g., this is useful for creating an ObservableDictionary that doesn't keep all its items in memory, and instead
    /// stores them in a database.
    /// </summary>
    /// <typeparam name="TKey">The dictionary key type</typeparam>
    /// <typeparam name="TValue">The dictionary value type</typeparam>
    public abstract class ObservableDictionaryBaseWithAbstractLiveLinq<TKey, TValue> : IObservableDictionary<TKey, TValue>
    {
        private readonly object _lock = new object();

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys.ToList();

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values.ToList();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetKeyValuePairEnumeratorInternal();
        }

        public bool IsReadOnly => false;

        public abstract IDictionaryChangesStrict<TKey, TValue> ToLiveLinq();

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var item in ((IDictionary<TKey, TValue>)this))
            {
                array[arrayIndex] = item;
                arrayIndex++;
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (!TryGetValue(item.Key, out var value))
            {
                return false;
            }

            var result = value.Equals(item.Value);
            return result;
        }

        public IMaybe<TValue> TryGetValue(TKey key)
        {
            if (TryGetValue(key, out var value))
            {
                return value.ToMaybe();
            }

            return Maybe<TValue>.Nothing();
        }
        
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return GetKeyValuePairEnumeratorInternal();
        }

        IEnumerator<IKeyValuePair<TKey, TValue>> IEnumerable<IKeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return GetIKeyValuePairEnumeratorInternal();
        }
        
        #region Stuff that might need to be overridden for performance/atomicity reasons
        
        public virtual void Clear()
        {
            RemoveRange(Keys);
        }

        public virtual IEnumerable<TKey> Keys => ((IDictionary<TKey, TValue>)this).Select(x => x.Key);
        public virtual IEnumerable<TValue> Values => ((IDictionary<TKey, TValue>)this).Select(x => x.Value);

        protected virtual void RemoveRangeInternal(IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
            {
                RemoveInternal(key);
            }
        }

        protected virtual void AddRangeInternal(ImmutableList<KeyValuePair<TKey, TValue>> pairs)
        {
            foreach (var pair in pairs)
            {
                AddInternal(pair.Key, pair.Value);
            }
        }

        protected virtual AddOrUpdateResult AddOrUpdateInternal(TKey key, TValue value, out TValue preExistingValue)
        {
            if (TryGetValue(key, out preExistingValue))
            {
                RemoveInternal(key);
                AddInternal(key, value);
                return AddOrUpdateResult.Update;
            }
            else
            {
                AddInternal(key, value);
                return AddOrUpdateResult.Add;
            }
        }

        public virtual bool ContainsKey(TKey key)
        {
            return TryGetValue(key, out var preExistingValue);
        }

        #endregion
        
        #region Abstract methods

        protected abstract IEnumerator<KeyValuePair<TKey, TValue>> GetKeyValuePairEnumeratorInternal();
        protected abstract IEnumerator<IKeyValuePair<TKey, TValue>> GetIKeyValuePairEnumeratorInternal();
        public abstract int Count { get; }
        public abstract bool TryGetValue(TKey key, out TValue value);
        protected abstract void AddInternal(TKey key, TValue value);
        protected abstract void RemoveInternal(TKey key);

        #endregion
        
        #region Methods responsible for mutation, but not sending out livelinq events (because these methods call other methods that do)
        
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(IKeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void AddOrUpdate(KeyValuePair<TKey, TValue> item)
        {
            AddOrUpdate(item.Key, item.Value);
        }

        public void AddOrUpdate(IKeyValuePair<TKey, TValue> item)
        {
            AddOrUpdate(item.Key, item.Value);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key, item.Value);
        }
        
        public bool Remove(IKeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key, item.Value);
        }

        public void AddRange(IEnumerable<IKeyValuePair<TKey, TValue>> pairs)
        { 
            AddRange(pairs.Select(x => new KeyValuePair<TKey, TValue>(x.Key, x.Value)));
        }

        public void AddOrUpdateRange(IEnumerable<IKeyValuePair<TKey, TValue>> pairs)
        { 
            AddOrUpdateRange(pairs.Select(x => new KeyValuePair<TKey, TValue>(x.Key, x.Value)));
        }

        #endregion
        
        #region Methods responsible for mutation AND sending livelinq events

        public virtual TValue this[TKey key]
        {
            get
            {
                if (!TryGetValue(key, out var result))
                {
                    throw new KeyNotFoundException();
                }

                return result;
            }
            set
            {
                var result = AddOrUpdateInternal(key, value, out var preExistingValue);
            }
        }

        public virtual void Add(TKey key, TValue value)
        {
            lock (_lock)
            {
                AddInternal(key, value);
            }
        }

        public virtual void AddOrUpdate(TKey key, TValue value)
        {
            lock (_lock)
            {

                if (TryGetValue(key, out var preExistingValue))
                {
                    RemoveInternal(key);
                }

                AddInternal(key, value);
            }
        }
        
        public virtual bool Remove(TKey key, TValue value)
        {
            lock (_lock)
            {
                if (TryGetValue(key, out var existingValue))
                {
                    if (!existingValue.Equals(value))
                    {
                        return false;
                    }

                    RemoveInternal(key);
                    return true;
                }
            }

            return false;
        }

        public virtual bool Remove(TKey key)
        {
            lock (_lock)
            {
                if (!TryGetValue(key, out var value))
                {
                    return false;
                }
                
                RemoveInternal(key);
                return true;
            }
        }

        public virtual void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            lock (_lock)
            {
                var pairsList = pairs.ToImmutableList();
                if (pairsList.Count == 0)
                {
                    return;
                }
                AddRangeInternal(pairsList);
            }
        }

        public virtual void AddOrUpdateRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            lock (_lock)
            {
                foreach (var item in pairs)
                {
                    AddOrUpdateInternal(item.Key, item.Value, out var preExistingValue);
                }
            }
        }

        public virtual void RemoveRange(IEnumerable<TKey> keys)
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
                RemoveRangeInternal(pairs.Select(x => x.Key));
            }
        }
        
        #endregion

        protected enum AddOrUpdateResult
        {
            Add,
            Update
        }
    }
}