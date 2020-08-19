using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    // public class ObservableTransactionalDictionaryWithBuiltInKeyDecorator<TKey, TValue> : IObservableTransactionalDictionaryWithBuiltInKey<TKey, TValue>
    // {
    //     private readonly ITransactionalCollection<IDisposableReadOnlyDictionaryWithBuiltInKey<TKey, TValue>, IDisposableDictionaryWithBuiltInKey<TKey, TValue>> _wrapped;
    //     private readonly bool _fireInitialState;
    //     private readonly Subject<IDictionaryChangeStrict<TKey, TValue>> _subject = new Subject<IDictionaryChangeStrict<TKey, TValue>>();
    //
    //     public ObservableTransactionalDictionaryWithBuiltInKeyDecorator(ITransactionalCollection<IDisposableReadOnlyDictionaryWithBuiltInKey<TKey, TValue>, IDisposableDictionaryWithBuiltInKey<TKey, TValue>> wrapped, bool fireInitialState)
    //     {
    //         _wrapped = wrapped;
    //         _fireInitialState = fireInitialState;
    //     }
    //
    //     public IDisposableReadOnlyDictionaryWithBuiltInKey<TKey, TValue> BeginRead()
    //     {
    //         return _wrapped.BeginRead();
    //     }
    //
    //     public IDisposableDictionaryWithBuiltInKey<TKey, TValue> BeginWrite()
    //     {
    //         return new AnonymousObservableDictionaryWithBuiltInKey<TKey, TValue>(_wrapped.BeginWrite(), _subject, false, true);
    //     }
    //
    //     public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
    //     {
    //         return Observable.Create<IDictionaryChangeStrict<TKey, TValue>>(observer =>
    //         {
    //             if (_fireInitialState)
    //             {
    //                 using (var reader = _wrapped.BeginRead())
    //                 {
    //                     var items = reader.ToImmutableList();
    //                     observer.OnNext(Utility.DictionaryAdd(items));
    //                 }
    //             }
    //             var result = _subject.Where(x => x.Values.Count > 0).Subscribe(observer);
    //             return result;
    //         }).ToLiveLinq();
    //     }
    // }
}