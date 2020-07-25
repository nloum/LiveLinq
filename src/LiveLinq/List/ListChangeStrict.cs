using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using GenericNumbers;
using LiveLinq.Core;

namespace LiveLinq.List
{
    internal class ListChangeStrict<T> : ListChange<T>, IListChangeStrict<T>
    {
        /// <summary>
        /// Creates an object that represents an add operation.
        /// </summary>
        internal ListChangeStrict(CollectionChangeType type, INumberRange<int> range, ImmutableList<T> values)
            : base(type, range, values)
        {
        }

        public override bool IsEffectivelyStrict => true;

        new public IEnumerable<IListChangeStrict<T>> Itemize()
        {
            if (Range.Size == 0) yield break;
            else if (Range.Size == 1) yield return this;
            else
            {
                if (Type == CollectionChangeType.Add)
                {
                    for (var i = 0; i < Keys.Count; i++)
                    {
                        yield return Utility.ListChangeStrict(Type, i + Range.LowerBound.ChangeStrictness(false).Value, Values[i]);
                    }
                }
                else if (Type == CollectionChangeType.Remove)
                {
                    for (var i = 0; i < Keys.Count; i++)
                    {
                        yield return Utility.ListChangeStrict(Type, i, Values[i]);
                    }
                }
                else throw new ArgumentException($"Unknown list change type: {Type}");
            }
        }
        
        public override string ToString()
        {
            var toFrom = Type == CollectionChangeType.Add ? "to" : "from";
            return $"{Type} {Values.Count} values {toFrom} a list (strictly)";
        }
    }
}
