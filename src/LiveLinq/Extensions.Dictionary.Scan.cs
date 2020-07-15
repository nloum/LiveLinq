using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using LiveLinq.Core;
using LiveLinq.Dictionary;

namespace LiveLinq
{
    // public static partial class Extensions
    // {
    //     public static IDictionaryChangesStrict<TKey, TValue> Buffer<TKey, TValue>(
    //         this IDictionaryChangesStrict<TKey, TValue> source, Func<ImmutableList<IDictionaryChangeStrict<TKey, TValue>>, bool> flushBuffer)
    //     {
    //         return source.AsObservable()
    //             .Scan(BufferedDictionaryChanges<TKey, TValue>.Empty, (state, newChange) =>
    //             {
    //                 if (newChange.Type == CollectionChangeType.Add)
    //                 {
    //                     var interestingKeys = state.Removals.Keys.Intersect(newChange.Keys);
    //                     foreach (var key in interestingKeys)
    //                     {
    //                         if ()
    //                     }
    //                 }
    //             });
    //     }
    // }
    //
    // public class BufferedDictionaryChanges<TKey, TValue>
    // {
    //     public static BufferedDictionaryChanges<TKey, TValue> Empty { get; } = new BufferedDictionaryChanges<TKey, TValue>(ImmutableDictionary<TKey, TValue>.Empty, ImmutableDictionary<TKey, TValue>.Empty);
    //     
    //     public BufferedDictionaryChanges(ImmutableDictionary<TKey, TValue> removals, ImmutableDictionary<TKey, TValue> additions)
    //     {
    //         Removals = removals;
    //         Additions = additions;
    //     }
    //
    //     public ImmutableDictionary<TKey, TValue> Removals { get; }
    //     public ImmutableDictionary<TKey, TValue> Additions { get; }
    // }
}