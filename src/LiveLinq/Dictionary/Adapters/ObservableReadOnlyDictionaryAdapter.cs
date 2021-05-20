using System;
using System.Collections;
using System.Collections.Generic;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary.Interfaces;
using SimpleMonads;

namespace LiveLinq.Dictionary.Adapters
{
    public class ObservableReadOnlyDictionaryAdapter<TKey, TValue> : IObservableReadOnlyDictionary<TKey, TValue>
    {
        private readonly IComposableReadOnlyDictionary<TKey, TValue> _state;
        private readonly IObservable<IDictionaryChangeStrict<TKey, TValue>> _observable;

        public ObservableReadOnlyDictionaryAdapter(IComposableReadOnlyDictionary<TKey, TValue> state, IObservable<IDictionaryChangeStrict<TKey, TValue>> observable)
        {
            _state = state;
            _observable = observable;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return new DictionaryChangesStrict<TKey, TValue>(_observable);
        }

        public IEnumerator<IKeyValue<TKey, TValue>> GetEnumerator()
        {
            return _state.GetEnumerator();
        }

        public int Count => _state.Count;

        public TValue GetValue(TKey key)
        {
            return _state.GetValue(key);
        }

        public bool ContainsKey(TKey key)
        {
            return _state.ContainsKey(key);
        }

        public TValue? TryGetValue(TKey key)
        {
            return _state.TryGetValue(key);
        }

        public IEqualityComparer<TKey> Comparer => _state.Comparer;

        public TValue this[TKey key] => _state[key];

        public IEnumerable<TKey> Keys => _state.Keys;

        public IEnumerable<TValue> Values => _state.Values;
    }
}