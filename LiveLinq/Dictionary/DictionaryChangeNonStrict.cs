using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using SimpleMonads;
using MoreCollections;
using LiveLinq.Core;

namespace LiveLinq.Dictionary
{
    [DataContract]
    public class DictionaryChangeNonStrict<TKey, TValue> : IDictionaryChange<TKey, TValue>
    {
        [DataMember]
        public CollectionChangeType Type { get; set; }
        [DataMember]
        public IReadOnlyList<TKey> Keys { get; set; }
        public IReadOnlyList<TValue> Values => ImmutableList<TValue>.Empty;
        public IReadOnlyList<IKeyValuePair<TKey, TValue>> Items => ImmutableList<IKeyValuePair<TKey, TValue>>.Empty;

        public bool IsEffectivelyStrict => false;

        public IMaybe<TValue> this[TKey key] => SimpleMonads.Utility.Nothing<TValue>();

        public DictionaryChangeNonStrict(CollectionChangeType type, ImmutableList<TKey> keys)
        {
            Keys = keys;
            Type = type;
        }

        public bool ContainsKey(TKey key)
        {
            return Keys.Contains(key);
        }

        public IEnumerable<IDictionaryChange<TKey, TValue>> Itemize()
        {
            foreach (var item in Items)
            {
                yield return new DictionaryChangeNonStrict<TKey, TValue>(Type, ImmutableList<TKey>.Empty.Add(item.Key));
            }
        }
    }
}
