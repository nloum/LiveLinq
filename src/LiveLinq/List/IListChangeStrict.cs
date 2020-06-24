using System.Collections.Generic;

namespace LiveLinq.List
{
    /// <summary>
    /// Represents a list change that will always include the relevant items, even for Remove events.
    /// This is different from <see cref="IListChange{T}"/>, which might not include the relevant items
    /// for Remove events, for the sake of efficiency.
    /// </summary>
    public interface IListChangeStrict<out T> : IListChange<T>
    {
        /// <summary>
        /// A set of changes that are identical to this change, but where each change in the set contains exactly one item.
        /// </summary>
        new IEnumerable<IListChangeStrict<T>> Itemize();
    }
}
