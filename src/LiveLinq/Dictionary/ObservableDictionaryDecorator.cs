using System;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using LiveLinq.Core;
using ComposableCollections.Dictionary;

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
    public class ObservableDictionaryDecorator<TKey, TValue> : ObservableDictionaryDecoratorBase<TKey, TValue>,
        IObservableDictionary<TKey, TValue>
    {
        private readonly IObservable<IDictionaryChangeStrict<TKey, TValue>> _observable;
        
        public ObservableDictionaryDecorator(IComposableDictionary<TKey, TValue> state) : this(state, new Subject<IDictionaryChangeStrict<TKey, TValue>>())
        {
        }

        public ObservableDictionaryDecorator(IComposableDictionary<TKey, TValue> state, Subject<IDictionaryChangeStrict<TKey, TValue>> subject) : this(state, subject, subject.OnNext)
        {
            DisposableCollector.Disposes(subject);
        }
        
        public ObservableDictionaryDecorator(IComposableDictionary<TKey, TValue> state, IObservable<IDictionaryChangeStrict<TKey, TValue>> observable, Action<IDictionaryChangeStrict<TKey, TValue>> onChange) : base(state, onChange)
        {
            _observable = observable;
        }

        protected ObservableDictionaryDecorator()
        {
            var subject = new Subject<IDictionaryChangeStrict<TKey, TValue>>();
            DisposableCollector.Disposes(subject);
            _observable = subject;
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
    }
}