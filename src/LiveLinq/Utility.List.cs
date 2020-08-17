using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;

using SimpleMonads;
using GenericNumbers;
using MoreCollections;
using LiveLinq.Core;
using LiveLinq.List;

using static GenericNumbers.NumbersUtility;

namespace LiveLinq
{
    internal static partial class Utility<T>
    {
        public static IListChangesStrict<T> EmptyChanges { get; }
        public static IListChangeStrict<T> EmptyChange { get; }

        static Utility()
        {
            EmptyChanges = ImmutableList<T>.Empty.ToLiveLinqUnchangingList();
            EmptyChange = Utility.ListChangeStrict<T>(CollectionChangeType.Add, 0);
        }
    }

    public static partial class Utility
    {
        /// <summary>
        /// This function is analogous to the indexing method of <see cref="List{T}"/>, except it returns
        /// an event stream of values as the value at the specified index changes, and it returns a Nothing
        /// event if there's nothing at that index (i.e., if the index is out of bounds).
        /// </summary>
        /// <param name="index">The index to watch.</param>
        internal static IObservable<IMaybe<T>> GetAtIndex<T>(IListChanges<T> source, int index)
        {
            return GetAtIndex(source, Observable.Return(index));
        }

        /// <summary>
        /// This function is analogous to the indexing method of <see cref="List{T}"/>, except it returns
        /// an event stream of values as the value at the specified index changes, and it returns a Nothing
        /// event if there's nothing at that index (i.e., if the index is out of bounds).
        /// </summary>
        /// <param name="index">The observable event stream where each event represents a new index to watch.</param>
        internal static IObservable<IMaybe<T>> GetAtIndex<T>(IListChanges<T> source, IObservable<int> index)
        {
            return source.ToObservableEnumerable()
                .CombineLatest(index, (list, idx) =>
                {
                    if (list.Count > idx)
                        return SimpleMonads.Utility.Something(list[idx]);
                    return SimpleMonads.Utility.Nothing<T>();
                });
        }

        /// <summary>
        /// This function is analogous to the indexing method of <see cref="List{T}"/>, except it returns
        /// an event stream of values as the value at the specified index changes, and it returns a Nothing
        /// event if there's nothing at that index (i.e., if the index is out of bounds).
        /// </summary>
        /// <param name="index">The observable event stream where each event represents a new index to watch.</param>
        internal static IObservable<IMaybe<T>> GetAtIndex<T>(IListChanges<T> source, IObservable<IMaybe<int>> index)
        {
            return source.ToObservableEnumerable()
                .CombineLatest(index, (list, maybeIdx) =>
                {
                    return maybeIdx.SelectMany(idx =>
                    {
                        if (list.Count > idx)
                            return SimpleMonads.Utility.Something(list[idx]);
                        return SimpleMonads.Utility.Nothing<T>();
                    });
                });
        }

        public static IObservable<IListChangeStrict<T>> ConvertStateToListChanges<T>(
            IEnumerable<T> previous,
            IEnumerable<T> current)
        {
            if (current.Any())
            {
                var currentChange = ListChangeStrict(CollectionChangeType.Add, 0, current);

                if (previous.Any())
                {
                    var previousChange = ListChangeStrict(CollectionChangeType.Remove, 0, previous);
                    var listChanges = new[] { previousChange, currentChange };
                    return listChanges.ToObservable();
                }
                return Observable.Return(currentChange);
            }
            if (previous.Any())
            {
                var previousChange = ListChangeStrict(CollectionChangeType.Remove, 0, previous);
                return Observable.Return(previousChange);
            }
            return Observable.Empty<IListChangeStrict<T>>();
        }

        /// <summary>
        /// Return a live linq query that never changes
        /// </summary>
        public static IListChanges<T> EmptyListChanges<T>()
        {
            return Utility<T>.EmptyChanges;
        }
        
        /// <summary>
        /// Return a live linq query that never changes
        /// </summary>
        public static IListChangesStrict<T> EmptyListChangesStrict<T>()
        {
            return Utility<T>.EmptyChanges;
        }

        #region List changes

        /// <summary>
        /// Create a <see cref="IListChange{T}"/> object that represents removing items from a list.
        /// </summary>
        public static IListChange<T> ListRemove<T>(INumberRange<int> range)
        {
            return new ListChange<T>(CollectionChangeType.Remove, range, ImmutableList<T>.Empty);
        }

        /// <summary>
        /// Create a <see cref="IListChange{T}"/> object that represents adding or removing items to or from a list.
        /// </summary>
        public static IListChange<T> ListChange<T>(CollectionChangeType type, INumberRange<int> range, IEnumerable<T> items)
        {
            var list = items.ToImmutableList();
            return new ListChange<T>(type, range, list);
        }

        /// <summary>
        /// Create a <see cref="IListChange{T}"/> object that represents adding or removing items to or from a list.
        /// </summary>
        public static IListChange<T> ListChange<T>(CollectionChangeType type, int index, ImmutableList<T> items)
        {
            return new ListChange<T>(type, Range(index, index + items.Count), items);
        }

        /// <summary>
        /// Create a <see cref="IListChange{T}"/> object that represents adding or removing items to or from a list.
        /// </summary>
        public static IListChange<T> ListChange<T>(CollectionChangeType type, int index, IEnumerable<T> items)
        {
            var list = items.ToImmutableList();
            return new ListChange<T>(type, Range(index, index + list.Count), list);
        }

        /// <summary>
        /// Create a <see cref="IListChange{T}"/> object that represents adding or removing items to or from a list.
        /// </summary>
        public static IListChange<T> ListChange<T>(CollectionChangeType type, int index, params T[] items)
        {
            var list = items.ToImmutableList();
            return new ListChange<T>(type, Range(index, index + list.Count), list);
        }

        /// <summary>
        /// Create a <see cref="IListChangeStrict{T}"/> object that represents adding or removing items to or from a list.
        /// </summary>
        public static IListChangeStrict<T> ListChangeStrict<T>(CollectionChangeType type, INumberRange<int> range, IEnumerable<T> items)
        {
            var list = items.ToImmutableList();
            return new ListChangeStrict<T>(type, range, list);
        }

        /// <summary>
        /// Create a <see cref="IListChangeStrict{T}"/> object that represents adding or removing items to or from a list.
        /// </summary>
        public static IListChangeStrict<T> ListChangeStrict<T>(CollectionChangeType type, int index, ImmutableList<T> items)
        {
            return new ListChangeStrict<T>(type, Range(index, index + items.Count), items);
        }

        /// <summary>
        /// Create a <see cref="IListChangeStrict{T}"/> object that represents adding or removing items to or from a list.
        /// </summary>
        public static IListChangeStrict<T> ListChangeStrict<T>(CollectionChangeType type, int index, IEnumerable<T> items)
        {
            var list = items.ToImmutableList();
            return new ListChangeStrict<T>(type, Range(index, index + list.Count), list);
        }

        /// <summary>
        /// Create a <see cref="IListChangeStrict{T}"/> object that represents adding or removing items to or from a list.
        /// </summary>
        public static IListChangeStrict<T> ListChangeStrict<T>(CollectionChangeType type, int index, params T[] items)
        {
            var list = items.ToImmutableList();
            return new ListChangeStrict<T>(type, Range(index, index + list.Count), list);
        }

        #endregion
    }
}
