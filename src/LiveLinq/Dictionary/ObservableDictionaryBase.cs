using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using MoreCollections;

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
    public abstract class ObservableDictionaryBase<TKey, TValue> : IObservableDictionary<TKey, TValue>
    {
        private readonly object _lock = new object();
        private readonly Subject<IDictionaryChangeStrict<TKey, TValue>> _subject = new Subject<IDictionaryChangeStrict<TKey, TValue>>();

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys.ToList();

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values.ToList();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool IsReadOnly => false;

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            if (Count > 0)
            {
                return Observable.Return(Utility.DictionaryAdd(this)).Concat(_subject).ToLiveLinq();
            }
            
            return _subject.ToLiveLinq();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var item in this)
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

        #region Stuff that might need to be overridden for performance reasons
        
        public virtual void Clear()
        {
            RemoveRange(Keys);
        }

        public virtual IEnumerable<TKey> Keys => this.Select(x => x.Key);
        public virtual IEnumerable<TValue> Values => this.Select(x => x.Value);

        #endregion
        
        #region Abstract methods

        public abstract IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();
        public abstract int Count { get; }
        public abstract bool ContainsKey(TKey key);
        public abstract bool TryGetValue(TKey key, out TValue value);
        protected abstract void AddInternal(TKey key, TValue value);
        protected abstract void RemoveInternal(TKey key);
        protected abstract void AddRangeInternal(ImmutableList<KeyValuePair<TKey, TValue>> pairs);
        protected abstract AddOrUpdateResult AddOrUpdateInternal(TKey key, TValue value, out TValue preExistingValue);
        protected abstract void RemoveRangeInternal(IEnumerable<TKey> keys);

        #endregion
        
        #region Methods responsible for mutation AND sending livelinq events

        public TValue this[TKey key]
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
                if (result == AddOrUpdateResult.Update)
                {
                    _subject.OnNext(Utility.DictionaryRemove(MoreCollections.Utility.KeyValuePair(key, preExistingValue)));
                }
                
                _subject.OnNext(Utility.DictionaryAdd(new KeyValuePair<TKey, TValue>(key, value)));
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(IKeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(TKey key, TValue value)
        {
            lock (_lock)
            {
                AddInternal(key, value);
                _subject.OnNext(Utility.DictionaryAdd(MoreCollections.Utility.KeyValuePair(key, value)));
            }
        }

        public void AddOrUpdate(KeyValuePair<TKey, TValue> item)
        {
            AddOrUpdate(item.Key, item.Value);
        }

        public void AddOrUpdate(IKeyValuePair<TKey, TValue> item)
        {
            AddOrUpdate(item.Key, item.Value);
        }

        public void AddOrUpdate(TKey key, TValue value)
        {
            lock (_lock)
            {

                if (TryGetValue(key, out var preExistingValue))
                {
                    RemoveInternal(key);
                    _subject.OnNext(Utility.DictionaryRemove(new []{new KeyValuePair<TKey, TValue>(key, preExistingValue) }));
                }

                AddInternal(key, value);
                _subject.OnNext(Utility.DictionaryAdd(new KeyValuePair<TKey, TValue>(key, value)));
            }
        }
        
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key, item.Value);
        }
        
        public bool Remove(IKeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key, item.Value);
        }

        public bool Remove(TKey key, TValue value)
        {
            lock (_lock)
            {
                if (!TryGetValue(key, out var existingValue))
                {
                    if (!existingValue.Equals(value))
                    {
                        return false;
                    }

                    RemoveInternal(key);
                    _subject.OnNext(Utility.DictionaryRemove(MoreCollections.Utility.KeyValuePair(key, value)));
                    return true;
                }
            }

            return false;
        }

        public bool Remove(TKey key)
        {
            lock (_lock)
            {
                if (!TryGetValue(key, out var value))
                {
                    return false;
                }
                
                RemoveInternal(key);
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
                AddRangeInternal(pairsList);
                _subject.OnNext(Utility.DictionaryAdd(pairsList));
            }
        }

        public void AddRange(IEnumerable<IKeyValuePair<TKey, TValue>> pairs)
        { 
            AddRange(pairs.Select(x => new KeyValuePair<TKey, TValue>(x.Key, x.Value)));
        }

        public void AddOrUpdateRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            lock (_lock)
            {
                foreach (var item in pairs)
                {
                    if (AddOrUpdateInternal(item.Key, item.Value, out var preExistingValue) == AddOrUpdateResult.Update)
                    {
                        _subject.OnNext(Utility.DictionaryRemove(new []{new KeyValuePair<TKey, TValue>(item.Key, preExistingValue) }));
                    }
                    
                    _subject.OnNext(Utility.DictionaryAdd(item));
                }
            }
        }

        public void AddOrUpdateRange(IEnumerable<IKeyValuePair<TKey, TValue>> pairs)
        { 
            AddOrUpdateRange(pairs.Select(x => new KeyValuePair<TKey, TValue>(x.Key, x.Value)));
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
                RemoveRangeInternal(pairs.Select(x => x.Key));
                _subject.OnNext(Utility.DictionaryRemove(pairs));
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