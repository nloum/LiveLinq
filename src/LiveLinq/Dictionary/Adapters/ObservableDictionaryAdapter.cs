using System;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using LiveLinq.Core;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.Base;
using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary.Interfaces;
using UtilityDisposables;

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
    public class ObservableDictionaryAdapter<TKey, TValue> : ObservableDictionaryAdapterBase<TKey, TValue>,
        IObservableDictionary<TKey, TValue>
    {
        private readonly IObservable<IDictionaryChangeStrict<TKey, TValue>> _observable;
        private readonly Action<IDictionaryChangeStrict<TKey, TValue>> _onChange;
        public DisposableCollector DisposableCollector { get; } = new DisposableCollector();
        
        public ObservableDictionaryAdapter(IComposableDictionary<TKey, TValue> state) : this(state, new Subject<IDictionaryChangeStrict<TKey, TValue>>())
        {
        }

        public ObservableDictionaryAdapter(IComposableDictionary<TKey, TValue> state, Subject<IDictionaryChangeStrict<TKey, TValue>> subject) : this(state, subject, subject.OnNext)
        {
            DisposableCollector.Disposes(subject);
        }
        
        public ObservableDictionaryAdapter(IComposableDictionary<TKey, TValue> state, IObservable<IDictionaryChangeStrict<TKey, TValue>> observable, Action<IDictionaryChangeStrict<TKey, TValue>> onChange) : base(state)
        {
            _observable = observable;
            _onChange = onChange;
        }

        protected override void OnRemove(IKeyValue<TKey, TValue> keyValue)
        {
            _onChange(new DictionaryChangeStrict<TKey, TValue>(CollectionChangeType.Remove,
                ImmutableDictionary<TKey, TValue>.Empty.Add(keyValue.Key, keyValue.Value)));
        }

        protected override void OnAdd(IKeyValue<TKey, TValue> added)
        {
            _onChange(new DictionaryChangeStrict<TKey, TValue>(CollectionChangeType.Add,
                ImmutableDictionary<TKey, TValue>.Empty.Add(added.Key, added.Value)));
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
        
        public void Dispose()
        {
            DisposableCollector.Dispose();
        }
    }
}