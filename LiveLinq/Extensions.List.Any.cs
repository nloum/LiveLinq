using System;
using System.Reactive.Linq;
using LiveLinq.List;

namespace LiveLinq
{
    public static partial class Extensions
    {
        /// <summary>
        /// Analogous to the LINQ Any extension method. This returns an event stream that will fire a new event every
        /// time a call to LINQ's Any extension method would have a different return value; and the event itself
        /// will be equal to the call to LINQ's Any extension method if it were called at that time. But of course,
        /// because this is LiveLinq, all elements in the query won't be iterated for every event.
        /// </summary>
        public static IObservable<bool> Any<TSource>(
                  this IListChanges<TSource> source)
        {
            return source.Count().Select(count => count > 0);
        }

        /// <summary>
        /// Analogous to the LINQ Any extension method. This returns an event stream that will fire a new event every
        /// time a call to LINQ's Any extension method would have a different return value; and the event itself
        /// will be equal to the call to LINQ's Any extension method if it were called at that time. But of course,
        /// because this is LiveLinq, all elements in the query won't be iterated for every event.
        /// </summary>
        public static IObservable<bool> Any<TSource>(
                  this IListChanges<TSource> source,
                  Func<TSource, IObservable<bool>> selector)
        {
            return source.Where(selector).Count().Select(count => count > 0);
        }

        /// <summary>
        /// Analogous to the LINQ Any extension method. This returns an event stream that will fire a new event every
        /// time a call to LINQ's Any extension method would have a different return value; and the event itself
        /// will be equal to the call to LINQ's Any extension method if it were called at that time. But of course,
        /// because this is LiveLinq, all elements in the query won't be iterated for every event.
        /// </summary>
        public static IObservable<bool> Any<TSource>(
            this IListChanges<TSource> source,
            Func<TSource, IObservable<int>, IObservable<bool>> selector)
        {
            return source.Where(selector).Count().Select(count => count > 0);
        }

        /// <summary>
        /// Analogous to the LINQ Any extension method. This returns an event stream that will fire a new event every
        /// time a call to LINQ's Any extension method would have a different return value; and the event itself
        /// will be equal to the call to LINQ's Any extension method if it were called at that time. But of course,
        /// because this is LiveLinq, all elements in the query won't be iterated for every event.
        /// </summary>
        public static IObservable<bool> Any<TSource>(
            this IListChanges<TSource> source,
            Func<TSource, bool> selector)
        {
            return source.Where(selector).Count().Select(count => count > 0);
        }

        /// <summary>
        /// Analogous to the LINQ Any extension method. This returns an event stream that will fire a new event every
        /// time a call to LINQ's Any extension method would have a different return value; and the event itself
        /// will be equal to the call to LINQ's Any extension method if it were called at that time. But of course,
        /// because this is LiveLinq, all elements in the query won't be iterated for every event.
        /// </summary>
        public static IObservable<bool> Any<TSource>(
            this IListChanges<TSource> source,
            Func<TSource, IObservable<int>, bool> selector)
        {
            return source.Where(selector).Count().Select(count => count > 0);
        }
    }
}
