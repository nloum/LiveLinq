using System.Collections.Generic;

namespace LiveLinq.Dictionary
{
    public interface IDictionaryChangeStrict<TKey, out TValue> : IDictionaryChange<TKey, TValue>
    {
        /// <summary>
        /// A set of changes that are identical to this change, but where each change in the set contains exactly one item.
        /// </summary>
        new IEnumerable<IDictionaryChangeStrict<TKey, TValue>> Itemize();
    }
}