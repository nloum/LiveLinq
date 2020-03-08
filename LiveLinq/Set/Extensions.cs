using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveLinq.Core;
using LiveLinq.Dictionary;

namespace LiveLinq.Set
{
    public static partial class Extensions
    {
        public static ISetChanges<TKey> Keys<TKey, TValue>(this IDictionaryChanges<TKey, TValue> dictionaryChanges)
        {
            return dictionaryChanges.AsObservable().Select(dictionaryChange =>
            {
                return Utility.SetChange(dictionaryChange.Type, dictionaryChange.Items.Select(x => x.Key));
            }).ToLiveLinq();
        }
        
        public static ISetChanges<T> SelectMany<TKey, TValue, T>(this IDictionaryChangesStrict<TKey, TValue> changes, Func<TKey, TValue, IEnumerable<T>> selector)
        {
            return changes.AsObservable().Select(change =>
                    Utility.SetChange(change.Type, change.Items.SelectMany(kvp => selector(kvp.Key, kvp.Value))))
                .ToLiveLinq();
        }
        
        public static ISetChanges<T> Select<TKey, TValue, T>(this IDictionaryChangesStrict<TKey, TValue> changes, Func<TKey, TValue, T> selector)
        {
            return changes.AsObservable().Select(change =>
                    Utility.SetChange(change.Type, change.Items.Select(kvp => selector(kvp.Key, kvp.Value))))
                .ToLiveLinq();
        }
        
        public static IReadOnlyObservableSet<T> ToReadOnlySet<T>(this ISetChanges<T> source)
        {
            var result = new ObservableSet<T>();
            source.AsObservable().Subscribe(x =>
            {
                if (x.Type == CollectionChangeType.Add)
                {
                    result.AddRange(x.Values);
                }
                else if (x.Type == CollectionChangeType.Remove)
                {
                    result.RemoveRange(x.Values);
                }
                else
                {
                    throw new ArgumentException($"Unknown change type {x.Type}");
                }
            });
            return result;
        }

        public static ISetChanges<T> ToLiveLinq<T>(this IObservable<ISetChange<T>> source)
        {
            return new SetChanges<T>(source);
        }
    }
}
