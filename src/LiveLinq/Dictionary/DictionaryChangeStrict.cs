using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using ComposableCollections.Dictionary;
using ComposableCollections.List;
using SimpleMonads;
using LiveLinq.Core;
using static SimpleMonads.Utility;

namespace LiveLinq.Dictionary
{
    [DataContract]
    public class DictionaryChangeStrict<TKey, TValue> : IDictionaryChangeStrict<TKey, TValue>
    {
        private IReadOnlyList<TKey> _keys;
        private IReadOnlyList<TValue> _values;
        private IReadOnlyList<IKeyValue<TKey, TValue>> _keyValuePairs;

        public DictionaryChangeStrict(CollectionChangeType type, ImmutableDictionary<TKey, TValue> items)
        {
            Type = type;
            Dictionary = items;
        }

        [DataMember] public ImmutableDictionary<TKey, TValue> Dictionary { get; set; }

        public IReadOnlyList<TValue> Values
        {
            get
            {
                if (_values == null)
                    _values = new SelectReadOnlyList<IKeyValue<TKey, TValue>, TValue>(KeyValuePairs, (x, _) => x.Value);
                return _values;
            }
        }

        [DataMember] public CollectionChangeType Type { get; set; }

        public IReadOnlyList<TKey> Keys
        {
            get
            {
                if (_keys == null)
                    _keys = new SelectReadOnlyList<IKeyValue<TKey, TValue>, TKey>(KeyValuePairs, (x, _) => x.Key);
                return _keys;
            }
        }

        public bool ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        public IReadOnlyList<IKeyValue<TKey, TValue>> KeyValuePairs
        {
            get
            {
                if (_keyValuePairs == null)
                    _keyValuePairs = Dictionary.Select(x => new KeyValue<TKey, TValue>(x.Key, x.Value)).ToImmutableList();
                return _keyValuePairs;
            }
        }

        public IMaybe<TValue> this[TKey key]
        {
            get
            {
                TValue value;
                if (Dictionary.TryGetValue(key, out value)) return Something(value);

                return Nothing<TValue>();
            }
        }

        IEnumerable<IDictionaryChange<TKey, TValue>> IDictionaryChange<TKey, TValue>.Itemize()
        {
            return Itemize();
        }

        public bool IsEffectivelyStrict => true;

        public IEnumerable<IDictionaryChangeStrict<TKey, TValue>> Itemize()
        {
            foreach (var item in KeyValuePairs)
                yield return new DictionaryChangeStrict<TKey, TValue>(Type,
                    ImmutableDictionary<TKey, TValue>.Empty.Add(item.Key, item.Value));
        }
        
        public override string ToString()
        {
            var toFrom = Type == CollectionChangeType.Add ? "to" : "from";
            return $"{Type} {Values.Count} values {toFrom} a dictionary (strictly)";
        }
    }
}
