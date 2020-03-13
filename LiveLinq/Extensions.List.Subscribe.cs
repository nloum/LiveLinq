﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using GenericNumbers;
using LiveLinq.Core;
using LiveLinq.List;
using MoreCollections;
using SimpleMonads;

namespace LiveLinq
{
    /// <summary>
    /// Static class that contains core extension methods for LiveLinq
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Utility method that calls <see cref="onAdd"/> when an item is added to the LiveLinq query and <see cref="onRemove"/>
        /// when an item is removed from a LiveLinq query.
        /// The first event in a LiveLinq query is the initial state of the list; so, <see cref="onAdd"/> is called first
        /// for every item that was already in the query.
        /// </summary>
        public static IDisposable Subscribe<T>(this IListChangesStrict<T> source, Action<T> onAdd, Action<T> onRemove)
        {
            return source.Subscribe(
                t =>
                {
                    onAdd(t);
                    return Unit.Default;
                },
                (t, _) => onRemove(t));
        }

        /// <summary>
        /// Utility method that calls <see cref="onAdd"/> when an item is added to the LiveLinq query. and <see cref="onRemove"/>
        /// when an item is removed from a LiveLinq query. The second parameter to the <see cref="onRemove"/> method is
        /// what was returned by <see cref="onAdd"/> when the item was added.
        /// The first event in a LiveLinq query is the initial state of the list; so, <see cref="onAdd"/> is called first
        /// for every item that was already in the query.
        /// </summary>
        public static IDisposable Subscribe<T, TState>(
            this IListChangesStrict<T> source,
            Func<T, TState> onAdd,
            Action<T, TState> onRemove)
        {
            return source.Subscribe((t, index) => onAdd(t), (item, state, oldIndex, newIndex) => { }, (t, state, index, removalMode) => onRemove(t, state), true, true);
        }

        public enum RemovalMode
        {
            Removal,
            Unsubscribe,
            Complete,
            Error
        }

        /// <summary>
        /// Utility method that calls <see cref="onAdd"/> when an item is added to the LiveLinq query. and <see cref="onRemove"/>
        /// when an item is removed from a LiveLinq query. The second parameter to the <see cref="onRemove"/> method is
        /// what was returned by <see cref="onAdd"/> when the item was added.
        /// The first event in a LiveLinq query is the initial state of the list; so, <see cref="onAdd"/> is called first
        /// for every item that was already in the query.
        /// </summary>
        public static IDisposable Subscribe<T>(
            this IListChangesStrict<T> source,
            Action<T, int> onAdd,
            Action<T, int, int> onMove,
            Action<T, int, RemovalMode> onRemove,
            bool moveBeforeAdd, bool moveBeforeRemove)
        {
            return source.Subscribe(
                (item, index) =>
                {
                    onAdd(item, index);
                    return Unit.Default;
                },
                (item, state, oldIndex, newIndex) => onMove(item, oldIndex, newIndex),
                (item, state, oldIndex, removalMode) => onRemove(item, oldIndex, removalMode),
                moveBeforeAdd, moveBeforeRemove);
        }
        
        /// <summary>
        /// Utility method that calls <see cref="onAdd"/> when an item is added to the LiveLinq query. and <see cref="onRemove"/>
        /// when an item is removed from a LiveLinq query. The second parameter to the <see cref="onRemove"/> method is
        /// what was returned by <see cref="onAdd"/> when the item was added.
        /// The first event in a LiveLinq query is the initial state of the list; so, <see cref="onAdd"/> is called first
        /// for every item that was already in the query.
        /// </summary>
        public static IDisposable Subscribe<T, TState>(
            this IListChangesStrict<T> source,
            Func<T, int, TState> onAdd,
            Action<T, TState, int, int> onMove,
            Action<T, TState, int, RemovalMode> onRemove,
            bool moveBeforeAdd, bool moveBeforeRemove)
        {
            return source.Subscribe(
                (IReadOnlyList<T> newItems, INumberRange<int> indexRange) =>
                {
                    return newItems.Select((value, i) => Tuple.Create(value,
                        onAdd(value, i + indexRange.LowerBound.ChangeStrictness(false).Value))).ToImmutableList();
                },
                (IReadOnlyList<Tuple<T, TState>> items, INumberRange<int> oldRange, INumberRange<int> newRange) =>
                {
                    for (var i = 0; i < items.Count; i++)
                    {
                        var oldIndex = oldRange.LowerBound.ChangeStrictness(false).Value + i;
                        var newIndex = newRange.LowerBound.ChangeStrictness(false).Value + i;
                        onMove(items[i].Item1, items[i].Item2, oldIndex, newIndex);
                    }
                },
                (IReadOnlyList<Tuple<T, TState>> items, INumberRange<int> oldRange, RemovalMode removalMode) =>
                {
                    for (var i = 0; i < items.Count; i++)
                    {
                        var oldIndex = oldRange.LowerBound.ChangeStrictness(false).Value + i;
                        onRemove(items[i].Item1, items[i].Item2, oldIndex, removalMode);
                    }
                },
                moveBeforeAdd, moveBeforeRemove);
        }
        
