using System;
using System.Collections;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using LiveLinq.Dictionary;
using LiveLinq.List;
using LiveLinq.Set;
using UtilityDisposables;
using SimpleMonads;

using MoreCollections;

using static SimpleMonads.Utility;

namespace LiveLinq
{
    public static partial class Extensions
    {
        public static IListChangesStrict<T> SubscribeOn<T>(this IListChangesStrict<T> source, IScheduler scheduler)
        {
            return source.AsObservable().SubscribeOn(scheduler).ToLiveLinq();
        }

        public static IListChangesStrict<T> ObserveOn<T>(this IListChangesStrict<T> source, IScheduler scheduler)
        {
            return source.AsObservable().ObserveOn(scheduler).ToLiveLinq();
        }

        public static IListChanges<T> SubscribeOn<T>(this IListChanges<T> source, IScheduler scheduler)
        {
            return source.AsObservable().SubscribeOn(scheduler).ToLiveLinq();
        }

        public static IListChanges<T> ObserveOn<T>(this IListChanges<T> source, IScheduler scheduler)
        {
            return source.AsObservable().ObserveOn(scheduler).ToLiveLinq();
        }

        public static IDictionaryChangesStrict<TKey, TValue> SubscribeOn<TKey, TValue>(this IDictionaryChangesStrict<TKey, TValue> source, IScheduler scheduler)
        {
            return source.AsObservable().SubscribeOn(scheduler).ToLiveLinq();
        }

        public static IDictionaryChangesStrict<TKey, TValue> ObserveOn<TKey, TValue>(this IDictionaryChangesStrict<TKey, TValue> source, IScheduler scheduler)
        {
            return source.AsObservable().ObserveOn(scheduler).ToLiveLinq();
        }

        public static IDictionaryChanges<TKey, TValue> SubscribeOn<TKey, TValue>(this IDictionaryChanges<TKey, TValue> source, IScheduler scheduler)
        {
            return source.AsObservable().SubscribeOn(scheduler).ToLiveLinq();
        }

        public static IDictionaryChanges<TKey, TValue> ObserveOn<TKey, TValue>(this IDictionaryChanges<TKey, TValue> source, IScheduler scheduler)
        {
            return source.AsObservable().ObserveOn(scheduler).ToLiveLinq();
        }
        
        public static ISetChanges<T> SubscribeOn<T>(this ISetChanges<T> source, IScheduler scheduler)
        {
            return source.AsObservable().SubscribeOn(scheduler).ToLiveLinq();
        }

        public static ISetChanges<T> ObserveOn<T>(this ISetChanges<T> source, IScheduler scheduler)
        {
            return source.AsObservable().ObserveOn(scheduler).ToLiveLinq();
        }

        /// <summary>
        /// Like the normal Reactive Extensions Subscribe method, except the OnCompleteOrUnsubscribe callback will be called when the event stream is unsubscribed from
        /// or the event stream is completed, and if there was at least one event.
        /// </summary>
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError,
            Action<T> onCompleteOrUnsubscribe)
        {
            return source.Subscribe(onNext, onError, m =>
            {
                if (m.HasValue) onCompleteOrUnsubscribe(m.Value);
            }, m =>
            {
                if (m.HasValue) onCompleteOrUnsubscribe(m.Value);
            });
        }
        
