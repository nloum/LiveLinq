using System;
using System.Reactive.Linq;
using LiveLinq.Core;
using LiveLinq.Set;

namespace LiveLinq
{
    public static partial class Extensions
    {
        public static IObservable<int> Count<T>(this ISetChanges<T> source)
        {
            return Observable.Return(0).Concat(source.AsObservable().Scan(0, (count, change) =>
            {
                if (change.Type == CollectionChangeType.Add)
                {
                    return count + change.Values.Count;
                }
                else if (change.Type == CollectionChangeType.Remove)
                {
                    return count - change.Values.Count;
                }
                else
                {
                    throw new ArgumentException($"Unknown change type {change.Type}");
                }
            }));
        }

        public static IObservable<bool> Any<T>(this ISetChanges<T> source)
        {
            return source.Count().Select(count => count > 0).DistinctUntilChanged();
        }

        public static IObservable<bool> All<T>(this ISetChanges<T> source, Func<T, bool> predicate)
        {
            return source.Where(predicate).Any();
        }

        public static IObservable<bool> All<T>(this ISetChanges<T> source, Func<T, IObservable<bool>> predicate)
        {
            return source.Where(predicate).Any();
        }
    }
}