using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.Interfaces;
using ComposableCollections.Dictionary.Write;
using LiveLinq.Dictionary.Interfaces;
using SimpleMonads;

namespace LiveLinq.Dictionary
{
    public class AggregateObservableDictionary<TKey, TValue> : IObservableDictionary<TKey, TValue>
    {
        private readonly IObservableDictionary<TKey, TValue> _mutationsGoHere;
        private readonly ImmutableList<IObservableReadOnlyDictionary<TKey, TValue>> _sources;

        public AggregateObservableDictionary(IObservableDictionary<TKey, TValue> mutationsGoHere, IEnumerable<IObservableReadOnlyDictionary<TKey, TValue>> sources)
        {
            _mutationsGoHere = mutationsGoHere;
            _sources = sources.Concat(new IObservableReadOnlyDictionary<TKey, TValue>[]{mutationsGoHere}).ToImmutableList();
        }

        public void Dispose()
        {
            _mutationsGoHere.Dispose();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IKeyValue<TKey, TValue>> GetEnumerator()
        {
            return _sources.SelectMany(x => x).GetEnumerator();
        }

        public int Count => _sources.Select(x => x.Count).Sum();

        public bool ContainsKey(TKey key)
        {
            return _sources.Any(x => x.ContainsKey(key));
        }

        public TValue GetOrAdd(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        public TValue GetOrAdd(TKey key, Func<TValue> value)
        {
            throw new NotImplementedException();
        }

        public TValue GetValue(TKey key)
        {
            return this[key];
        }

        public void SetValue(TKey key, TValue value)
        {
            this[key] = value;
        }

        public TValue? TryGetValue(TKey key)
        {
            foreach (var wrapped in _sources)
            {
                var result = wrapped.TryGetValue(key);
                if (result != null)
                {
                    return result;
                }
            }
            
            return default;
        }

        public IEqualityComparer<TKey> Comparer => _mutationsGoHere.Comparer;

        public IEnumerable<TKey> Keys => _sources.SelectMany(x => x.Keys);

        public IEnumerable<TValue> Values => _sources.SelectMany(x => x.Values);

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = TryGetValue(key);
            return value != null;
        }

        public void Write(IEnumerable<DictionaryWrite<TKey, TValue>> mutations, out IReadOnlyList<DictionaryWriteResult<TKey, TValue>> results)
        {
            _mutationsGoHere.Write(mutations, out results);
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

        public bool TryAdd(TKey key, TValue value, out TValue previousValue, out TValue result)
        {
            return TryAdd(key, () => value, out previousValue, out result);
        }

        public void TryAddRange(IEnumerable<IKeyValue<TKey, TValue>> newItems, out IComposableReadOnlyDictionary<TKey, IDictionaryItemAddAttempt<TValue>> result)
        {
            _mutationsGoHere.TryAddRange(newItems, out result);
        }

        public void TryAddRange(IEnumerable<KeyValuePair<TKey, TValue>> newItems, out IComposableReadOnlyDictionary<TKey, IDictionaryItemAddAttempt<TValue>> result)
        {
            _mutationsGoHere.TryAddRange(newItems, out result);
        }

        public void TryAddRange<TKeyValuePair>(IEnumerable<TKeyValuePair> newItems, Func<TKeyValuePair, TKey> key, Func<TKeyValuePair, TValue> value, out IComposableReadOnlyDictionary<TKey, IDictionaryItemAddAttempt<TValue>> result)
        {
            _mutationsGoHere.TryAddRange(newItems, key, value, out result);
        }

        public void TryAddRange(IEnumerable<IKeyValue<TKey, TValue>> newItems)
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

        public void TryAddRange(params IKeyValue<TKey, TValue>[] newItems)
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

        public void AddRange(IEnumerable<IKeyValue<TKey, TValue>> newItems)
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

        public void AddRange(params IKeyValue<TKey, TValue>[] newItems)
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

        public void TryUpdateRange(IEnumerable<IKeyValue<TKey, TValue>> newItems)
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

        public void TryUpdateRange(params IKeyValue<TKey, TValue>[] newItems)
        {
            _mutationsGoHere.TryUpdateRange(newItems);
        }

        public void TryUpdateRange(params KeyValuePair<TKey, TValue>[] newItems)
        {
            _mutationsGoHere.TryUpdateRange(newItems);
        }

        public void TryUpdateRange(IEnumerable<IKeyValue<TKey, TValue>> newItems, out IComposableReadOnlyDictionary<TKey, IDictionaryItemUpdateAttempt<TValue>> result)
        {
            _mutationsGoHere.TryUpdateRange(newItems, out result);
        }

        public void TryUpdateRange(IEnumerable<KeyValuePair<TKey, TValue>> newItems, out IComposableReadOnlyDictionary<TKey, IDictionaryItemUpdateAttempt<TValue>> result)
        {
            _mutationsGoHere.TryUpdateRange(newItems, out result);
        }

        public void TryUpdateRange<TKeyValuePair>(IEnumerable<TKeyValuePair> newItems, Func<TKeyValuePair, TKey> key, Func<TKeyValuePair, TValue> value, out IComposableReadOnlyDictionary<TKey, IDictionaryItemUpdateAttempt<TValue>> result)
        {
            _mutationsGoHere.TryUpdateRange(newItems, key, value, out result);
        }

        public void Update(TKey key, TValue value)
        {
            _mutationsGoHere.Update(key, value);
        }

        public void UpdateRange(IEnumerable<IKeyValue<TKey, TValue>> newItems)
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

        public void UpdateRange(IEnumerable<IKeyValue<TKey, TValue>> newItems, out IComposableReadOnlyDictionary<TKey, IDictionaryItemUpdateAttempt<TValue>> results)
        {
            _mutationsGoHere.UpdateRange(newItems, out results);
        }

        public void UpdateRange(IEnumerable<KeyValuePair<TKey, TValue>> newItems, out IComposableReadOnlyDictionary<TKey, IDictionaryItemUpdateAttempt<TValue>> results)
        {
            _mutationsGoHere.UpdateRange(newItems, out results);
        }

        public void UpdateRange<TKeyValuePair>(IEnumerable<TKeyValuePair> newItems, Func<TKeyValuePair, TKey> key, Func<TKeyValuePair, TValue> value, out IComposableReadOnlyDictionary<TKey, IDictionaryItemUpdateAttempt<TValue>> results)
        {
            _mutationsGoHere.UpdateRange(newItems, key, value, out results);
        }

        public void UpdateRange(params IKeyValue<TKey, TValue>[] newItems)
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

        public void AddOrUpdateRange(IEnumerable<IKeyValue<TKey, TValue>> newItems, out IComposableReadOnlyDictionary<TKey, IDictionaryItemAddOrUpdate<TValue>> result)
        {
            _mutationsGoHere.AddOrUpdateRange(newItems, out result);
        }

        public void AddOrUpdateRange(IEnumerable<KeyValuePair<TKey, TValue>> newItems, out IComposableReadOnlyDictionary<TKey, IDictionaryItemAddOrUpdate<TValue>> result)
        {
            _mutationsGoHere.AddOrUpdateRange(newItems, out result);
        }

        public void AddOrUpdateRange<TKeyValuePair>(IEnumerable<TKeyValuePair> newItems, Func<TKeyValuePair, TKey> key, Func<TKeyValuePair, TValue> value, out IComposableReadOnlyDictionary<TKey, IDictionaryItemAddOrUpdate<TValue>> result)
        {
            _mutationsGoHere.AddOrUpdateRange(newItems, key, value, out result);
        }

        public void AddOrUpdateRange(IEnumerable<IKeyValue<TKey, TValue>> newItems)
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

        public void AddOrUpdateRange(params IKeyValue<TKey, TValue>[] newItems)
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

        public void RemoveWhere(Func<IKeyValue<TKey, TValue>, bool> predicate)
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

        public void TryRemoveRange(IEnumerable<TKey> keysToRemove, out IComposableReadOnlyDictionary<TKey, TValue> removedItems)
        {
            _mutationsGoHere.TryRemoveRange(keysToRemove, out removedItems);
        }

        public void RemoveRange(IEnumerable<TKey> keysToRemove, out IComposableReadOnlyDictionary<TKey, TValue> removedItems)
        {
            _mutationsGoHere.RemoveRange(keysToRemove, out removedItems);
        }

        public void RemoveWhere(Func<TKey, TValue, bool> predicate, out IComposableReadOnlyDictionary<TKey, TValue> removedItems)
        {
            _mutationsGoHere.RemoveWhere(predicate, out removedItems);
        }

        public void RemoveWhere(Func<IKeyValue<TKey, TValue>, bool> predicate, out IComposableReadOnlyDictionary<TKey, TValue> removedItems)
        {
            _mutationsGoHere.RemoveWhere(predicate, out removedItems);
        }

        public void Clear(out IComposableReadOnlyDictionary<TKey, TValue> removedItems)
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
            get => TryGetValue(key)!;
            set => _mutationsGoHere[key] = value;
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _sources.Select(x => x.ToLiveLinq().AsObservable()).Merge().ToLiveLinq();
        }
    }
}