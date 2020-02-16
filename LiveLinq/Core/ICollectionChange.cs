using System.Collections.Generic;

namespace LiveLinq.Core
{
    /// <summary>
    /// Represents a single change to a collection.
    /// </summary>
    public interface ICollectionChange<out TValue>
    {
        /// <summary>
        /// A list of items that changed. If this is a remove change, then this list may be empty.
        /// </summary>
        IReadOnlyList<TValue> Values { get; }

        /// <summary>
        /// The type of change this object represents.
        /// </summary>
        CollectionChangeType Type { get; }

        /// <summary>
        /// Returns true if the Items read-only list is non-empty.
        /// </summary>
        bool IsEffectivelyStrict { get; }
    }

    /// <summary>
    /// Represents a single change to a collection, for collections that support indexing. There are specializations of this interface for dictionaries and lists.
    /// </summary>
    public interface IKeyedCollectionChange<TKey, out TValue> : ICollectionChange<TValue>
    {
        /// <summary>
        /// A list of keys (i.e., indices into a list or keys for a dictionary) of items that changed.
        /// </summary>
        IReadOnlyList<TKey> Keys { get; }

        /// <summary>
        /// Returns true if the Keys property contains the specified key; false otherwise.
        /// </summary>
        bool ContainsKey(TKey key);
    }
}