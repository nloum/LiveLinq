using System;

using LiveLinq.Dictionary;
using LiveLinq.List;
using LiveLinq.Ordered;
using LiveLinq.Set;

namespace LiveLinq
{
    public static partial class Extensions
    {
        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public static IListChanges<TSource> Distinct<TSource, TKey>(
            this ISetChanges<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.GroupBy(keySelector)
                .MakeStrictExpensively()
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => kvp.Value.First())
                .Where(m => m.HasValue)
                .Select(m => m.Value);
        }

        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public static IListChanges<TSource> Distinct<TSource, TKey>(
            this ISetChanges<TSource> source,
            Func<TSource, IObservable<TKey>> keySelector)
        {
            return source.GroupBy(keySelector)
                .MakeStrictExpensively()
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => kvp.Value.First())
                .Where(m => m.HasValue)
                .Select(m => m.Value);
        }

        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public static IListChanges<TSource> Distinct<TSource, TKey>(
            this IListChanges<TSource> source,
            Func<TSource, IObservable<int>, IObservable<TKey>> keySelector)
        {
            return source.GroupBy(keySelector)
                .MakeStrictExpensively()
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => kvp.Value.First())
                .Where(m => m.HasValue)
                .Select(m => m.Value);
        }

        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public static IListChanges<TSource> Distinct<TSource, TKey>(
            this IListChanges<TSource> source,
            Func<TSource, IObservable<int>, TKey> keySelector)
        {
            return source.GroupBy(keySelector)
                .MakeStrictExpensively()
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => kvp.Value.First())
                .Where(m => m.HasValue)
                .Select(m => m.Value);
        }
    }
}
