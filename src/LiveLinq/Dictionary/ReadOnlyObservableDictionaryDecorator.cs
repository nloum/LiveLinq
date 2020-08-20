using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Linq;
using ComposableCollections.Dictionary;
using SimpleMonads;
using UtilityDisposables;

namespace LiveLinq.Dictionary
{
    public class ReadOnlyObservableDictionaryDecorator<TKey, TValue> : IReadOnlyObservableDictionary<TKey, TValue>
    {
        private readonly IComposableReadOnlyDictionary<TKey, TValue> _source;
        private readonly IObservable<IDictionaryChangeStrict<TKey, TValue>> _observable;
        public DisposableCollector DisposableCollector { get; } = new DisposableCollector();

        public ReadOnlyObservableDictionaryDecorator(IComposableReadOnlyDictionary<TKey, TValue> source,
            IObservable<IDictionaryChangeStrict<TKey, TValue>> observable)
        {
            _source = source;
            _observable = observable;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            DisposableCollector.Dispose();
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return Observable.Create<IDictionaryChangeStrict<TKey, TValue>>(observer =>
            {
                var items = this.ToImmutableList();
                if (items.Count > 0)
                {
                    observer.OnNext(Utility.DictionaryAdd(items));
                }
                var result = _observable.Where(x => x.Values.Count > 0).Subscribe(observer);
                return result;
            }).ToLiveLinq();
        }

        public IEnumerator<IKeyValue<TKey, TValue>> GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        public int Count => _source.Count;

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