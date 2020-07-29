using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using MoreCollections;
using SimpleMonads;

namespace LiveLinq.Dictionary
{
    public class AggregateObservableDictionary<TKey, TValue> : IObservableDictionary<TKey, TValue>
    {
        private readonly IObservableDictionary<TKey, TValue> _mutationsGoHere;
        private readonly ImmutableList<IReadOnlyObservableDictionary<TKey, TValue>> _wrapped;

        public AggregateObservableDictionary(IObservableDictionary<TKey, TValue> mutationsGoHere, IEnumerable<IReadOnlyObservableDictionary<TKey, TValue>> wrapped)
        {
            _mutationsGoHere = mutationsGoHere;
            _wrapped = wrapped.Concat(new IReadOnlyObservableDictionary<TKey, TValue>[]{mutationsGoHere}).ToImmutableList();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IKeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _wrapped.SelectMany(x => x).GetEnumerator();
        }

        public int Count => _wrapped.Select(x => x.Count).Sum();

        public bool ContainsKey(TKey key)
        {
            return _wrapped.Any(x => x.ContainsKey(key));
        }

        public IMaybe<TValue> TryGetValue(TKey key)
        {
            foreach (var wrapped in _wrapped)
            {
                var result = wrapped.TryGetValue(key);
                if (result.HasValue)
                {
                    return result;
                }
            }
            
            return Maybe<TValue>.Nothing();
        }

        public IEqualityComparer<TKey> Comparer => _mutationsGoHere.Comparer;

        public IEnumerable<TKey> Keys => _wrapped.SelectMany(x => x.Keys);

        public IEnumerable<TValue> Values => _wrapped.SelectMany(x => x.Values);

        public bool TryGetValue(TKey key, out TValue value)
        {
            var result = TryGetValue(key);
            if (result.HasValue)
            {
                value = result.Value;
                return true;
            }

            value = default;
            return false;
        }

        public void Mutate(IEnumerable<DictionaryMutation<TKey, TValue>> mutations, out IReadOnlyList<DictionaryMutationResult<TKey, TValue>> results)
        {
            _mutationsGoHere.Mutate(mutations, out results);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            return _mutationsGoHere.TryAdd(key, value);
        }

        public bool TryAdd(TKey key, Func<TValue> value)
        {
            return _mutationsGoHere.TryAdd(key, value);
        }

        public bool TryAdd(TKey key, Func<TValue> value, out TValue existingValue, out TValue newValue)
        {
            return _mutationsGoHere.TryAdd(key, value, out existingValue, out newValue);
        }

        public void TryAddRange(IEnumerable<IKeyValuePair<TKey, TValue>> newItems, out IReadOnlyDictionaryEx<TKey, IDictionaryItemAddAttempt<TValue>> result)
        {
            _mutationsGoHere.TryAddRange(newItems, out result);
        }

        public void TryAddRange(IEnumerable<KeyValuePair<TKey, TValue>> newItems, out IReadOnlyDictionaryEx<TKey, IDictionaryItemAddAttempt<TValue>> result)
        {
            _mutationsGoHere.TryAddRange(newItems, out result);
        }

        public void TryAddRange<TKeyValuePair>(IEnumerable<TKeyValuePair> newItems, Func<TKeyValuePair, TKey> key, Func<TKeyValuePair, TValue> value, out IReadOnlyDictionaryEx<TKey, IDictionaryItemAddAttempt<TValue>> result)
        {
            _mutationsGoHere.TryAddRange(newItems, key, value, out result);
        }

        public void TryAddRange(IEnumerable<IKeyValuePair<TKey, TValue>> newItems)
        {
            _mutationsGoHere.TryAddRange(newItems);
        }

        public void TryAddRange(IEnumerable<KeyValuePair<TKey, TValue>> newItems)
        {
            _mutationsGoHere.TryAddRange(newItems);
        }

