using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GenericNumbers;
using LiveLinq.Core;

namespace LiveLinq.List
{

    /// <summary>
    /// Describes a change to a list.
    /// </summary>
    internal class ListChange<T> : IListChange<T>
    {
        public IReadOnlyList<T> Values { get; }

        public IReadOnlyList<int> Keys { get; }

        public CollectionChangeType Type { get; }

        public INumberRange<int> Range { get; }

        public virtual bool IsEffectivelyStrict => Type == CollectionChangeType.Add || Keys.Count == 0 || Values.Any();

        public bool ContainsKey(int key)
        {
            return Range.Includes(key);
        }

        public IEnumerable<IListChange<T>> Itemize()
        {
            if (Range.Size == 0) yield break;
            else if (Range.Size == 1) yield return this;
            else
            {
                if (Type == CollectionChangeType.Add)
                {
                    for (var i = 0; i < Keys.Count; i++)
                    {
                        yield return Utility.ListChange(Type, i + Range.LowerBound.ChangeStrictness(false).Value, Values[i]);
                    }
                }
                else if (Type == CollectionChangeType.Remove)
                {
                    if (IsEffectivelyStrict)
                    {
                        for (var i = 0; i < Keys.Count; i++)
                        {
                            yield return Utility.ListChange(Type, i, Values[i]);
                        }
                    }
                    else
                    {
                        for (var i = 0; i < Keys.Count; i++)
                        {
                            yield return Utility.ListRemove<T>(NumbersUtility.Range(i + Range.LowerBound.ChangeStrictness(false).Value, i + Range.LowerBound.ChangeStrictness(false).Value + 1));
                        }
                    }
                } else throw new ArgumentException($"Unknown list change type: {Type}");
            }
        }
        
        internal ListChange(CollectionChangeType type, INumberRange<int> range, ImmutableList<T> values)
        {
            Type = type;
            Range = range;
            Values = values;
            Keys = Range.Numbers();
        }
        
        public override string ToString()
        {
            var toFrom = Type == CollectionChangeType.Add ? "to" : "from";
            return $"{Type} {Values.Count} values {toFrom} a list (non-strictly)";
        }
    }
}
