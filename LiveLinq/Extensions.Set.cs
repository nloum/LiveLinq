using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveLinq.Core;
using LiveLinq.Dictionary;
using LiveLinq.Set;
using static LiveLinq.Utility;

namespace LiveLinq
{
    public static partial class Extensions
    {
        public static ISetChanges<T> UnchangingSetLiveLinq<T>(this IEnumerable<T> items)
        {
            return Observable.Return(SetChange(CollectionChangeType.Add, items)).ToLiveLinq();
        }
        
        /// <summary>
        /// Creates an observable event stream where each event is the new state of the LiveLinq query and
        /// the most recent change.
        /// </summary>
        public static IObservable<SetStateAndChange<T>> ToObservableStateAndChange<T>(this ISetChanges<T> source)
        {
            return source.AsObservable().Scan(new SetStateAndChange<T>(), (state, change) => state.Mutate(change));
        }

        /// <summary>
        /// Applies the Set change to the specified <see cref="ImmutableSet{T}"/>.
        /// </summary>
        public static SetStateAndChange<T> Mutate<T>(this SetStateAndChange<T> subject, ISetChange<T> change)
        {
            switch (change.Type)
            {
                case CollectionChangeType.Add:
                {
                    var newState = subject.State;
                    foreach (var item in change.Values)
                    {
                        newState = newState.Add(item);
                    }
                    var result = new SetStateAndChange<T>(newState, change);
                    return result;
                }
                case CollectionChangeType.Remove:
                {
                    var newState = subject.State;
                    foreach (var item in change.Values)
                    {
                        newState = newState.Remove(item);
                    }
                    var result = new SetStateAndChange<T>(newState, change);
                    return result;
                }
                default:
                    throw new NotImplementedException();
            }
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
