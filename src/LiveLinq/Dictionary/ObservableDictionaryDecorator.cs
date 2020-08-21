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
        private ISubject<IDictionaryChangeStrict<TKey, TValue>> _subject;
        
        public ObservableDictionaryDecorator(IComposableDictionary<TKey, TValue> state) : this(state, new Subject<IDictionaryChangeStrict<TKey, TValue>>())
        {
        }

        public ObservableDictionaryDecorator(IComposableDictionary<TKey, TValue> state, ISubject<IDictionaryChangeStrict<TKey, TValue>> subject) : base(state, subject)
        {
            _subject = subject;
        }

        protected ObservableDictionaryDecorator()
        {
            var subject = new Subject<IDictionaryChangeStrict<TKey, TValue>>();
            this.DisposableCollector.Disposes(subject);
            _subject = subject;
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
                var result = _subject.Where(x => x.Values.Count > 0).Subscribe(observer);
                return result;
            }).ToLiveLinq();
        }
    }
}