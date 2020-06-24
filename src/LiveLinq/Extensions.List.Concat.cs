using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveLinq.List;

namespace LiveLinq
{
    using System.Reactive.Linq;

    public static partial class Extensions
    {
        /// <summary>
        /// Analogous to the LINQ Concat extension method. This returns a LiveLinq query that consists of the source
        /// and rest queries all concatenated together, in the order that they are specified.
        /// </summary>
        public static IListChanges<T> Concat<T>(this IListChanges<T> source, params IListChanges<T>[] rest)
        {
            var collections = new List<IListChanges<T>>();
            collections.Add(source);
            collections.AddRange(rest);
            return Observable.Return(collections).Select(coll => coll.AsEnumerable()).ToLiveLinq().SelectMany(x => x);
        }
    }
}
