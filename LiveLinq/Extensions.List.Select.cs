using System;
using System.Reactive.Linq;
using LiveLinq.List;
using static LiveLinq.Utility;

namespace LiveLinq
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

        /// <remarks>
        ///Note: when a list with 2 items has the first item removed, the only change event that is fired is the removal
        /// of the first item. There is no change event explicitly indicating that the index of the second item changed from
        /// 1 to 0, because supporting that feature would be extremely performance intensive. Therefore, the <see cref="IObservable{int}"/>
        /// parameter to the selector should be used with caution.
        /// </remarks>
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

        /// <remarks>
        ///Note: when a list with 2 items has the first item removed, the only change event that is fired is the removal
        /// of the first item. There is no change event explicitly indicating that the index of the second item changed from
        /// 1 to 0, because supporting that feature would be extremely performance intensive. Therefore, the <see cref="IObservable{int}"/>
        /// parameter to the selector should be used with caution.
        /// </remarks>
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

        /// <remarks>
        ///Note: when a list with 2 items has the first item removed, the only change event that is fired is the removal
        /// of the first item. There is no change event explicitly indicating that the index of the second item changed from
        /// 1 to 0, because supporting that feature would be extremely performance intensive. Therefore, the <see cref="IObservable{int}"/>
        /// parameter to the selector should be used with caution.
        /// </remarks>
        public static IListChangesStrict<TResult> Select<TSource, TResult>(
            this IListChangesStrict<TSource> source,
            Func<TSource, int, TResult> selector)
        {
            return source
                .SelectMany((src, index) => index.Select(idx => selector(src, idx)).Take(1).ToLiveLinq())
                .AsObservable()
                .Select(lc => ListChangeStrict(lc.Type, lc.Range, lc.Values))
                .ToLiveLinq();
        }

        #endregion
    }
}
