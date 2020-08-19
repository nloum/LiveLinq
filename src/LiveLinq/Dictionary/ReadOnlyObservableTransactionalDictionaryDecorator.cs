using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    public class ObservableTransactionalDictionaryDecorator<TKey, TValue> : IObservableTransactionalDictionary<TKey, TValue>
    {
        private readonly ITransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue>, IDisposableDictionary<TKey, TValue>> _wrapped;
        private readonly bool _fireInitialState;
        private readonly Subject<IDictionaryChangeStrict<TKey, TValue>> _subject = new Subject<IDictionaryChangeStrict<TKey, TValue>>();

        public ObservableTransactionalDictionaryDecorator(ITransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue>, IDisposableDictionary<TKey, TValue>> wrapped, bool fireInitialState)
        {
            _wrapped = wrapped;
            _fireInitialState = fireInitialState;
        }

        public IDisposableReadOnlyDictionary<TKey, TValue> BeginRead()
        {
            return _wrapped.BeginRead();
        }

        public IDisposableDictionary<TKey, TValue> BeginWrite()
        {
            return new ObservableDictionaryDecorator<TKey, TValue>(_wrapped.BeginWrite(), _subject, false, true);
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return Observable.Create<IDictionaryChangeStrict<TKey, TValue>>(observer =>
            {
                if (_fireInitialState)
                {
                    using (var reader = _wrapped.BeginRead())
                    {
                        var items = reader.ToImmutableList();
                        observer.OnNext(Utility.DictionaryAdd(items));
                    }
                }
                var result = _subject.Where(x => x.Values.Count > 0).Subscribe(observer);
                return result;
            }).ToLiveLinq();
        }
    }
}