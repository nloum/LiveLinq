using System;
using System.Reactive.Linq;
using LiveLinq.List;

namespace LiveLinq
{
    public static partial class Extensions
    {
        /// <summary>
        /// Analogous to the LINQ All extension method. This returns an event stream that will fire a new event every
        /// time a call to LINQ's All extension method would have a different return value; and the event itself
        /// will be equal to the call to LINQ's All extension method if it were called at that time. But of course,
        /// because this is LiveLinq, all elements in the query won't be iterated for every event.
        /// </summary>
        public static IObservable<bool> All<TSource>(
                 this IListChanges<TSource> source,
                 Func<TSource, IObservable<bool>> selector)
        {
            return source.Where(src => selector(src).Select(result => !result)).Count().Select(count => count > 0);
        }

        /// <summary>
        /// Analogous to the LINQ All extension method. This returns an event stream that will fire a new event every
        /// time a call to LINQ's All extension method would have a different return value; and the event itself
        /// will be equal to the call to LINQ's All extension method if it were called at that time. But of course,
        /// because this is LiveLinq, all elements in the query won't be iterated for every event.
        /// </summary>
        public static IObservable<bool> All<TSource>(
            this IListChanges<TSource> source,
            Func<TSource, IObservable<int>, IObservable<bool>> selector)
        {
            return source.Where((src, idx) => selector(src, idx).Select(result => !result)).Count().Select(count => count > 0);
        }

        /// <summary>
        /// Analogous to the LINQ All extension method. This returns an event stream that will fire a new event every
        /// time a call to LINQ's All extension method would have a different return value; and the event itself
        /// will be equal to the call to LINQ's All extension method if it were called at that time. But of course,
        /// because this is LiveLinq, all elements in the query won't be iterated for every event.
        /// </summary>
        public static IObservable<bool> All<TSource>(
            this IListChanges<TSource> source,
            Func<TSource, bool> selector)
        {
            return source.Where(src => !selector(src)).Count().Select(count => count > 0);
        }

        /// <summary>
        /// Analogous to the LINQ All extension method. This returns an event stream that will fire a new event every
        /// time a call to LINQ's All extension method would have a different return value; and the event itself
        /// will be equal to the call to LINQ's All extension method if it were called at that time. But of course,
        /// because this is LiveLinq, all elements in the query won't be iterated for every event.
        /// </summary>
        public static IObservable<bool> All<TSource>(
            this IListChanges<TSource> source,
            Func<TSource, IObservable<int>, bool> selector)
        {
            return source.Where((src, idx) => !selector(src, idx)).Count().Select(count => count > 0);
        }
    }
}
