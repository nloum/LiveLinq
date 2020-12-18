using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Core;
using LiveLinq.Dictionary.Interfaces;
using SimpleMonads;

namespace LiveLinq.Dictionary
{
    public class BindableDictionaryAdapter<TKey, TValue> : INotifyCollectionChanged, IDisposable, IComposableReadOnlyDictionary<TKey, TValue>
    {
        private readonly IObservableReadOnlyDictionary<TKey, TValue> _source;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private readonly IDisposable _subscription;
        public void Dispose()
        {
            _subscription.Dispose();
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        public BindableDictionaryAdapter(IObservableReadOnlyDictionary<TKey, TValue> source)
        {
            _source = source;
            _subscription = _source.ToLiveLinq().AsObservable().Subscribe(dictionaryChange =>
            {
                if (dictionaryChange.Type == CollectionChangeType.Add)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, dictionaryChange.KeyValuePairs));
                }
                else
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, dictionaryChange.KeyValuePairs));
                }
            });
        }

        public IEnumerator<IKeyValue<TKey, TValue>> GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _source.Count;

        public TValue GetValue(TKey key)
        {
            return _source.GetValue(key);
        }

        public bool ContainsKey(TKey key)
        {
            return _source.ContainsKey(key);
        }

        public IMaybe<TValue> TryGetValue(TKey key)
        {
            return _source.TryGetValue(key);
        }

        public IEqualityComparer<TKey> Comparer => _source.Comparer;

        public TValue this[TKey key] => _source[key];

        public IEnumerable<TKey> Keys => _source.Keys;

        public IEnumerable<TValue> Values => _source.Values;
    }
}