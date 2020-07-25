using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
    public class ObservableDictionaryDecorator<TKey, TValue> : IObservableDictionary<TKey, TValue>
    {
        private readonly Subject<IDictionaryChangeStrict<TKey, TValue>> _subject = new Subject<IDictionaryChangeStrict<TKey, TValue>>();

        private IDictionaryEx<TKey, TValue> _state;

        public ObservableDictionaryDecorator(IDictionaryEx<TKey, TValue> state)
        {
            _state = state;
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return Observable.Create<IDictionaryChangeStrict<TKey, TValue>>(observer =>
            {
                var result = _subject.Where(x => x.Values.Count > 0).Subscribe(observer);
                var items = ((IReadOnlyDictionaryEx<TKey, TValue>)this).ToImmutableList();
                if (items.Count > 0)
                {
                    observer.OnNext(Utility.DictionaryAdd(items));
                }

                return result;
            }).ToLiveLinq();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IKeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _state.GetEnumerator();
        }

        public int Count => _state.Count;

        public bool ContainsKey(TKey key)
        {
            return _state.ContainsKey(key);
        }

        public IMaybe<TValue> TryGetValue(TKey key)
        {
            return _state.TryGetValue(key);
        }

        public IEqualityComparer<TKey> Comparer => _state.Comparer;

        public IEnumerable<TKey> Keys => _state.Keys;

        public IEnumerable<TValue> Values => _state.Values;

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _state.TryGetValue(key, out value);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            if (_state.TryAdd(key, value))
            {
                _subject.OnNext(Utility.DictionaryAdd(MoreCollections.Utility.KeyValuePair(key, value)));
                return true;
            }
            
            return false;
        }

        public bool TryAdd(TKey key, Func<TValue> value)
        {
            if (_state.TryAdd(key, value, out var existingValue, out var newValue))
            {
                _subject.OnNext(Utility.DictionaryAdd(MoreCollections.Utility.KeyValuePair(key, newValue)));
                return true;
            }
            
            return false;
        }

        public bool TryAdd(TKey key, Func<TValue> value, out TValue existingValue, out TValue newValue)
        {
            if (_state.TryAdd(key, value, out existingValue, out newValue))
            {
                _subject.OnNext(Utility.DictionaryAdd(MoreCollections.Utility.KeyValuePair(key, newValue)));
                return true;
            }

            return false;
        }

        public void TryAddRange(IEnumerable<IKeyValuePair<TKey, TValue>> newItems, out IReadOnlyDictionaryEx<TKey, IDictionaryItemAddAttempt<TValue>> result)
        {
            _state.TryAddRange(newItems, out result);
            _subject.OnNext(Utility.DictionaryAdd(result.Where(x => x.Value.Added).Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void TryAddRange(IEnumerable<KeyValuePair<TKey, TValue>> newItems, out IReadOnlyDictionaryEx<TKey, IDictionaryItemAddAttempt<TValue>> result)
        {
            _state.TryAddRange(newItems, out result);
            _subject.OnNext(Utility.DictionaryAdd(result.Where(x => x.Value.Added).Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void TryAddRange<TKeyValuePair>(IEnumerable<TKeyValuePair> newItems, Func<TKeyValuePair, TKey> key, Func<TKeyValuePair, TValue> value, out IReadOnlyDictionaryEx<TKey, IDictionaryItemAddAttempt<TValue>> result)
        {
            _state.TryAddRange(newItems, key, value, out result);
            _subject.OnNext(Utility.DictionaryAdd(result.Where(x => x.Value.Added).Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void TryAddRange(IEnumerable<IKeyValuePair<TKey, TValue>> newItems)
        {
            _state.TryAddRange(newItems, out var result);
            _subject.OnNext(Utility.DictionaryAdd(result.Where(x => x.Value.Added).Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void TryAddRange(IEnumerable<KeyValuePair<TKey, TValue>> newItems)
        {
            _state.TryAddRange(newItems, out var result);
            _subject.OnNext(Utility.DictionaryAdd(result.Where(x => x.Value.Added).Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void TryAddRange<TKeyValuePair>(IEnumerable<TKeyValuePair> newItems, Func<TKeyValuePair, TKey> key, Func<TKeyValuePair, TValue> value)
        {
            _state.TryAddRange(newItems, key, value, out var result);
            _subject.OnNext(Utility.DictionaryAdd(result.Where(x => x.Value.Added).Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void TryAddRange(params IKeyValuePair<TKey, TValue>[] newItems)
        {
            _state.TryAddRange(newItems, out var result);
            _subject.OnNext(Utility.DictionaryAdd(result.Where(x => x.Value.Added).Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void TryAddRange(params KeyValuePair<TKey, TValue>[] newItems)
        {
            _state.TryAddRange(newItems, out var result);
            _subject.OnNext(Utility.DictionaryAdd(result.Where(x => x.Value.Added).Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void Add(TKey key, TValue value)
        {
            _state.Add(key, value);
            _subject.OnNext(Utility.DictionaryAdd(MoreCollections.Utility.KeyValuePair(key, value)));
        }

        public void AddRange(IEnumerable<IKeyValuePair<TKey, TValue>> newItems)
        {
            _state.TryAddRange(newItems, out var result);
            _subject.OnNext(Utility.DictionaryAdd(result.Where(x => x.Value.Added).Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> newItems)
        {
            _state.TryAddRange(newItems, out var result);
            _subject.OnNext(Utility.DictionaryAdd(result.Where(x => x.Value.Added).Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void AddRange<TKeyValuePair>(IEnumerable<TKeyValuePair> newItems, Func<TKeyValuePair, TKey> key, Func<TKeyValuePair, TValue> value)
        {
            _state.TryAddRange(newItems, key, value, out var result);
            _subject.OnNext(Utility.DictionaryAdd(result.Where(x => x.Value.Added).Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void AddRange(params IKeyValuePair<TKey, TValue>[] newItems)
        {
            _state.TryAddRange(newItems, out var result);
            _subject.OnNext(Utility.DictionaryAdd(result.Where(x => x.Value.Added).Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void AddRange(params KeyValuePair<TKey, TValue>[] newItems)
        {
            _state.TryAddRange(newItems, out var result);
            _subject.OnNext(Utility.DictionaryAdd(result.Where(x => x.Value.Added).Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public bool TryUpdate(TKey key, TValue value)
        {
            if (_state.TryUpdate(key, value, out var previousValue))
            {
                _subject.OnNext(Utility.DictionaryRemove(MoreCollections.Utility.KeyValuePair(key, previousValue)));
                _subject.OnNext(Utility.DictionaryAdd(MoreCollections.Utility.KeyValuePair(key, value)));
                return true;
            }

            return false;
        }

        public bool TryUpdate(TKey key, TValue value, out TValue previousValue)
        {
            if (_state.TryUpdate(key, value, out previousValue))
            {
                _subject.OnNext(Utility.DictionaryRemove(MoreCollections.Utility.KeyValuePair(key, previousValue)));
                _subject.OnNext(Utility.DictionaryAdd(MoreCollections.Utility.KeyValuePair(key, value)));
                return true;
            }

            return false;
        }

        public bool TryUpdate(TKey key, Func<TValue, TValue> value, out TValue previousValue, out TValue newValue)
        {
            if (_state.TryUpdate(key, value, out previousValue, out newValue))
            {
                _subject.OnNext(Utility.DictionaryRemove(MoreCollections.Utility.KeyValuePair(key, previousValue)));
                _subject.OnNext(Utility.DictionaryAdd(MoreCollections.Utility.KeyValuePair(key, newValue)));
                return true;
            }

            return false;
        }

        public void TryUpdateRange(IEnumerable<IKeyValuePair<TKey, TValue>> newItems)
        {
            _state.TryUpdateRange(newItems, out var results);
            var resultsList = results.Where(x => x.Value.Updated).ToImmutableList();
            _subject.OnNext(Utility.DictionaryRemove(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.ExistingValue.Value))));
            _subject.OnNext(Utility.DictionaryAdd(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void TryUpdateRange(IEnumerable<KeyValuePair<TKey, TValue>> newItems)
        {
            _state.TryUpdateRange(newItems, out var results);
            var resultsList = results.Where(x => x.Value.Updated).ToImmutableList();
            _subject.OnNext(Utility.DictionaryRemove(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.ExistingValue.Value))));
            _subject.OnNext(Utility.DictionaryAdd(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void TryUpdateRange<TKeyValuePair>(IEnumerable<TKeyValuePair> newItems, Func<TKeyValuePair, TKey> key, Func<TKeyValuePair, TValue> value)
        {
            _state.TryUpdateRange(newItems, key, value, out var results);
            var resultsList = results.Where(x => x.Value.Updated).ToImmutableList();
            _subject.OnNext(Utility.DictionaryRemove(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.ExistingValue.Value))));
            _subject.OnNext(Utility.DictionaryAdd(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void TryUpdateRange(params IKeyValuePair<TKey, TValue>[] newItems)
        {
            _state.TryUpdateRange(newItems, out var results);
            var resultsList = results.Where(x => x.Value.Updated).ToImmutableList();
            _subject.OnNext(Utility.DictionaryRemove(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.ExistingValue.Value))));
            _subject.OnNext(Utility.DictionaryAdd(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void TryUpdateRange(params KeyValuePair<TKey, TValue>[] newItems)
        {
            _state.TryUpdateRange(newItems, out var results);
            var resultsList = results.Where(x => x.Value.Updated).ToImmutableList();
            _subject.OnNext(Utility.DictionaryRemove(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.ExistingValue.Value))));
            _subject.OnNext(Utility.DictionaryAdd(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void TryUpdateRange(IEnumerable<IKeyValuePair<TKey, TValue>> newItems, out IReadOnlyDictionaryEx<TKey, IDictionaryItemUpdateAttempt<TValue>> results)
        {
            _state.TryUpdateRange(newItems, out results);
            var resultsList = results.Where(x => x.Value.Updated).ToImmutableList();
            _subject.OnNext(Utility.DictionaryRemove(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.ExistingValue.Value))));
            _subject.OnNext(Utility.DictionaryAdd(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void TryUpdateRange(IEnumerable<KeyValuePair<TKey, TValue>> newItems, out IReadOnlyDictionaryEx<TKey, IDictionaryItemUpdateAttempt<TValue>> results)
        {
            _state.TryUpdateRange(newItems, out results);
            var resultsList = results.Where(x => x.Value.Updated).ToImmutableList();
            _subject.OnNext(Utility.DictionaryRemove(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.ExistingValue.Value))));
            _subject.OnNext(Utility.DictionaryAdd(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void TryUpdateRange<TKeyValuePair>(IEnumerable<TKeyValuePair> newItems, Func<TKeyValuePair, TKey> key, Func<TKeyValuePair, TValue> value, out IReadOnlyDictionaryEx<TKey, IDictionaryItemUpdateAttempt<TValue>> results)
        {
            _state.TryUpdateRange(newItems, key, value, out results);
            var resultsList = results.Where(x => x.Value.Updated).ToImmutableList();
            _subject.OnNext(Utility.DictionaryRemove(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.ExistingValue.Value))));
            _subject.OnNext(Utility.DictionaryAdd(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void Update(TKey key, TValue value)
        {
            _state.Update(key, value, out var previousValue);
            _subject.OnNext(Utility.DictionaryRemove(MoreCollections.Utility.KeyValuePair(key, previousValue)));
            _subject.OnNext(Utility.DictionaryAdd(MoreCollections.Utility.KeyValuePair(key, value)));
        }

        public void UpdateRange(IEnumerable<IKeyValuePair<TKey, TValue>> newItems)
        {
            _state.UpdateRange(newItems, out var results);
            var resultsList = results.Where(x => x.Value.Updated).ToImmutableList();
            _subject.OnNext(Utility.DictionaryRemove(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.ExistingValue.Value))));
            _subject.OnNext(Utility.DictionaryAdd(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void UpdateRange(IEnumerable<KeyValuePair<TKey, TValue>> newItems)
        {
            _state.UpdateRange(newItems, out var results);
            var resultsList = results.Where(x => x.Value.Updated).ToImmutableList();
            _subject.OnNext(Utility.DictionaryRemove(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.ExistingValue.Value))));
            _subject.OnNext(Utility.DictionaryAdd(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void UpdateRange<TKeyValuePair>(IEnumerable<TKeyValuePair> newItems, Func<TKeyValuePair, TKey> key, Func<TKeyValuePair, TValue> value)
        {
            _state.UpdateRange(newItems, key, value, out var results);
            var resultsList = results.Where(x => x.Value.Updated).ToImmutableList();
            _subject.OnNext(Utility.DictionaryRemove(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.ExistingValue.Value))));
            _subject.OnNext(Utility.DictionaryAdd(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void Update(TKey key, TValue value, out TValue previousValue)
        {
            _state.Update(key, value, out previousValue);
            _subject.OnNext(Utility.DictionaryRemove(MoreCollections.Utility.KeyValuePair(key, previousValue)));
            _subject.OnNext(Utility.DictionaryAdd(MoreCollections.Utility.KeyValuePair(key, value)));
        }

        public void UpdateRange(IEnumerable<IKeyValuePair<TKey, TValue>> newItems, out IReadOnlyDictionaryEx<TKey, IDictionaryItemUpdateAttempt<TValue>> results)
        {
            _state.UpdateRange(newItems, out results);
            var resultsList = results.Where(x => x.Value.Updated).ToImmutableList();
            _subject.OnNext(Utility.DictionaryRemove(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.ExistingValue.Value))));
            _subject.OnNext(Utility.DictionaryAdd(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void UpdateRange(IEnumerable<KeyValuePair<TKey, TValue>> newItems, out IReadOnlyDictionaryEx<TKey, IDictionaryItemUpdateAttempt<TValue>> results)
        {
            _state.UpdateRange(newItems, out results);
            var resultsList = results.Where(x => x.Value.Updated).ToImmutableList();
            _subject.OnNext(Utility.DictionaryRemove(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.ExistingValue.Value))));
            _subject.OnNext(Utility.DictionaryAdd(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void UpdateRange<TKeyValuePair>(IEnumerable<TKeyValuePair> newItems, Func<TKeyValuePair, TKey> key, Func<TKeyValuePair, TValue> value, out IReadOnlyDictionaryEx<TKey, IDictionaryItemUpdateAttempt<TValue>> results)
        {
            _state.UpdateRange(newItems, key, value, out results);
            var resultsList = results.Where(x => x.Value.Updated).ToImmutableList();
            _subject.OnNext(Utility.DictionaryRemove(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.ExistingValue.Value))));
            _subject.OnNext(Utility.DictionaryAdd(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void UpdateRange(params IKeyValuePair<TKey, TValue>[] newItems)
        {
            _state.UpdateRange(newItems, out var results);
            var resultsList = results.Where(x => x.Value.Updated).ToImmutableList();
            _subject.OnNext(Utility.DictionaryRemove(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.ExistingValue.Value))));
            _subject.OnNext(Utility.DictionaryAdd(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public void UpdateRange(params KeyValuePair<TKey, TValue>[] newItems)
        {
            _state.UpdateRange(newItems, out var results);
            var resultsList = results.Where(x => x.Value.Updated).ToImmutableList();
            _subject.OnNext(Utility.DictionaryRemove(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.ExistingValue.Value))));
            _subject.OnNext(Utility.DictionaryAdd(resultsList.Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue.Value))));
        }

        public DictionaryItemAddOrUpdateResult AddOrUpdate(TKey key, TValue value)
        {
            var result = _state.AddOrUpdate(key, value);
            if (result == DictionaryItemAddOrUpdateResult.Add)
            {
                _subject.OnNext(Utility.DictionaryRemove(MoreCollections.Utility.KeyValuePair(key, value)));
            }

            _subject.OnNext(Utility.DictionaryAdd(MoreCollections.Utility.KeyValuePair(key, value)));

            return result;
        }

        public DictionaryItemAddOrUpdateResult AddOrUpdate(TKey key, Func<TValue> valueIfAdding, Func<TValue, TValue> valueIfUpdating)
        {
            var result = _state.AddOrUpdate(key, valueIfAdding, valueIfUpdating, out var previousValue, out var newValue);
            if (result == DictionaryItemAddOrUpdateResult.Add)
            {
                _subject.OnNext(Utility.DictionaryRemove(MoreCollections.Utility.KeyValuePair(key, newValue)));
            }

            _subject.OnNext(Utility.DictionaryAdd(MoreCollections.Utility.KeyValuePair(key, newValue)));

            return result;
        }

        public DictionaryItemAddOrUpdateResult AddOrUpdate(TKey key, Func<TValue> valueIfAdding, Func<TValue, TValue> valueIfUpdating,
            out TValue previousValue, out TValue newValue)
        {
            var result = _state.AddOrUpdate(key, valueIfAdding, valueIfUpdating, out previousValue, out newValue);
            if (result == DictionaryItemAddOrUpdateResult.Add)
            {
                _subject.OnNext(Utility.DictionaryRemove(MoreCollections.Utility.KeyValuePair(key, newValue)));
            }

            _subject.OnNext(Utility.DictionaryAdd(MoreCollections.Utility.KeyValuePair(key, newValue)));

            return result;
        }

        public void AddOrUpdateRange(IEnumerable<IKeyValuePair<TKey, TValue>> newItems, out IReadOnlyDictionaryEx<TKey, IDictionaryItemAddOrUpdate<TValue>> results)
        {
            _state.AddOrUpdateRange(newItems, out results);
            SendLiveLinqEventForAddOrUpdate(results);
        }

        private void SendLiveLinqEventForAddOrUpdate(IReadOnlyDictionaryEx<TKey, IDictionaryItemAddOrUpdate<TValue>> results)
        {
            var resultsGrouped = results.GroupBy(x => x.Value.Result)
                .ToImmutableDictionary(x => x.Key, x => x.ToImmutableList());
            _subject.OnNext(Utility.DictionaryRemove(resultsGrouped[DictionaryItemAddOrUpdateResult.Update]
                .Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.ExistingValue.Value))));
            var addedByUpdate = resultsGrouped[DictionaryItemAddOrUpdateResult.Update]
                .Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue));
            var addedBySimpleAdd = resultsGrouped[DictionaryItemAddOrUpdateResult.Add]
                .Select(kvp => MoreCollections.Utility.KeyValuePair(kvp.Key, kvp.Value.NewValue));
            _subject.OnNext(Utility.DictionaryAdd(addedByUpdate.Concat(addedBySimpleAdd)));
        }

        public void AddOrUpdateRange(IEnumerable<KeyValuePair<TKey, TValue>> newItems, out IReadOnlyDictionaryEx<TKey, IDictionaryItemAddOrUpdate<TValue>> results)
        {
            _state.AddOrUpdateRange(newItems, out results);
            SendLiveLinqEventForAddOrUpdate(results);
        }

        public void AddOrUpdateRange<TKeyValuePair>(IEnumerable<TKeyValuePair> newItems, Func<TKeyValuePair, TKey> key, Func<TKeyValuePair, TValue> value, out IReadOnlyDictionaryEx<TKey, IDictionaryItemAddOrUpdate<TValue>> results)
        {
            _state.AddOrUpdateRange(newItems, key, value, out results);
            SendLiveLinqEventForAddOrUpdate(results);
        }

        public void AddOrUpdateRange(IEnumerable<IKeyValuePair<TKey, TValue>> newItems)
        {
            _state.AddOrUpdateRange(newItems, out var results);
            SendLiveLinqEventForAddOrUpdate(results);
        }

        public void AddOrUpdateRange(IEnumerable<KeyValuePair<TKey, TValue>> newItems)
        {
            _state.AddOrUpdateRange(newItems, out var results);
            SendLiveLinqEventForAddOrUpdate(results);
        }

        public void AddOrUpdateRange<TKeyValuePair>(IEnumerable<TKeyValuePair> newItems, Func<TKeyValuePair, TKey> key, Func<TKeyValuePair, TValue> value)
        {
            _state.AddOrUpdateRange(newItems, key, value, out var results);
            SendLiveLinqEventForAddOrUpdate(results);
        }

        public void AddOrUpdateRange(params IKeyValuePair<TKey, TValue>[] newItems)
        {
            _state.AddOrUpdateRange(newItems, out var results);
            SendLiveLinqEventForAddOrUpdate(results);
        }

        public void AddOrUpdateRange(params KeyValuePair<TKey, TValue>[] newItems)
        {
            _state.AddOrUpdateRange(newItems, out var results);
            SendLiveLinqEventForAddOrUpdate(results);
        }

        public void TryRemoveRange(IEnumerable<TKey> keysToRemove)
        {
            _state.TryRemoveRange(keysToRemove, out var results);
            _subject.OnNext(Utility.DictionaryRemove(results));
        }

        public void RemoveRange(IEnumerable<TKey> keysToRemove)
        {
            _state.RemoveRange(keysToRemove, out var results);
            _subject.OnNext(Utility.DictionaryRemove(results));
        }

        public void RemoveWhere(Func<TKey, TValue, bool> predicate)
        {
            _state.RemoveWhere(predicate, out var results);
            _subject.OnNext(Utility.DictionaryRemove(results));
        }

        public void RemoveWhere(Func<IKeyValuePair<TKey, TValue>, bool> predicate)
        {
            _state.RemoveWhere(predicate, out var results);
            _subject.OnNext(Utility.DictionaryRemove(results));
        }

        public void Clear()
        {
            _state.Clear(out var results);
            _subject.OnNext(Utility.DictionaryRemove(results));
        }

        public bool TryRemove(TKey key)
        {
            if (_state.TryRemove(key, out var removedItem))
            {
                _subject.OnNext(Utility.DictionaryRemove(MoreCollections.Utility.KeyValuePair(key, removedItem)));
                return true;
            }

            return false;
        }

        public void Remove(TKey key)
        {
            _state.Remove(key, out var removedItem);
            _subject.OnNext(Utility.DictionaryRemove(MoreCollections.Utility.KeyValuePair(key, removedItem)));
        }

        public void TryRemoveRange(IEnumerable<TKey> keysToRemove, out IReadOnlyDictionaryEx<TKey, TValue> removedItems)
        {
            _state.TryRemoveRange(keysToRemove, out removedItems);
            _subject.OnNext(Utility.DictionaryRemove(removedItems));
        }

        public void RemoveRange(IEnumerable<TKey> keysToRemove, out IReadOnlyDictionaryEx<TKey, TValue> removedItems)
        {
            _state.RemoveRange(keysToRemove, out removedItems);
            _subject.OnNext(Utility.DictionaryRemove(removedItems));
        }

        public void RemoveWhere(Func<TKey, TValue, bool> predicate, out IReadOnlyDictionaryEx<TKey, TValue> removedItems)
        {
            _state.RemoveWhere(predicate, out removedItems);
            _subject.OnNext(Utility.DictionaryRemove(removedItems));
        }

        public void RemoveWhere(Func<IKeyValuePair<TKey, TValue>, bool> predicate, out IReadOnlyDictionaryEx<TKey, TValue> removedItems)
        {
            _state.RemoveWhere(predicate, out removedItems);
            _subject.OnNext(Utility.DictionaryRemove(removedItems));
        }

        public void Clear(out IReadOnlyDictionaryEx<TKey, TValue> removedItems)
        {
            _state.Clear(out removedItems);
            _subject.OnNext(Utility.DictionaryRemove(removedItems));
        }

        public bool TryRemove(TKey key, out TValue removedItem)
        {
            if (_state.TryRemove(key, out removedItem))
            {
                _subject.OnNext(Utility.DictionaryRemove(MoreCollections.Utility.KeyValuePair(key, removedItem)));
                
                return true;
            }

            return false;
        }

        public void Remove(TKey key, out TValue removedItem)
        {
            _state.Remove(key, out removedItem);
            _subject.OnNext(Utility.DictionaryRemove(MoreCollections.Utility.KeyValuePair(key, removedItem)));
        }

        public TValue this[TKey key]
        {
            get => _state[key];
            set => _state[key] = value;
        }
    }
}