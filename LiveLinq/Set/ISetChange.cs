using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveLinq.Core;

namespace LiveLinq.Set
{
    public interface ISetChange<out T> : ICollectionChange<T>
    {
        IEnumerable<ISetChange<T>> Itemize();
    }

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
    }
}
