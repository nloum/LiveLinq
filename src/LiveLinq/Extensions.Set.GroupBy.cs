using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using LiveLinq.Dictionary;
using LiveLinq.Set;
using System.Collections.Generic;
using ComposableCollections.Dictionary;

namespace LiveLinq
{
    public static partial class Extensions
    {
        public static IDictionaryChanges<TKey, ISetChanges<TValue>> GroupBy<TKey, TValue>(
            this ISetChanges<TValue> source, Func<TValue, IObservable<TKey>> groupBy)
        {
            return source.Select(value => groupBy(value).Select(key => new {key, value}))
                .GroupBy(x => x.key)
                .SelectValue((key, values) => values.Select(x => x.value));
        }
        
        public static IDictionaryChangesStrict<TKey, ISetChanges<TValue>> GroupBy<TKey, TValue>(
            this ISetChanges<TValue> source, Func<TValue, TKey> groupBy)
        {
            return source.AsObservable()
                .Scan(
                    Tuple.Create(ImmutableDictionary<TKey, BehaviorSubject<ISetChange<TValue>>>.Empty,
                        ImmutableList<IDictionaryChangeStrict<TKey, ISetChanges<TValue>>>.Empty), (state, change) =>
                    {
                        var dictionaryChanges = new List<IDictionaryChangeStrict<TKey, ISetChanges<TValue>>>();
                        var dictionary = state.Item1;
                        foreach (var group in change.Values.GroupBy(groupBy))
                        {
                            var setChange = Utility.SetChange(change.Type, group.ToImmutableList());
                            
                            if (dictionary.ContainsKey(group.Key))
                            {
                                dictionary[group.Key].OnNext(setChange);
                            }
                            else
                            {
                                var newValue = new BehaviorSubject<ISetChange<TValue>>(setChange);
                                var dictionaryChange = Utility.DictionaryChange(change.Type, new KeyValue<TKey, ISetChanges<TValue>>(group.Key, newValue.ToLiveLinq()));
                                dictionaryChanges.Add(dictionaryChange);
                                dictionary = dictionary.SetItem(group.Key, newValue);
                            }
                        }

                        return Tuple.Create(dictionary, dictionaryChanges.ToImmutableList());
                    })
                .SelectMany(x => x.Item2)
                .ToLiveLinq();
        }
    }
}