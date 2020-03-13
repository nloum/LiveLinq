using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveLinq.List;

namespace LiveLinq
{
    public static partial class Extensions
    {
        public static IListChanges<TSource> Except<TSource>(this IListChanges<TSource> source, IListChanges<TSource> exceptions)
        {
            return source.Where(item => exceptions.Contains(item).Select(b => !b));
        }

        public static IListChanges<TSource> Except<TSource, TKey>(this IListChanges<TSource> source, IListChanges<TSource> exceptions, Func<TSource, TKey> keySelector)
        {
            var exceptionKeys = exceptions.Select(keySelector);
            return source.Where(item => exceptionKeys.Contains(keySelector(item)).Select(b => !b));
        }

        public static IListChanges<TSource> Except<TSource, TKey>(this IListChanges<TSource> source, IListChanges<TSource> exceptions, Func<TSource, IObservable<TKey>> keySelector)
        {
            var exceptionKeys = exceptions.Select(keySelector);
            return source.Where(item => exceptionKeys.Contains(keySelector(item)).Select(b => !b));
        }

        public static IListChanges<TSource> Except<TSource, TException, TKey>(this IListChanges<TSource> source, Func<TSource, TKey> sourceKeySelector, IListChanges<TException> exceptions, Func<TException, TKey> exceptionKeySelector)
        {
            var exceptionKeys = exceptions.Select(exceptionKeySelector);
            return source.Where(item => exceptionKeys.Contains(sourceKeySelector(item)).Select(b => !b));
        }

        public static IListChanges<TSource> Except<TSource, TException, TKey>(this IListChanges<TSource> source, Func<TSource, TKey> sourceKeySelector, IListChanges<TException> exceptions, Func<TException, IObservable<TKey>> exceptionKeySelector)
        {
            var exceptionKeys = exceptions.Select(exceptionKeySelector);
            return source.Where(item => exceptionKeys.Contains(sourceKeySelector(item)).Select(b => !b));
        }

        public static IListChanges<TSource> Except<TSource, TException, TKey>(this IListChanges<TSource> source, Func<TSource, IObservable<TKey>> sourceKeySelector, IListChanges<TException> exceptions, Func<TException, TKey> exceptionKeySelector)
        {
            var exceptionKeys = exceptions.Select(exceptionKeySelector);
            return source.Where(item => exceptionKeys.Contains(sourceKeySelector(item)).Select(b => !b));
        }

        public static IListChanges<TSource> Except<TSource, TException, TKey>(this IListChanges<TSource> source, Func<TSource, IObservable<TKey>> sourceKeySelector, IListChanges<TException> exceptions, Func<TException, IObservable<TKey>> exceptionKeySelector)
        {
            var exceptionKeys = exceptions.Select(exceptionKeySelector);
            return source.Where(item => exceptionKeys.Contains(sourceKeySelector(item)).Select(b => !b));
        }
    }
}