        public void TryAddRange<TKeyValuePair>(IEnumerable<TKeyValuePair> newItems, Func<TKeyValuePair, TKey> key, Func<TKeyValuePair, TValue> value)
        {
            _mutationsGoHere.TryAddRange(newItems, key, value);
        }

        public void TryAddRange(params IKeyValuePair<TKey, TValue>[] newItems)
        {
            _mutationsGoHere.TryAddRange(newItems);
        }

        public void TryAddRange(params KeyValuePair<TKey, TValue>[] newItems)
        {
            _mutationsGoHere.TryAddRange(newItems);
        }

        public void Add(TKey key, TValue value)
        {
            _mutationsGoHere.Add(key, value);
        }

        public void AddRange(IEnumerable<IKeyValuePair<TKey, TValue>> newItems)
        {
            _mutationsGoHere.AddRange(newItems);
        }

        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> newItems)
        {
            _mutationsGoHere.AddRange(newItems);
        }

        public void AddRange<TKeyValuePair>(IEnumerable<TKeyValuePair> newItems, Func<TKeyValuePair, TKey> key, Func<TKeyValuePair, TValue> value)
        {
            _mutationsGoHere.AddRange(newItems, key, value);
        }

        public void AddRange(params IKeyValuePair<TKey, TValue>[] newItems)
        {
            _mutationsGoHere.AddRange(newItems);
        }

        public void AddRange(params KeyValuePair<TKey, TValue>[] newItems)
        {
            _mutationsGoHere.AddRange(newItems);
        }

        public bool TryUpdate(TKey key, TValue value)
        {
            return _mutationsGoHere.TryUpdate(key, value);
        }

        public bool TryUpdate(TKey key, TValue value, out TValue previousValue)
        {
            return _mutationsGoHere.TryUpdate(key, value, out previousValue);
        }

        public bool TryUpdate(TKey key, Func<TValue, TValue> value, out TValue previousValue, out TValue newValue)
        {
            return _mutationsGoHere.TryUpdate(key, value, out previousValue, out newValue);
        }

        public void TryUpdateRange(IEnumerable<IKeyValuePair<TKey, TValue>> newItems)
        {
            _mutationsGoHere.TryUpdateRange(newItems);
        }

        public void TryUpdateRange(IEnumerable<KeyValuePair<TKey, TValue>> newItems)
        {
            _mutationsGoHere.TryUpdateRange(newItems);
        }

        public void TryUpdateRange<TKeyValuePair>(IEnumerable<TKeyValuePair> newItems, Func<TKeyValuePair, TKey> key, Func<TKeyValuePair, TValue> value)
        {
            _mutationsGoHere.TryUpdateRange(newItems, key, value);
        }

        public void TryUpdateRange(params IKeyValuePair<TKey, TValue>[] newItems)
        {
            _mutationsGoHere.TryUpdateRange(newItems);
        }

        public void TryUpdateRange(params KeyValuePair<TKey, TValue>[] newItems)
        {
            _mutationsGoHere.TryUpdateRange(newItems);
        }

        public void TryUpdateRange(IEnumerable<IKeyValuePair<TKey, TValue>> newItems, out IReadOnlyDictionaryEx<TKey, IDictionaryItemUpdateAttempt<TValue>> result)
        {
            _mutationsGoHere.TryUpdateRange(newItems, out result);
        }

        public void TryUpdateRange(IEnumerable<KeyValuePair<TKey, TValue>> newItems, out IReadOnlyDictionaryEx<TKey, IDictionaryItemUpdateAttempt<TValue>> result)
        {
            _mutationsGoHere.TryUpdateRange(newItems, out result);
        }

        public void TryUpdateRange<TKeyValuePair>(IEnumerable<TKeyValuePair> newItems, Func<TKeyValuePair, TKey> key, Func<TKeyValuePair, TValue> value, out IReadOnlyDictionaryEx<TKey, IDictionaryItemUpdateAttempt<TValue>> result)
        {
            _mutationsGoHere.TryUpdateRange(newItems, key, value, out result);
        }

