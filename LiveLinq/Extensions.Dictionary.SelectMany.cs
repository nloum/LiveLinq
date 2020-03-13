using System;
using System.Reactive.Linq;
using LiveLinq.Dictionary;
using LiveLinq.Ordered;
using LiveLinq.List;

namespace LiveLinq
{
    public partial class Extensions
    {
        public static IListChanges<TResult> SelectMany<TKeySource, TValueSource, TResult>(
                    this IDictionaryChanges<TKeySource, TValueSource> source,
                    Func<TKeySource, TValueSource, IListChanges<TResult>> selector)
        {
            return source
                .MakeStrictExpensively()
                .OrderBy(x => x.Key)
                .SelectMany(kvp => selector(kvp.Key, kvp.Value));
        }
        
        public static IDictionaryChanges<TKeyResult, TValueResult> SelectMany<TKeySource, TValueSource, TKeyResult, TValueResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TValueSource, IDictionaryChanges<TKeyResult, TValueResult>> selector)
        {
            var observable = Observable.Create<IDictionaryChange<TKeyResult, TValueResult>>(observer => SelectManySubscribe(source, observer, selector));
            return observable.ToLiveLinq();
        }

        public static IDictionaryChangesStrict<TKeyResult, TValueResult> SelectMany<TKeySource, TValueSource, TKeyResult, TValueResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TValueSource, IDictionaryChangesStrict<TKeyResult, TValueResult>> selector)
        {
            var observable = Observable.Create<IDictionaryChangeStrict<TKeyResult, TValueResult>>(observer => SelectManySubscribe(source, observer, selector));
            return observable.ToLiveLinq();
        }

        private static IDisposable SelectManySubscribe<TKeySource, TValueSource, TKeyResult, TValueResult>(
            IDictionaryChanges<TKeySource, TValueSource> source,
            IObserver<IDictionaryChange<TKeyResult, TValueResult>> observer,
            Func<TKeySource, TValueSource, IDictionaryChanges<TKeyResult, TValueResult>> selector)
        {
            return source.Subscribe(
                (key, value) => selector(key, value).AsObservable().Subscribe(observer.OnNext, observer.OnError),
                (key, value, subscription) => subscription.Dispose());
        }

        private static IDisposable SelectManySubscribe<TKeySource, TValueSource, TKeyResult, TValueResult>(
            IDictionaryChanges<TKeySource, TValueSource> source,
            IObserver<IDictionaryChangeStrict<TKeyResult, TValueResult>> observer,
            Func<TKeySource, TValueSource, IDictionaryChangesStrict<TKeyResult, TValueResult>> selector)
        {
            return source.Subscribe(
                (key, value) => selector(key, value).AsObservable().Subscribe(observer.OnNext, observer.OnError),
                (key, value, subscription) => subscription.Dispose());
        }
    }
}
