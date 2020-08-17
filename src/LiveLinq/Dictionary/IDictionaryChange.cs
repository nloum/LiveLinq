using System.Collections.Generic;
using SimpleMonads;
using ComposableCollections.Dictionary;
using LiveLinq.Core;

namespace LiveLinq.Dictionary
{
    public interface IDictionaryChange<TKey, out TValue> : IKeyedCollectionChange<TKey, TValue>
    {
        /// <summary>
        /// Represents the key/value pairs being added or removed
        /// </summary>
        IReadOnlyList<IKeyValue<TKey, TValue>> KeyValuePairs { get; }

        /// <summary>
        /// Get the value at the specified index.
        /// </summary>
        IMaybe<TValue> this[TKey key] { get; }

        /// <summary>
        /// A set of changes that are identical to this change, but where each change in the set contains exactly one item.
        /// </summary>
        IEnumerable<IDictionaryChange<TKey, TValue>> Itemize();
    }
}
