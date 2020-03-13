using System;
using LiveLinq.List;

namespace LiveLinq
{
     public static partial class Extensions
    {
        public static IListChanges<TSource> Intersect<TSource>(this IListChanges<TSource> source, IListChanges<TSource> intersections)
        {
            return source.Where(item => intersections.Contains(item));
        }

        public static IListChanges<TSource> Intersect<TSource, TKey>(this IListChanges<TSource> source, IListChanges<TSource> intersections, Func<TSource, TKey> keySelector)
        {
            var intersectionKeys = intersections.Select(keySelector);
            return source.Where(item => intersectionKeys.Contains(keySelector(item)));
        }

        public static IListChanges<TSource> Intersect<TSource, TKey>(this IListChanges<TSource> source, IListChanges<TSource> intersections, Func<TSource, IObservable<TKey>> keySelector)
        {
            var intersectionKeys = intersections.Select(keySelector);
            return source.Where(item => intersectionKeys.Contains(keySelector(item)));
        }

        public static IListChanges<TSource> Intersect<TSource, TIntersection, TKey>(this IListChanges<TSource> source, Func<TSource, TKey> sourceKeySelector, IListChanges<TIntersection> intersections, Func<TIntersection, TKey> intersectionKeySelector)
        {
            var intersectionKeys = intersections.Select(intersectionKeySelector);
            return source.Where(item => intersectionKeys.Contains(sourceKeySelector(item)));
        }

        public static IListChanges<TSource> Intersect<TSource, TIntersection, TKey>(this IListChanges<TSource> source, Func<TSource, TKey> sourceKeySelector, IListChanges<TIntersection> intersections, Func<TIntersection, IObservable<TKey>> intersectionKeySelector)
        {
            var intersectionKeys = intersections.Select(intersectionKeySelector);
            return source.Where(item => intersectionKeys.Contains(sourceKeySelector(item)));
        }

        public static IListChanges<TSource> Intersect<TSource, TIntersection, TKey>(this IListChanges<TSource> source, Func<TSource, IObservable<TKey>> sourceKeySelector, IListChanges<TIntersection> intersections, Func<TIntersection, TKey> intersectionKeySelector)
        {
            var intersectionKeys = intersections.Select(intersectionKeySelector);
            return source.Where(item => intersectionKeys.Contains(sourceKeySelector(item)));
        }

        public static IListChanges<TSource> Intersect<TSource, TIntersection, TKey>(this IListChanges<TSource> source, Func<TSource, IObservable<TKey>> sourceKeySelector, IListChanges<TIntersection> intersections, Func<TIntersection, IObservable<TKey>> intersectionKeySelector)
        {
            var intersectionKeys = intersections.Select(intersectionKeySelector);
            return source.Where(item => intersectionKeys.Contains(sourceKeySelector(item)));
        }
    }
}