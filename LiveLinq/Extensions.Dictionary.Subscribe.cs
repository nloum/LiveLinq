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
        public static IDisposable Subscribe<TKey, TValue>(
            this IDictionaryChanges<TKey, TValue> source,
            Action<TKey, TValue> attach,
            Action<TKey, TValue> detach)
        {
            return source.Subscribe((key, value) =>
            {
                attach(key, value);
                return Unit.Default;
            }, (key, value, _) => detach(key, value));
        }

        public static IDisposable Subscribe<TKey, TValue, TState>(
            this IDictionaryChanges<TKey, TValue> source,
            Func<TKey, TValue, TState> attach,
            Action<TKey, TValue, TState> detach)
        {
            return source.AsObservable()
                .Select(change => change.Itemize().ToObservable())
                .Concat()
                .Scan(ImmutableDictionary<TKey, Tuple<TValue, TState>>.Empty,
                    (state, change) =>
                    {
                        var key = change.Items[0].Key;
                        if (change.Type == CollectionChangeType.Add)
                        {
                            var value = change.Items[0].Value;
                            return state.Add(key, Tuple.Create(value, attach(key, value)));
                        }
                        else
                        {
                            detach(key, state[key].Item1, state[key].Item2);
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
                            detach(item.Key, item.Value.Item1, item.Value.Item2);
                        }
                    }
                });
        }
    }
}
