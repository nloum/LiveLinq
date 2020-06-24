using System;
using LiveLinq.List;

namespace LiveLinq.Ordered
{
    /// <summary>
    /// A LiveLinq query that you can call ThenBy on. To get an instance of this type, call OrderBy.
    /// This is identical to the semantics of LINQ's OrderBy, ThenBy, OrderByDescending, and ThenByDescending.
    /// 
    /// Example:
    /// 
    /// var orderedByName = liveLinqQuery.OrderBy(x => x.Name);
    /// var orderedByNameThenByAge = orderedByName.ThenBy(x => x.Age);
    /// 
    /// The purpose of this code is so that you can do things like this:
    /// 
    /// var orderedByNameThenByAge = liveLinqQuery.OrderBy(x => x.Name).ThenBy(x => x.Age);
    /// 
    /// Just like in LINQ.
    /// </summary>
    public interface IOrderedListChanges<T> : IListChangesStrict<T>
    {
        Func<T, T, int> Comparer { get; }
    }
}
