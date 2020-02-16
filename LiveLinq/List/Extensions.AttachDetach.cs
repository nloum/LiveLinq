using System;
using System.Reactive;

using UtilityDisposables;
using MoreCollections;
using LiveLinq.Core;

namespace LiveLinq.List
{
    /// <summary>
    /// Static class that contains core extension methods for LiveLinq
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Utility method that calls <see cref="attach"/> when an item is added to the LiveLinq query and <see cref="detach"/>
        /// when an item is removed from a LiveLinq query.
        /// The first event in a LiveLinq query is the initial state of the list; so, <see cref="attach"/> is called first
        /// for every item that was already in the query.
        /// </summary>
        public static IDisposable AttachDetach<T>(this IListChangesStrict<T> source, Action<T> attach, Action<T> detach)
        {
            return source.AttachDetach(
                (t, i) =>
                {
                    attach(t);
                    return Unit.Default;
                },
                (t, i, _) => detach(t));
        }

        /// <summary>
        /// Utility method that calls <see cref="attach"/> when an item is added to the LiveLinq query and <see cref="detach"/>
        /// when an item is removed from a LiveLinq query.
        /// The first event in a LiveLinq query is the initial state of the list; so, <see cref="attach"/> is called first
        /// for every item that was already in the query.
        /// </summary>
        public static IDisposable AttachDetach<T>(
            this IListChangesStrict<T> source,
            Action<T, IObservable<int>> attach,
            Action<T, IObservable<int>> detach)
        {
            return source.AttachDetach(
                (t, i) =>
                {
                    attach(t, i);
                    return Unit.Default;
                },
                (t, i, _) => detach(t, i));
        }

        /// <summary>
        /// Utility method that calls <see cref="attach"/> when an item is added to the LiveLinq query. and <see cref="detach"/>
        /// when an item is removed from a LiveLinq query. The second parameter to the <see cref="detach"/> method is
        /// what was returned by <see cref="attach"/> when the item was added.
        /// The first event in a LiveLinq query is the initial state of the list; so, <see cref="attach"/> is called first
        /// for every item that was already in the query.
        /// </summary>
        public static IDisposable AttachDetach<T, TState>(
            this IListChangesStrict<T> source,
            Func<T, TState> attach,
            Action<T, TState> detach)
        {
            return source.AttachDetach((t, i) => attach(t), (t, i, state) => detach(t, state));
        }

        /// <summary>
        /// Utility method that calls <see cref="attach"/> when an item is added to the LiveLinq query. and <see cref="detach"/>
        /// when an item is removed from a LiveLinq query. The second parameter to the <see cref="detach"/> method is
        /// what was returned by <see cref="attach"/> when the item was added.
        /// The first event in a LiveLinq query is the initial state of the list; so, <see cref="attach"/> is called first
        /// for every item that was already in the query.
        /// </summary>
        public static IDisposable AttachDetach<T, TState>(
            this IListChangesStrict<T> source,
            Func<T, IObservable<int>, TState> attach,
            Action<T, IObservable<int>, TState> detach)
        {
            var sourceWithState = source.Select((t, i) => Tuple.Create(t, i, attach(t, i))).ToObservableStateAndChange();
            var theLock = new object();
            StateAndChange<Tuple<T, IObservable<int>, TState>> lastStateAndChange = null;
            return sourceWithState.Subscribe(
                stateAndChange =>
                {
                    lock (theLock)
                    {
                        var change = stateAndChange.MostRecentChange;
                        lastStateAndChange = stateAndChange;
                        if (change.Type == CollectionChangeType.Remove)
                        {
                            foreach (var remove in change.Values)
                            {
                                detach(remove.Item1, remove.Item2, remove.Item3);
                            }
                        }
                    }
                }).DisposeWith(
                        new AnonymousDisposable(
                            () =>
                            {
                                lock (theLock)
                                {
                                    if (lastStateAndChange == null) return;
                                    var currentItems = lastStateAndChange.State;
                                    var a = currentItems.Count;
                                    foreach (var remove in currentItems)
                                    {
                                        detach(remove.Item1, remove.Item2, remove.Item3);
                                    }
                                }
                            }));
        }
    }
}
