using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Tracing;
using System.Linq;
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
        public static IDisposable Subscribe<T, TState>(this ISetChanges<T> source, Func<T, TState> onAdd, Action<T, TState, RemovalMode> onRemove)
        {
            return source.Subscribe(items =>
                items.Select(item => new KeyValuePair<T, TState>(item, onAdd(item))), 
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
