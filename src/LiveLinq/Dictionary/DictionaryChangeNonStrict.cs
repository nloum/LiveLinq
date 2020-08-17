using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using SimpleMonads;
using ComposableCollections.Dictionary;
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
        public IReadOnlyList<IKeyValue<TKey, TValue>> KeyValuePairs => ImmutableList<IKeyValue<TKey, TValue>>.Empty;

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
            foreach (var item in KeyValuePairs)
            {
                yield return new DictionaryChangeNonStrict<TKey, TValue>(Type, ImmutableList<TKey>.Empty.Add(item.Key));
            }
        }

        public override string ToString()
        {
            var toFrom = Type == CollectionChangeType.Add ? "to" : "from";
            return $"{Type} {Values.Count} values {toFrom} a dictionary (non-strictly)";
        }
    }
}