        /// <summary>
        /// Like the normal Reactive Extensions Subscribe method, except the OnComplete callback will only be called if there was at least
        /// one event, and its one parameter will be the last event.
        /// </summary>
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError,
            Action<IMaybe<T>> onComplete, Action<IMaybe<T>> onUnsubscribe)
        {
            return source.Subscribe(onNext, (exception, last) => onError(exception), onComplete, onUnsubscribe);
        }

        /// <summary>
        /// Like the normal Reactive Extensions Subscribe method, except the OnComplete callback will only be called if there was at least
        /// one event, and its one parameter will be the last event.
        /// </summary>
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception, IMaybe<T>> onError,
            Action<IMaybe<T>> onComplete, Action<IMaybe<T>> onUnsubscribe)
        {
            var last = Nothing<T>();

            return source.Do(t =>
            {
                last = Something(t);
                onNext(t);
            }, exception => onError(exception, last))
            .TakeLast(1)
            .Subscribe(_ => { }, exception => onError(exception, last), () => onComplete(last))
            .DisposeWith(new AnonymousDisposable(() => onUnsubscribe(last)));
        }

        public static IObservable<T> OnUnsubscribe<T>(this IObservable<T> source, Action<IMaybe<T>> onUnsubscribe)
        {
            return Observable.Create<T>(observer =>
            {
                return source.Subscribe(observer.OnNext, observer.OnError, _ => { }, onUnsubscribe);
            });
        }

        /// <summary>
        ///     A fluent way to call Observable.Return.
        /// </summary>
        public static IObservable<T> ObserveReturn<T>(this T source)
        {
            return Observable.Return(source);
        }
        
        /// <summary>
        ///     Returns an event stream where each event is an array that has up to count number of elements.
        ///     The elements are the last events that occurred on observable.
        ///     An empty array is never produced as an event by the resulting event stream.
        ///     Resulting events may contain less than count events until count is reached.
        ///     The first element in the array is the oldest, the last element in the array is the newest.
        /// </summary>
        public static IObservable<T[]> CollectLast<T>(this IObservable<T> observable, int count)
        {
            return observable.Scan(
                new T[0],
                (array, element) =>
                    {
                        if (array.Length < count)
                        {
                            var newArray = new T[array.Length + 1];
                            Array.Copy(array, newArray, array.Length);
                            newArray[array.Length] = element;
                            return newArray;
                        }
                        else
                        {
                            var newArray = new T[count];
                            Array.Copy(array, 1, newArray, 0, count - 1);
                            newArray[count - 1] = element;
                            return newArray;
                        }
                    });
        }

        /// <summary>
        /// Get events from the most recent source event. So for instance, if source an
        /// <see cref="IObservable{IObservable{string}}"/>, then this will only fire
        /// events from the most recent <see cref="IObservable{string}"/>.
        /// </summary>
        public static IObservable<T2> SelectLatest<T1, T2>(this IObservable<T1> source,
            Func<T1, IObservable<T2>> selector)
        {
            return source.Select(selector).SelectLatest();
        }

        /// <summary>
        /// Get events from the most recent source event. So for instance, if source an
        /// <see cref="IObservable{IObservable{string}}"/>, then this will only fire
        /// events from the most recent <see cref="IObservable{string}"/>.
        /// </summary>
        public static IObservable<T> SelectLatest<T>(this IObservable<IObservable<T>> source)
        {
            return
                Observable.Create<T>(
                    observer =>
                        {
                            return
                                source.Select(o => o.Subscribe(observer.OnNext, observer.OnError))
                                    .DisposeAllPrevious()
                                    .Subscribe(_ => { }, observer.OnError, observer.OnCompleted);
                        });
        }
        
        /// <summary>
        ///     Simply selects System.Reactive.Unit.Default for every item. This basically strips
        ///     the event stream of all type information and the actual event objects, replacing them
        ///     with <see cref="Unit" /> objects. This can be handy when combining event streams of
        ///     different types, when you don't care about the actual event objects.
        /// </summary>
        public static IObservable<Unit> SelectUnit<T>(this IObservable<T> source)
        {
            return source.Select(_ => Unit.Default);
        }

        #region Subscribe

        /// <summary>
        ///     Same as the main Subscribe method, but without a parameter for onNext
        /// </summary>
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action onNext)
        {
            if (onNext == null)
            {
                throw new NullReferenceException(nameof(onNext));
            }
            return source.Subscribe(_ => onNext());
        }

        /// <summary>
        ///     Same as the main Subscribe method, but without a parameter for onNext or onError
        /// </summary>
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action onNext, Action onError)
        {
            if (onNext == null)
            {
                throw new NullReferenceException(nameof(onNext));
            }
            if (onError == null)
            {
                throw new NullReferenceException(nameof(onError));
            }
            return source.Subscribe(_ => onNext(), _ => onError());
        }

        /// <summary>
        ///     Same as the main Subscribe method, but without a parameter for onNext or onError
        /// </summary>
        public static IDisposable Subscribe<T>(
            this IObservable<T> source,
            Action onNext,
            Action onError,
            Action onCompleted)
        {
            if (onNext == null)
            {
                throw new NullReferenceException(nameof(onNext));
            }
            if (onError == null)
            {
                throw new NullReferenceException(nameof(onError));
            }
            if (onCompleted == null)
            {
                throw new NullReferenceException(nameof(onCompleted));
            }
            return source.Subscribe(_ => onNext(), _ => onError(), onCompleted);
        }

        #endregion

        #region DoPrevious

        public static IObservable<T> DisposeAllPrevious<T>(this IObservable<T> source) where T : IDisposable
        {
            return source.DoPrevious(x => x.Dispose(), true, true);
        }

        /// <summary>
        ///     Same as the Do extension method, but operates only on the value that is being superseded by the latest event in the
        ///     stream.
        /// </summary>
        public static IObservable<T> DoPrevious<T>(this IObservable<T> source, Action<T> onNext, bool onError, bool onComplete)
        {
            return source.DoPrevious(
                onNext,
                (maybeLast, _) =>
                {
                    if (onError && maybeLast.HasValue)
                        onNext(maybeLast.Value);
                },
                maybeLast =>
                {
                    if (onComplete && maybeLast.HasValue)
                        onNext(maybeLast.Value);
                });
        }

        /// <summary>
        ///     Same as the Do extension method, but operates only on the value that is being superseded by the latest event in the
        ///     stream.
        /// </summary>
        public static IObservable<T> DoPrevious<T>(
            this IObservable<T> source,
            Action<T> onNext,
            Action<Exception> onError)
        {
            return source.DoPrevious(onNext, onError, () => { });
        }

        /// <summary>
        ///     Same as the Do extension method, but operates only on the value that is being superseded by the latest event in the
        ///     stream.
        /// </summary>
        public static IObservable<T> DoPrevious<T>(
            this IObservable<T> source,
            Action<T> onNext,
            Action<Exception> onError,
            Action onCompleted)
        {
            return source.DoPrevious(onNext, (_, ex) => onError(ex), _ => onCompleted());
        }

        /// <summary>
        ///     Same as the Do extension method, but operates only on the value that is being superseded by the latest event in the
        ///     stream.
        /// </summary>
        public static IObservable<T> DoPrevious<T>(
            this IObservable<T> source,
            Action<T> onNext,
            Action<IMaybe<T>, Exception> onError,
            Action<IMaybe<T>> onCompleted)
        {
            var lastValue = Nothing<T>();
            return source.Scan(
                EnumerableUtility.EmptyArray<T>(),
                (elements, element) =>
                    {
                        lastValue = Something(element);
                        if (elements.Length == 0)
                        {
                            return new[] { element };
                        }
                        if (elements.Length == 1)
                        {
                            return new[] { elements[0], element };
                        }
                        return new[] { elements[1], element };
                    }).Do(
                        t =>
                            {
                                if (t.Length == 2)
                                {
                                    onNext(t[0]);
                                }
                            },
                        ex => onError(lastValue, ex),
                        () => onCompleted(lastValue))
                    .Select(ts => ts[ts.Length - 1]);
        }

        #endregion
    }
}