        public void Update(TKey key, TValue value)
        {
            _mutationsGoHere.Update(key, value);
        }

        public void UpdateRange(IEnumerable<IKeyValuePair<TKey, TValue>> newItems)
        {
            _mutationsGoHere.UpdateRange(newItems);
        }

        public void UpdateRange(IEnumerable<KeyValuePair<TKey, TValue>> newItems)
        {
            _mutationsGoHere.UpdateRange(newItems);
        }

        public void UpdateRange<TKeyValuePair>(IEnumerable<TKeyValuePair> newItems, Func<TKeyValuePair, TKey> key, Func<TKeyValuePair, TValue> value)
        {
            _mutationsGoHere.UpdateRange(newItems, key, value);
        }

        public void Update(TKey key, TValue value, out TValue previousValue)
        {
            _mutationsGoHere.Update(key, value, out previousValue);
        }

        public void UpdateRange(IEnumerable<IKeyValuePair<TKey, TValue>> newItems, out IReadOnlyDictionaryEx<TKey, IDictionaryItemUpdateAttempt<TValue>> results)
        {
            _mutationsGoHere.UpdateRange(newItems, out results);
        }

        public void UpdateRange(IEnumerable<KeyValuePair<TKey, TValue>> newItems, out IReadOnlyDictionaryEx<TKey, IDictionaryItemUpdateAttempt<TValue>> results)
        {
            _mutationsGoHere.UpdateRange(newItems, out results);
        }

        public void UpdateRange<TKeyValuePair>(IEnumerable<TKeyValuePair> newItems, Func<TKeyValuePair, TKey> key, Func<TKeyValuePair, TValue> value, out IReadOnlyDictionaryEx<TKey, IDictionaryItemUpdateAttempt<TValue>> results)
        {
            _mutationsGoHere.UpdateRange(newItems, key, value, out results);
        }

        public void UpdateRange(params IKeyValuePair<TKey, TValue>[] newItems)
        {
            _mutationsGoHere.UpdateRange(newItems);
        }

        public void UpdateRange(params KeyValuePair<TKey, TValue>[] newItems)
        {
            _mutationsGoHere.UpdateRange(newItems);
        }

        public DictionaryItemAddOrUpdateResult AddOrUpdate(TKey key, TValue value)
        {
            return _mutationsGoHere.AddOrUpdate(key, value);
        }

        public DictionaryItemAddOrUpdateResult AddOrUpdate(TKey key, Func<TValue> valueIfAdding, Func<TValue, TValue> valueIfUpdating)
        {
            return _mutationsGoHere.AddOrUpdate(key, valueIfAdding, valueIfUpdating);
        }

        public DictionaryItemAddOrUpdateResult AddOrUpdate(TKey key, Func<TValue> valueIfAdding, Func<TValue, TValue> valueIfUpdating,
            out TValue previousValue, out TValue newValue)
        {
            return _mutationsGoHere.AddOrUpdate(key, valueIfAdding, valueIfUpdating, out previousValue, out newValue);
        }

        public void AddOrUpdateRange(IEnumerable<IKeyValuePair<TKey, TValue>> newItems, out IReadOnlyDictionaryEx<TKey, IDictionaryItemAddOrUpdate<TValue>> result)
        {
            _mutationsGoHere.AddOrUpdateRange(newItems, out result);
        }

        public void AddOrUpdateRange(IEnumerable<KeyValuePair<TKey, TValue>> newItems, out IReadOnlyDictionaryEx<TKey, IDictionaryItemAddOrUpdate<TValue>> result)
        {
            _mutationsGoHere.AddOrUpdateRange(newItems, out result);
        }

