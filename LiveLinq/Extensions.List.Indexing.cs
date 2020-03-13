using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveLinq.List;
using SimpleMonads;

namespace LiveLinq
{
    public static partial class Extensions
    {
        /// <summary>
        /// This function is analogous to the First LINQ extension method, except it returns
        /// an event stream of values as the first element in the source list changes, and it
        /// returns a Nothing event if the source list becomes empty.
        /// </summary>
        public static IObservable<IMaybe<T>> First<T>(this IListChanges<T> source)
        {
            return source[0];
        }

        #region Contains

        /// <summary>
        /// Similar to the Contains method on an IEnumerable, except this method returns an event stream
        /// that fires a new event every time the contained item is removed or added again.
        /// </summary>
        public static IObservable<bool> Contains<T>(this IListChanges<T> source, T item)
        {
            return Observable.Return(false).Concat(source.Where(t =>
                t.Equals(item)).Count().Select(count =>
                    count > 0)).DistinctUntilChanged();
        }

        /// <summary>
        /// Similar to the Contains method on an IEnumerable, except this method returns an event stream
        /// that fires a new event every time the contained item is removed or added again.
        /// </summary>
        /// <param name="item">The event stream of items that you are interested in whether source contains them.</param>
        public static IObservable<bool> Contains<T>(this IListChanges<T> source, IObservable<T> item)
        {
            return
                source.Where(t => item.Select(test => test.Equals(item)))
                    .Count()
                    .Select(count => count > 0)
                    .DistinctUntilChanged();
        }

        #endregion
    }
}
