using System;

using LiveLinq.Dictionary;
using LiveLinq.List;
using LiveLinq.Ordered;

namespace LiveLinq
{
    public static partial class Extensions
    {
        public static IListChanges<TSource> Distinct<TSource, TKey>(
            this IListChanges<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.GroupBy(keySelector)
                .MakeStrictExpensively()
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => kvp.Value.First())
                .Where(m => m.HasValue)
                .Select(m => m.Value);
        }

        public static IListChanges<TSource> Distinct<TSource, TKey>(
            this IListChanges<TSource> source,
            Func<TSource, IObservable<TKey>> keySelector)
        {
            return source.GroupBy(keySelector)
                .MakeStrictExpensively()
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => kvp.Value.First())
                .Where(m => m.HasValue)
                .Select(m => m.Value);
        }

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
