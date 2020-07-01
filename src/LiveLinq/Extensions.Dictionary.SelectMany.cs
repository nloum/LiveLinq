using System;
using System.Collections.Immutable;
using System.Reactive.Linq;
using LiveLinq.Dictionary;
using LiveLinq.Ordered;
using LiveLinq.List;
using LiveLinq.Set;

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

        public static ISetChanges<TResult> SelectMany<TKeySource, TValueSource, TResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TValueSource, ISetChanges<TResult>> selector)
        {
            return source
                .KeyValuePairsAsSet()
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
                (key, value) =>
                {
                    var result = new CachedState<TKeyResult, TValueResult>();
                    result.Disposable = selector(key, value).ToObservableStateAndChange()
                        .Subscribe(x =>
                        {
                            result.State = x.State;
                            observer.OnNext(x.MostRecentChange);
                        }, observer.OnError);
                    return result;
                },
                (key, value, cachedState) =>
                {
                    cachedState.Disposable.Dispose();
                    observer.OnNext(Utility.DictionaryRemove(cachedState.State));
                });
        }

        private static IDisposable SelectManySubscribe<TKeySource, TValueSource, TKeyResult, TValueResult>(
            IDictionaryChanges<TKeySource, TValueSource> source,
            IObserver<IDictionaryChangeStrict<TKeyResult, TValueResult>> observer,
            Func<TKeySource, TValueSource, IDictionaryChangesStrict<TKeyResult, TValueResult>> selector)
        {
            return source.Subscribe(
                (key, value) =>
                {
                    var result = new CachedState<TKeyResult, TValueResult>();
                    result.Disposable = selector(key, value).ToObservableStateAndChange()
                        .Subscribe(x =>
                        {
                            result.State = x.State;
                            observer.OnNext(x.MostRecentChange);
                        }, observer.OnError);
                    return result;
                },
                (key, value, cachedState) =>
                {
                    cachedState.Disposable.Dispose();
                    // Sometimes the SelectMany selector never returns before we have to remove the SelectMany parent.
                    // In that case, cachedState.State will be null and there's nothing that needs to be removed.
                    if (cachedState.State != null)
                    {
                        observer.OnNext(Utility.DictionaryRemove(cachedState.State));
                    }
                });
        }

        private class CachedState<TKey, TValue>
        {
            public ImmutableDictionary<TKey, TValue> State { get; set; }
            public IDisposable Disposable { get; set; }

        }
    }
}
