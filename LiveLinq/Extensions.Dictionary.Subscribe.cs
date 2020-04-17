using System;
using System.Collections.Immutable;
using System.Reactive;
using System.Reactive.Linq;
using LiveLinq.Core;
using LiveLinq.Dictionary;

namespace LiveLinq
{
    public static partial class Extensions
    {
        public static IDictionaryChangesStrict<TKey, TValue> Do<TKey, TValue>(
            IDictionaryChangesStrict<TKey, TValue> source, Action<TKey, TValue> onAdd, Action<TKey, TValue> onRemove)
        {
            return source.AsObservable()
                .Do(change =>
                {
                    if (change.Type == CollectionChangeType.Add)
                    {
                        foreach (var item in change.KeyValuePairs)
                        {
                            onAdd(item.Key, item.Value);
                        }
                    }
                    else if (change.Type == CollectionChangeType.Remove)
                    {
                        foreach (var item in change.KeyValuePairs)
                        {
                            onRemove(item.Key, item.Value);
                        }
                    }
                }).ToLiveLinq();
        }
        
        public static IDisposable Subscribe<TKey, TValue>(
            this IDictionaryChanges<TKey, TValue> source,
            Action<TKey, TValue> onAdd,
            Action<TKey, TValue> onRemove)
        {
            return source.Subscribe((key, value) =>
            {
                onAdd(key, value);
                return Unit.Default;
            }, (key, value, _) => onRemove(key, value));
        }

        public static IDisposable Subscribe<TKey, TValue, TState>(
            this IDictionaryChanges<TKey, TValue> source,
            Func<TKey, TValue, TState> onAdd,
            Action<TKey, TValue, TState> onRemove)
        {
            return source.AsObservable()
                .Select(change => change.Itemize().ToObservable())
                .Concat()
                .Scan(ImmutableDictionary<TKey, Tuple<TValue, TState>>.Empty,
                    (state, change) =>
                    {
                        var key = change.KeyValuePairs[0].Key;
                        if (change.Type == CollectionChangeType.Add)
                        {
                            var value = change.KeyValuePairs[0].Value;
                            return state.Add(key, Tuple.Create(value, onAdd(key, value)));
                        }
                        else
                        {
                            onRemove(key, state[key].Item1, state[key].Item2);
                            return state.Remove(key);
                        }
                        throw new NotImplementedException($"Unknown collection change type: {change.Type}");
                    })
                .Subscribe(_ => { }, _ => { }, last =>
                {
                    if (last != null)
                    {
                        foreach (var item in last)
                        {
                            onRemove(item.Key, item.Value.Item1, item.Value.Item2);
                        }
                    }
                });
        }
    }
}
