using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveLinq.Core;
using LiveLinq.List;

namespace LiveLinq
{
    using System.Reactive.Linq;

    using MoreCollections;

    public static partial class Extensions
    {
        public class ListChangeWithCount<T>
        {
            public static ListChangeWithCount<T> Empty { get; } = new ListChangeWithCount<T>(
                Utility<T>.EmptyChange,
                0);

            public ListChangeWithCount(IListChange<T> change, int count)
            {
                Change = change;
                Count = count;
            }

            public IListChange<T> Change { get; }
            public int Count { get; }
        }
        
        public class ListChangeStrictWithCount<T>
        {
            public static ListChangeStrictWithCount<T> Empty { get; } = new ListChangeStrictWithCount<T>(
                Utility<T>.EmptyChange,
                0);

            public ListChangeStrictWithCount(IListChangeStrict<T> change, int count)
            {
                Change = change;
                Count = count;
            }

            public IListChangeStrict<T> Change { get; }
            public int Count { get; }
        }

        /// <returns>An event stream where each event in the stream indicates the number of items in the query
        /// at that specific moment in time, and the latest change that caused that number to be what it is.
        /// Note that the first event in this sequence has for its change an empty Add operation,
        /// so that the first count can be zero.</returns>
        public static IObservable<ListChangeWithCount<T>> WithCount<T>(this IObservable<IListChange<T>> source)
        {
            return source.AsObservable().Scan(
                ListChangeWithCount<T>.Empty,
                (state, change) =>
                {
                    switch (change.Type)
                    {
                        case CollectionChangeType.Add:
                            return new ListChangeWithCount<T>(change, state.Count + change.Range.Size);
                        case CollectionChangeType.Remove:
                            return new ListChangeWithCount<T>(change, state.Count - change.Range.Size);
                        default:
                            throw new ArgumentException($"Unknown collection changed type: {change.Type}");
                    }
                });
        }
        
        /// <returns>An event stream where each event in the stream indicates the number of items in the query
        /// at that specific moment in time, and the latest change that caused that number to be what it is.
        /// Note that the first event in this sequence has for its change an empty Add operation,
        /// so that the first count can be zero.</returns>
        public static IObservable<ListChangeStrictWithCount<T>> WithCount<T>(this IObservable<IListChangeStrict<T>> source)
        {
            return source.AsObservable().Scan(
                ListChangeStrictWithCount<T>.Empty,
                (state, change) =>
                {
                    switch (change.Type)
                    {
                        case CollectionChangeType.Add:
                            return new ListChangeStrictWithCount<T>(change, state.Count + change.Range.Size);
                        case CollectionChangeType.Remove:
                            return new ListChangeStrictWithCount<T>(change, state.Count - change.Range.Size);
                        default:
                            throw new ArgumentException($"Unknown collection changed type: {change.Type}");
                    }
                });
        }

        /// <returns>An event stream where each event in the stream indicates the number of items in the query
        /// at that specific moment in time.</returns>
        public static IObservable<int> Count<T>(this IListChanges<T> source)
        {
            return source.AsObservable().WithCount().Select(x => x.Count);
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
