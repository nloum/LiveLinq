using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveLinq.Core;

namespace LiveLinq.List
{
    using System.Reactive.Linq;

    using MoreCollections;

    public static partial class Extensions
    {
        /// <returns>An event stream where each event in the stream indicates the number of items in the query
        /// at that specific moment in time.</returns>
        public static IObservable<int> Count<T>(this IListChanges<T> source)
        {
            return Observable.Return(0).Concat(source.AsObservable().Scan(
                0,
                (count, change) =>
                {
                    switch (change.Type)
                    {
                        case CollectionChangeType.Add:
                            return count + change.Range.Size;
                        case CollectionChangeType.Remove:
                            return count - change.Range.Size;
                        default:
                            throw new ArgumentException();
                    }
                }));
        }

        /// <returns>An event stream where each event in the stream indicates the number of items
        /// that meet the criteria specified by <see cref="selector"/> at that specific moment in time.</returns>
        public static IObservable<int> Count<TSource>(
              this IListChanges<TSource> source,
              Func<TSource, IObservable<bool>> selector)
        {
            return source.Where(selector).Count();
        }

        /// <returns>An event stream where each event in the stream indicates the number of items
        /// that meet the criteria specified by <see cref="selector"/> at that specific moment in time.</returns>
        public static IObservable<int> Count<TSource>(
            this IListChanges<TSource> source,
            Func<TSource, IObservable<int>, IObservable<bool>> selector)
        {
            return source.Where(selector).Count();
        }

        /// <returns>An event stream where each event in the stream indicates the number of items
        /// that meet the criteria specified by <see cref="selector"/> at that specific moment in time.</returns>
        public static IObservable<int> Count<TSource>(
            this IListChanges<TSource> source,
            Func<TSource, bool> selector)
        {
            return source.Where(selector).Count();
        }

        /// <returns>An event stream where each event in the stream indicates the number of items
        /// that meet the criteria specified by <see cref="selector"/> at that specific moment in time.</returns>
        public static IObservable<int> Count<TSource>(
            this IListChanges<TSource> source,
            Func<TSource, IObservable<int>, bool> selector)
        {
            return source.Where(selector).Count();
        }
    }
}