        public void AddOrUpdateRange<TKeyValuePair>(IEnumerable<TKeyValuePair> newItems, Func<TKeyValuePair, TKey> key, Func<TKeyValuePair, TValue> value, out IReadOnlyDictionaryEx<TKey, IDictionaryItemAddOrUpdate<TValue>> result)
        {
            _mutationsGoHere.AddOrUpdateRange(newItems, key, value, out result);
        }

        public void AddOrUpdateRange(IEnumerable<IKeyValuePair<TKey, TValue>> newItems)
        {
            _mutationsGoHere.AddOrUpdateRange(newItems);
        }

        public void AddOrUpdateRange(IEnumerable<KeyValuePair<TKey, TValue>> newItems)
        {
            _mutationsGoHere.AddOrUpdateRange(newItems);
        }

        public void AddOrUpdateRange<TKeyValuePair>(IEnumerable<TKeyValuePair> newItems, Func<TKeyValuePair, TKey> key, Func<TKeyValuePair, TValue> value)
        {
            _mutationsGoHere.AddOrUpdateRange(newItems, key, value);
        }

        public void AddOrUpdateRange(params IKeyValuePair<TKey, TValue>[] newItems)
        {
            _mutationsGoHere.AddOrUpdateRange(newItems);
        }

        public void AddOrUpdateRange(params KeyValuePair<TKey, TValue>[] newItems)
        {
            _mutationsGoHere.AddOrUpdateRange(newItems);
        }

        public void TryRemoveRange(IEnumerable<TKey> keysToRemove)
        {
            _mutationsGoHere.TryRemoveRange(keysToRemove);
        }

        public void RemoveRange(IEnumerable<TKey> keysToRemove)
        {
            _mutationsGoHere.RemoveRange(keysToRemove);
        }

        public void RemoveWhere(Func<TKey, TValue, bool> predicate)
        {
            _mutationsGoHere.RemoveWhere(predicate);
        }

        public void RemoveWhere(Func<IKeyValuePair<TKey, TValue>, bool> predicate)
        {
            _mutationsGoHere.RemoveWhere(predicate);
        }

        public void Clear()
        {
            _mutationsGoHere.Clear();
        }

        public bool TryRemove(TKey key)
        {
            return _mutationsGoHere.TryRemove(key);
        }

        public void Remove(TKey key)
        {
            _mutationsGoHere.Remove(key);
        }

        public void TryRemoveRange(IEnumerable<TKey> keysToRemove, out IReadOnlyDictionaryEx<TKey, TValue> removedItems)
        {
            _mutationsGoHere.TryRemoveRange(keysToRemove, out removedItems);
        }

        public void RemoveRange(IEnumerable<TKey> keysToRemove, out IReadOnlyDictionaryEx<TKey, TValue> removedItems)
        {
            _mutationsGoHere.RemoveRange(keysToRemove, out removedItems);
        }

        public void RemoveWhere(Func<TKey, TValue, bool> predicate, out IReadOnlyDictionaryEx<TKey, TValue> removedItems)
        {
            _mutationsGoHere.RemoveWhere(predicate, out removedItems);
        }

        public void RemoveWhere(Func<IKeyValuePair<TKey, TValue>, bool> predicate, out IReadOnlyDictionaryEx<TKey, TValue> removedItems)
        {
            _mutationsGoHere.RemoveWhere(predicate, out removedItems);
        }

        public void Clear(out IReadOnlyDictionaryEx<TKey, TValue> removedItems)
        {
            _mutationsGoHere.Clear(out removedItems);
        }

        public bool TryRemove(TKey key, out TValue removedItem)
        {
            return _mutationsGoHere.TryRemove(key, out removedItem);
        }

        public void Remove(TKey key, out TValue removedItem)
        {
            _mutationsGoHere.Remove(key, out removedItem);
        }

        public TValue this[TKey key]
        {
            get
            {
                return TryGetValue(key).Value;
            }
            set => _mutationsGoHere[key] = value;
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _wrapped.Select(x => x.ToLiveLinq().AsObservable()).Merge().ToLiveLinq();
        }
    }
}