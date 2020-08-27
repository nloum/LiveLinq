using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using GenericNumbers;
using MoreCollections;
using LiveLinq.Core;
using LiveLinq.List;

namespace LiveLinq
{
    public static partial class Extensions
    {
        /// <summary>
        ///     Applies the list change to the specified <see cref="ImmutableList{T}" />.
        /// </summary>
        public static ImmutableList<T> Write<T>(this ImmutableList<T> subject, IListChange<T> change)
        {
            var range = change.Range.ChangeStrictness(false, true);
            switch (change.Type)
            {
                case CollectionChangeType.Add:
                {
                    var result = subject.InsertRange(change.Range.LowerBound.Value, change.Values);
                    return result;
                }
                case CollectionChangeType.Remove:
                {
                    var result = subject.RemoveRange(change.Range);
                    return result;
                }
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        ///     Applies the list change to the specified <see cref="ImmutableList{T}" />.
        /// </summary>
        public static void Write<T>(this IList<T> subject, IListChange<T> change)
        {
            var range = change.Range.ChangeStrictness(false, true);
            switch (change.Type)
            {
                case CollectionChangeType.Add:
                {
                    subject.InsertRange(range.LowerBound.Value, change.Values);
                    break;
                }
                case CollectionChangeType.Remove:
                {
                    subject.RemoveRange(range);
                    break;
                }
                default:
                    throw new NotImplementedException();
            }
        }
    }
}