        /// <summary>
        /// Utility method that calls <see cref="onAdd"/> when an item is added to the LiveLinq query. and <see cref="onRemove"/>
        /// when an item is removed from a LiveLinq query. The second parameter to the <see cref="onRemove"/> method is
        /// what was returned by <see cref="onAdd"/> when the item was added.
        /// The first event in a LiveLinq query is the initial state of the list; so, <see cref="onAdd"/> is called first
        /// for every item that was already in the query.
        /// </summary>
        public static IDisposable Subscribe<T, TState>(
            this IListChangesStrict<T> source,
            Func<IReadOnlyList<T>, INumberRange<int>, ImmutableList<Tuple<T, TState>>> onAdd,
            Action<IReadOnlyList<Tuple<T, TState>>, INumberRange<int>, INumberRange<int>> onMove,
            Action<IReadOnlyList<Tuple<T, TState>>, INumberRange<int>, RemovalMode> onRemove,
            bool moveBeforeAdd, bool moveBeforeRemove)
        {
            return source
                .AsObservable().Scan(new StateAndChange<Tuple<T, TState>>[0], (state, change) =>
                {
                    if (state.Length == 0)
                    {
                        // This is the first change

                        if (change.Type != CollectionChangeType.Add)
                        {
                            throw new InvalidOperationException(
                                $"Unexpected initial collection change type: {change.Type}. Maybe the first collection change was skipped?");
                        }

                        var values = onAdd(change.Values, change.Range);
                        var modifiedChange = Utility.ListChangeStrict(change.Type, change.Range,
                            values);

                        return new[]
                        {
                            new StateAndChange<Tuple<T, TState>>(values, modifiedChange)
                        };
                    }
                    else
                    {
                        // This is not the first change

                        var mostRecentState = state[state.Length - 1];
                        ImmutableList<Tuple<T, TState>> newState;
                        IListChangeStrict<Tuple<T, TState>> newChange;

                        if (change.Type == CollectionChangeType.Add)
                        {
                            if (onMove != null && moveBeforeAdd)
                            {
                                var oldRange = NumbersUtility.Range( change.Range.LowerBound.ChangeStrictness(false).Value, mostRecentState.State.Count );
                                var newRange = NumbersUtility.Range(change.Range.UpperBound.ChangeStrictness(true).Value, change.Range.UpperBound.ChangeStrictness(true).Value + oldRange.Size );
                                onMove(mostRecentState.State.TakeEfficiently(change.Range), oldRange, newRange);
                            }

                            newChange = Utility.ListChangeStrict(CollectionChangeType.Add, change.Range,
                                onAdd(change.Values, change.Range));
                            newState = mostRecentState.State.Mutate(newChange);

                            if (onMove != null && !moveBeforeAdd)
                            {
                                var oldRange = NumbersUtility.Range( change.Range.LowerBound.ChangeStrictness(false).Value, mostRecentState.State.Count );
                                var newRange = NumbersUtility.Range(change.Range.UpperBound.ChangeStrictness(true).Value, change.Range.UpperBound.ChangeStrictness(true).Value + oldRange.Size );
                                onMove(mostRecentState.State.TakeEfficiently(change.Range), oldRange, newRange);
                            }
                        }
                        else
                        {
                            if (onMove != null && moveBeforeRemove)
                            {
                                var oldRange = NumbersUtility.Range(change.Range.UpperBound.ChangeStrictness(true).Value, mostRecentState.State.Count);
                                var newRange = NumbersUtility.Range( change.Range.LowerBound.ChangeStrictness(false).Value, change.Range.LowerBound.ChangeStrictness(false).Value + oldRange.Size );
                                onMove(mostRecentState.State.TakeEfficiently(change.Range), oldRange, newRange);
                            }

                            onRemove(mostRecentState.State.TakeEfficiently(change.Range), change.Range, RemovalMode.Removal);
                            
                            newChange = Utility.ListChangeStrict(CollectionChangeType.Remove, change.Range,
                                mostRecentState.State.Skip(change.Range.LowerBound.ChangeStrictness(false).Value)
                                    .Take(change.Range.Size));
                            newState = mostRecentState.State.Mutate(newChange);

                            if (onMove != null && !moveBeforeRemove)
                            {
                                var oldRange = NumbersUtility.Range(change.Range.UpperBound.ChangeStrictness(true).Value, mostRecentState.State.Count);
                                var newRange = NumbersUtility.Range( change.Range.LowerBound.ChangeStrictness(false).Value, change.Range.LowerBound.ChangeStrictness(false).Value + oldRange.Size );
                                onMove(mostRecentState.State.TakeEfficiently(change.Range), oldRange, newRange);
                            }
                        }

                        return new StateAndChange<Tuple<T, TState>>[]
                        {
                            mostRecentState,
                            new StateAndChange<Tuple<T, TState>>(newState, newChange),
                        };
                    }
                }).Subscribe(_ => { }, (exception, maybeData) =>
                {
                    maybeData.IfHasValue(data =>
                    {
                        if (data.Length > 0)
                        {
                            onRemove(data[data.Length - 1].State, NumbersUtility.Range(0, data[data.Length - 1].State.Count), RemovalMode.Error);
                        }
                    });
                }, maybeData =>
                {
                    maybeData.IfHasValue(data =>
                    {
                        if (data.Length > 0)
                        {
                            onRemove(data[data.Length - 1].State, NumbersUtility.Range(0, data[data.Length - 1].State.Count), RemovalMode.Complete);
                        }
                    });
                }, maybeData =>
                {
                    maybeData.IfHasValue(data =>
                    {
                        if (data.Length > 0)
                        {
                            onRemove(data[data.Length - 1].State, NumbersUtility.Range(0, data[data.Length - 1].State.Count), RemovalMode.Unsubscribe);
                        }
                    });
                });
        }
    }
}