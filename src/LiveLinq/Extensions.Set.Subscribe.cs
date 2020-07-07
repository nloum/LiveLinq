using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using LiveLinq.Core;
using LiveLinq.Set;
using MoreCollections;
using SimpleMonads;
using static LiveLinq.Utility;

namespace LiveLinq
{
    public static partial class Extensions
    {
        public static ISetChanges<T> Do<T>(this ISetChanges<T> source, Action<T> onAdd, Action<T> onRemove)
        {
            return source.AsObservable().Do(change =>
            {
                if (change.Type == CollectionChangeType.Add)
                {
                    foreach (var item in change.Values)
                    {
                        onAdd(item);
                    }
                }
                else if (change.Type == CollectionChangeType.Remove)
                {
                    foreach (var item in change.Values)
                    {
                        onRemove(item);
                    }
                }
            }).ToLiveLinq();
        }
        
        public static IDisposable Subscribe<T>(this ISetChanges<T> source, Action<T> onAdd, Action<T, RemovalMode> onRemove)
        {
            return source.Subscribe(t =>
                {
                    onAdd(t);
                    return Unit.Default;
                }, (t, state, removalMode) =>
                onRemove(t, removalMode));
        }

        public static IDisposable Subscribe<T, TState>(this ISetChanges<T> source, Func<T, TState> onAdd, Action<T, TState, RemovalMode> onRemove)
        {
            return source.Subscribe(items =>
                items.Select(item => new KeyValuePair<T, TState>(item, onAdd(item))).ToImmutableList(), 
                (items, removalMode) =>
                {
                    foreach (var item in items)
                    {
                        onRemove(item.Key, item.Value, removalMode);
                    }
                });
        }
        
		public static IDisposable Subscribe<T, TState>(this ISetChanges<T> source, Func<IReadOnlyList<T>, IEnumerable<KeyValuePair<T, TState>>> onAdd, Action<IEnumerable<KeyValuePair<T, TState>>, RemovalMode> onRemove)
        {
            return source.AsObservable().Scan(ImmutableDictionary<T, TState>.Empty, (state, change) =>
            {
                if (change.Type == CollectionChangeType.Add)
                {
                    return state.AddRange(onAdd(change.Values));
                }
                else if (change.Type == CollectionChangeType.Remove)
                {
                    onRemove(change.Values.Select(value =>
                        new KeyValuePair<T, TState>(value, state[value])), RemovalMode.Removal);
                    return state.RemoveRange(change.Values);
                }
                else
                {
                    throw new ArgumentException($"Unknown change type: {change.Type}");
                }
            }).Subscribe(_ => { }, (exception, maybeState) =>
                {
                    maybeState.IfHasValue(state => onRemove(state, RemovalMode.Error));
                }, maybeState => { maybeState.IfHasValue(state => onRemove(state, RemovalMode.Complete)); }, maybeState =>
                {
                    maybeState.IfHasValue(state => onRemove(state, RemovalMode.Unsubscribe));
                });
        }
    }
}
