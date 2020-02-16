using System;
using System.Reactive.Linq;
using static LiveLinq.List.Utility;

namespace LiveLinq.List
{
    public static partial class Extensions
    {
        #region Non-strict

        public static IListChanges<TResult> Select<TSource, TResult>(
            this IListChanges<TSource> source,
            Func<TSource, IObservable<TResult>> selector)
        {
            return source.SelectMany(src => selector(src).ToLiveLinq());
        }

        public static IListChanges<TResult> Select<TSource, TResult>(
            this IListChanges<TSource> source,
            Func<TSource, IObservable<int>, IObservable<TResult>> selector)
        {
            return source.SelectMany((src, index) => selector(src, index).ToLiveLinq());
        }

        public static IListChanges<TResult> Select<TSource, TResult>(
            this IListChanges<TSource> source,
            Func<TSource, TResult> selector)
        {
            return source.SelectMany(src => Observable.Return(selector(src)).ToLiveLinq());
        }

        public static IListChanges<TResult> Select<TSource, TResult>(
            this IListChanges<TSource> source,
            Func<TSource, IObservable<int>, TResult> selector)
        {
            return source.SelectMany((src, index) => Observable.Return(selector(src, index)).ToLiveLinq());
        }

        #endregion

        #region Strict

        public static IListChanges<TResult> Select<TSource, TResult>(
            this IListChangesStrict<TSource> source,
            Func<TSource, IObservable<TResult>> selector)
        {
            return source.SelectMany(src => selector(src).ToLiveLinq());
        }

        public static IListChanges<TResult> Select<TSource, TResult>(
            this IListChangesStrict<TSource> source,
            Func<TSource, IObservable<int>, IObservable<TResult>> selector)
        {
            return source.SelectMany((src, index) => selector(src, index).ToLiveLinq());
        }

        public static IListChangesStrict<TResult> Select<TSource, TResult>(
            this IListChangesStrict<TSource> source,
            Func<TSource, TResult> selector)
        {
            return source
                .SelectMany(src => Observable.Return(selector(src)).ToLiveLinq())
                .AsObservable()
                .Select(lc => ListChangeStrict(lc.Type, lc.Range, lc.Values))
                .ToLiveLinq();
        }

        public static IListChangesStrict<TResult> Select<TSource, TResult>(
            this IListChangesStrict<TSource> source,
            Func<TSource, IObservable<int>, TResult> selector)
        {
            return source
                .SelectMany((src, index) => Observable.Return(selector(src, index)).ToLiveLinq())
                .AsObservable()
                .Select(lc => ListChangeStrict(lc.Type, lc.Range, lc.Values))
                .ToLiveLinq();
        }

        #endregion
    }
}
