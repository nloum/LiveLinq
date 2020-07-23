using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
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
    public abstract class ObservableDictionaryBase<TKey, TValue> : ObservableDictionaryBaseWithAbstractLiveLinq<TKey, TValue>
    {
        private readonly object _lock = new object();
        private readonly Subject<IDictionaryChangeStrict<TKey, TValue>> _subject = new Subject<IDictionaryChangeStrict<TKey, TValue>>();

        public override IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return Observable.Create<IDictionaryChangeStrict<TKey, TValue>>(observer =>
            {
                var result = _subject.Subscribe(observer);
                var items = ((IReadOnlyDictionaryEx<TKey, TValue>)this).ToImmutableList();
                if (items.Count > 0)
                {
                    observer.OnNext(Utility.DictionaryAdd(items));
                }

                return result;
            }).ToLiveLinq();
        }

        #region Methods responsible for mutation AND sending livelinq events

        public override TValue this[TKey key]
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
                lock (_lock)
                {
                    var result = AddOrUpdateInternal(key, value, out var preExistingValue);
                    if (result == AddOrUpdateResult.Update)
                    {
                        _subject.OnNext(Utility.DictionaryRemove(MoreCollections.Utility.KeyValuePair(key, preExistingValue)));
                    }
                
                    _subject.OnNext(Utility.DictionaryAdd(new KeyValuePair<TKey, TValue>(key, value)));
                }
            }
        }

        public override void Add(TKey key, TValue value)
        {
            lock (_lock)
            {
                AddInternal(key, value);
                _subject.OnNext(Utility.DictionaryAdd(MoreCollections.Utility.KeyValuePair(key, value)));
            }
        }

        public override void AddOrUpdate(TKey key, TValue value)
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
        
        public override bool Remove(TKey key, TValue value)
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
                    _subject.OnNext(Utility.DictionaryRemove(MoreCollections.Utility.KeyValuePair(key, value)));
                    return true;
                }
            }

            return false;
        }

        public override bool Remove(TKey key)
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

        public override void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
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

        public override void AddOrUpdateRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
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

        public override void RemoveRange(IEnumerable<TKey> keys)
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
    }
}