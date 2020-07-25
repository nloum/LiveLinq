using System.Collections.Generic;
using System.Collections.Immutable;
using LiveLinq.Core;

namespace LiveLinq.Set
{
    /// <summary>
    /// Describes a change to a set.
    /// </summary>
    internal class SetChange<T> : ISetChange<T>
    {
        public IReadOnlyList<T> Values { get; }

        public CollectionChangeType Type { get; }

        public virtual bool IsEffectivelyStrict => true;

        public IEnumerable<ISetChange<T>> Itemize()
        {
            foreach(var value in Values)
            {
                yield return Utility.SetChange(Type, value);
            }
        }

        internal SetChange(CollectionChangeType type, ImmutableList<T> values)
        {
            Type = type;
            Values = values;
        }
        
        public override string ToString()
        {
            var toFrom = Type == CollectionChangeType.Add ? "to" : "from";
            return $"{Type} {Values.Count} values {toFrom} a set";
        }
    }
}