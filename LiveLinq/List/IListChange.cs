using System.Collections.Generic;
using GenericNumbers;
using LiveLinq.Core;

namespace LiveLinq.List
{
    /// <summary>
    /// Represents a change to a list. This includes a list of items that were relevant to the change if it was an Add change,
    /// but if it was a Remove change then the items might not be in the <see cref="Items"/> collection, for the sake of
    /// efficiency.
    /// </summary>
    public interface IListChange<out T> : IKeyedCollectionChange<int, T>
    {
        /// <summary>
        /// The range of indices of items that should be effected by this change.
        /// Note that the bounds may be negative, in which case they are to be
        /// added to the length of the list to get the actual index of the item.
        /// Because the bounds may be negative, it's possible that the upper bound
        /// can be negative and the lower bound positive.
        /// </summary>
        INumberRange<int> Range { get; }

        /// <summary>
        /// A set of changes that are identical to this change, but where each change in the set contains exactly one item.
        /// </summary>
        IEnumerable<IListChange<T>> Itemize();
    }
}
