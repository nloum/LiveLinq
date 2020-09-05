using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveLinq.Core;
using LiveLinq.Dictionary;
using LiveLinq.List;
using LiveLinq.Set;
using SimpleMonads;
using static LiveLinq.Utility;

namespace LiveLinq
{
    public static partial class Extensions
    {
        public static ISetChanges<T> OtherwiseEmpty<T>(this IMaybe<ISetChanges<T>> maybe)
        {
            return maybe.Otherwise(EmptySetChanges<T>);
        }
        
        public static ISetChanges<T> ToLiveLinqUnchangingSet<T>(this IEnumerable<T> items)
        {
            return Observable.Return(SetChange(CollectionChangeType.Add, items)).ToLiveLinq();
        }
        
        /// <summary>
        /// Creates an observable event stream where each event is the new state of the LiveLinq query and
        /// the most recent change.
        /// </summary>
        public static IObservable<SetStateAndChange<T>> ToObservableStateAndChange<T>(this ISetChanges<T> source)
        {
            return source.AsObservable().Scan(new SetStateAndChange<T>(), (state, change) => state.Write(change));
        }

        /// <summary>
        /// Creates an observable event stream where each event is the new state of the LiveLinq query and
        /// the most recent change.
        /// </summary>
        public static IObservable<ImmutableHashSet<T>> ToObservableState<T>(this ISetChanges<T> source)
        {
            return source.ToObservableStateAndChange().Select(x => x.State);
        }

        /// <summary>
        /// Applies the Set change to the specified <see cref="ImmutableSet{T}"/>.
        /// </summary>
        public static SetStateAndChange<T> Write<T>(this SetStateAndChange<T> subject, ISetChange<T> change)
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
                    Utility.SetChange(change.Type, change.KeyValuePairs.SelectMany(kvp => selector(kvp.Key, kvp.Value))))
                .ToLiveLinq();
        }
        
        public static ISetChanges<T> Select<TKey, TValue, T>(this IDictionaryChangesStrict<TKey, TValue> changes, Func<TKey, TValue, T> selector)
        {
            return changes.AsObservable().Select(change =>
                    Utility.SetChange(change.Type, change.KeyValuePairs.Select(kvp => selector(kvp.Key, kvp.Value))))
                .ToLiveLinq();
        }
        
        public static IObservableReadOnlySet<T> ToReadOnlyObservableSet<T>(this ISetChanges<T> source)
        {
            var result = new ReadOnlyObservableSet<T>(source);
            return result;
        }

        public static ISetChanges<T> ToLiveLinq<T>(this IObservable<ISetChange<T>> source)
        {
            return new SetChanges<T>(source);
        }

        public static ISetChanges<T> ToSetChanges<T>(this IListChangesStrict<T> listChanges)
        {
            return listChanges.AsObservable().Select(listChange =>
            {
                return (ISetChange<T>)new SetChange<T>(listChange.Type, listChange.Values.ToImmutableList());
            }).ToLiveLinq();
        }

        public static IObservable<IMaybe<T>> First<T>(this ISetChanges<T> source)
        {
            return source.ToObservableState().Select(x => x.FirstOrDefault()?.ToMaybe() ?? Maybe<T>.Nothing(() => throw new IndexOutOfRangeException("Cannot get the first item of an empty set")));
        }
        
        public static ISetChanges<TValue> SelectMany<T, TValue>(this IObservable<T> source, Func<T, ISetChanges<TValue>> setChanges)
        {
            int index = 0;
            var observable = source.SelectLatest(x => setChanges(x).ToObservableStateAndChange().Select(stateChangeChange => new { stateChangeChange, index = index++ }));
            
            var result = Observable.Create<ISetChange<TValue>>((observer, cancellationToken) =>
            {
                return Task.Factory.StartNew<Action>(() =>
                {
                    var prevIndex = -1;
                    SetStateAndChange<TValue> prevStateAndChange = null;
                    foreach (var item in observable.ToEnumerable())
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return () => { };
                        }
                    
                        if (prevStateAndChange == null)
                        {
                            prevIndex = item.index;
                            prevStateAndChange = item.stateChangeChange;
                            observer.OnNext(item.stateChangeChange.MostRecentChange);
                        }
                        else
                        {
                            if (prevIndex != item.index)
                            {
                                observer.OnNext(new SetChange<TValue>(CollectionChangeType.Remove, prevStateAndChange.State.ToImmutableList()));
                                observer.OnNext(new SetChange<TValue>(CollectionChangeType.Add, item.stateChangeChange.State.ToImmutableList()));
                            }
                            else
                            {
                                observer.OnNext(item.stateChangeChange.MostRecentChange);
                            }
                        }
                    }
                    return () => { };
                });
            });

            return result.ToLiveLinq();
        }
    }
}
