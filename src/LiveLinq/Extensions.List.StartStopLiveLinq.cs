using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using SimpleMonads;
using MoreCollections;
using LiveLinq.Core;
using LiveLinq.List;
using LiveLinq.List.Adapters;
using static LiveLinq.Utility;

namespace LiveLinq
{
    public static partial class Extensions
    {
        public static BindableListAdapter<TItem> WithBindability<TItem>(this IReadOnlyObservableList<TItem> source)
        {
            return new BindableListAdapter<TItem>(source);
        }
        
        /// <summary>
        /// Takes a non-strict LiveLinq query and turns it into a strict one. This tracks all the adds
        /// so that it knows the exact object that is being removed when a remove event comes from the source
        /// that specifies the indices to be removed, but does not specify the actual objects to be removed.
        /// 
        /// A strict LiveLinq query is one that always specifies the objects that are being added and removed.
        /// A non-strict LiveLinq query is one that may not specify the objects that are being removed
        /// (but it will specify the indices of course), and it will still always specify the items that are
        /// being added.
        /// </summary>
        public static IListChangesStrict<T> MakeStrictExpensively<T>(this IListChanges<T> source)
        {
            if (source is IListChangesStrict<T>) return (IListChangesStrict<T>)source;
            return source.ToObservableStateAndChange().Select(sac => sac.MostRecentChange).ToLiveLinq();
        }

        /// <summary>
        /// Takes a strict LiveLinq query and turns it into a non-strict one.
        /// 
        /// A strict LiveLinq query is one that always specifies the objects that are being added and removed.
        /// A non-strict LiveLinq query is one that may not specify the objects that are being removed
        /// (but it will specify the indices of course), and it will still always specify the items that are
        /// being added.
        /// </summary>
        public static IListChanges<T> MakeNonStrict<T>(this IListChangesStrict<T> source)
        {
            return source;
        }

        #region Methods for ending a LiveLinq method chain
        
        /// <summary>
        /// Creates an observable event stream where each event is the new state of the LiveLinq query and
        /// the most recent change.
        /// </summary>
        public static IObservable<ListStateAndChange<T>> ToObservableStateAndChange<T>(this IListChanges<T> source)
        {
            return source.AsObservable().Scan(new ListStateAndChange<T>(), (state, change) => state.Write(change));
        }

        /// <summary>
        /// Creates an observable event stream where each event is the new state of the LiveLinq query and
        /// the most recent change.
        /// </summary>
        public static IObservable<ImmutableList<T>> ToObservableState<T>(this IListChanges<T> source)
        {
            return source.ToObservableStateAndChange().Select(x => x.State);
        }

        /// <summary>
        /// Applies the list change to the specified <see cref="ImmutableList{T}"/>.
        /// </summary>
        public static ListStateAndChange<T> Write<T>(this ListStateAndChange<T> subject, IListChange<T> change)
        {
            switch (change.Type)
            {
                case CollectionChangeType.Add:
                    {
                        var result = new ListStateAndChange<T>(subject.State.InsertRange(change.Range.LowerBound.Value, change.Values), ListChangeStrict(change.Type, change.Range, change.Values));
                        return result;
                    }
                case CollectionChangeType.Remove:
                    {
                        IListChangeStrict<T> newChange;
                        if (!change.Values.Any()) newChange = ListChangeStrict(CollectionChangeType.Remove, change.Range, subject.State.GetRange(change.Range));
                        else newChange = ListChangeStrict(CollectionChangeType.Remove, change.Range, change.Values);
                        var result = new ListStateAndChange<T>(subject.State.RemoveRange(change.Range), newChange);
                        return result;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Return an event stream where every time the source LiveLinq query changes, a new event is fired,
        /// and each event represents the most recent state of the LiveLinq query.
        /// </summary>
        public static IObservable<ImmutableList<T>> ToObservableEnumerable<T>(this IListChanges<T> source)
        {
            return source.ToObservableStateAndChange().Select(state => state.State);
        }

        /// <summary>
        /// Converts a LiveLinq query into a list that changes whenever the LiveLinq query changes; thus,
        /// the result of this function always represents the latest state of the LiveLinq query.
        /// 
        /// Note that the return value is disposable. This value must be disposed in order to stop
        /// watching for changes in the LiveLinq query.
        /// </summary>
        public static IReadOnlyObservableList<T> ToReadOnlyObservableList<T>(this IListChanges<T> source)
        {
            return new ReadOnlyObservableList<T>(source);
        }

        #endregion

        #region Methods for initiating a LiveLinq method chain

        /// <summary>
        /// Generates a LiveLinq query that initially has no element, and as each new source event is fired, it appends that
        /// as an item to the query.
        /// </summary>
        public static IListChangesStrict<T> Collect<T>(this IObservable<T> source)
        {
            // Note: the first event should ALWAYS represent the actual state of the list at that time. If the initial state
            // of the list is empty, the first event MUST be an empty add event. So, here we first generate an empty add
            // event and THEN we start collecting items from the source.
            return Observable.Return(ListChangeStrict<T>(CollectionChangeType.Add, 0))
                .Concat(source.Select((t, idx) => ListChangeStrict(CollectionChangeType.Add, idx, t)))
                .ToLiveLinq();
        }

        /// <summary>
        /// Generates a LiveLinq query that initially has no element, and as each new source event is fired, it clears the query
        /// and then adds the value from the new source event.
        /// </summary>
        public static IListChangesStrict<T> ToLiveLinq<T>(this IObservable<T> source)
        {
            return source.Select(maybe => maybe.ToMaybe()).ToLiveLinq();
        }

        /// <summary>
        /// Generates a LiveLinq query that initially has no element, and as each new source event is fired, it clears the query
        /// and then, if the new source event has a value, it adds that value.
        /// </summary>
        public static IListChangesStrict<T> ToLiveLinq<T>(this IObservable<IMaybe<T>> source)
        {
            return source.Select(maybe => maybe.ToEnumerable()).ToLiveLinq();
        }

        /// <summary>
        /// Generates a LiveLinq query that initially has no element, and as each new source event is fired, it clears the query
        /// and then adds the items in the new source event.
        /// </summary>
        public static IListChangesStrict<T> ToLiveLinq<T>(this IObservable<IEnumerable<T>> source)
        {
            // Note: the first event should ALWAYS represent the actual state of the list at that time. If the initial state
            // of the list is empty, the first event MUST be an empty add event. So, here we first return an empty enumerable
            // event and then we start using events from source.
            source = Observable.Return(EnumerableUtility.EmptyArray<T>().AsEnumerable()).Concat(source);

            return
                source.Scan(
                    new { Previous = EnumerableUtility.EmptyArray<T>(), Current = EnumerableUtility.EmptyArray<T>() },
                    (state, newValue) => new { Previous = state.Current.ToArray(), Current = newValue.ToArray() })
                    .Select(state => Utility.ConvertStateToListChanges(state.Previous, state.Current))
                    .Concat()
                    .ToLiveLinq();
        }

        /// <summary>
        ///     Start a LiveLinq method chain. The LiveLinq extension methods only work on a
        ///     ListChanges and not on an ObservableCollection because 1) we can have
        ///     multiple observable collection types (such as BindingList, and any class that
        ///     implements INotifyCollectionChanged), 2) we don't want to have a namespacing conflict
        ///     where, depending upon the argument types, a LINQ method could be the one being used
        ///     or a LiveLinq version of it could be used. We want it to be crystal clear which
        ///     method is being called, because LiveLinq is analogous to LINQ, but the differences
        ///     are very important.
        /// </summary>
        public static IListChangesStrict<TElement> ToLiveLinq<TElement>(this ObservableCollection<TElement> coll)
        {
            return coll.ToLiveLinq(coll);
        }

        /// <summary>
        ///     Start a LiveLinq method chain. The LiveLinq extension methods only work on a
        ///     ListChanges and not on an ObservableCollection because 1) we can have
        ///     multiple observable collection types (such as BindingList, and any class that
        ///     implements INotifyCollectionChanged), 2) we don't want to have a namespacing conflict
        ///     where, depending upon the argument types, a LINQ method could be the one being used
        ///     or a LiveLinq version of it could be used. We want it to be crystal clear which
        ///     method is being called, because LiveLinq is analogous to LINQ, but the differences
        ///     are very important.
        /// </summary>
        public static IListChangesStrict<TElement> ToLiveLinq<TElement>(this IEnumerable<TElement> coll, INotifyCollectionChanged changes)
        {
            var events = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    //handler =>
                    //{
                    //    NotifyCollectionChangedEventHandler kpeHandler = (sender, e) => handler(e);
                    //    return kpeHandler;
                    //},
                    handler => changes.CollectionChanged += handler,
                    handler => changes.CollectionChanged -= handler)
                    .Select(ep => ep.EventArgs);

            return coll.ToLiveLinq(events);
        }
        
        /// <summary>
        /// Creates a LiveLinq query based on the specified enumerable and a stream of <see cref="NotifyCollectionChangedEventArgs"/>.
        /// </summary>
        public static IListChangesStrict<TElement> ToLiveLinq<TElement>(
            this IEnumerable<TElement> initialState,
            IObservable<NotifyCollectionChangedEventArgs> changes)
        {
            var result = Observable.Create(
                (IObserver<NotifyCollectionChangedEventArgs> observer) =>
                    {
                        // The first event should ALWAYS represent the actual state of the list at that time. If the initial state
                        // of the list is empty, the first event MUST be an empty add event. So, right here we generate the first
                        // event of the query, which is an add event with the current state of the list.
                        observer.OnNext(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, initialState.ToList(), 0));
                        return changes.Subscribe(observer);
                    });

            return result
                .Scan(new LiveLinqFromCollectionStateAndChange<TElement>(), Write)
                .Select(state => state.MostRecentChanges.ToObservable())
                .Concat()
                .ToLiveLinq();
        }

        private static LiveLinqFromCollectionStateAndChange<TElement> Write<TElement>(
            LiveLinqFromCollectionStateAndChange<TElement> liveLinqFromCollectionState, NotifyCollectionChangedEventArgs change)
        {
            switch (change.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var newItems = change.NewItems.Cast<TElement>();
                        return new LiveLinqFromCollectionStateAndChange<TElement>(
                            liveLinqFromCollectionState.State.InsertRange(change.NewStartingIndex, newItems),
                            ListChangeStrict(CollectionChangeType.Add, change.NewStartingIndex, newItems));
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        var oldItems = change.OldItems.Cast<TElement>().ToImmutableList();
                        return new LiveLinqFromCollectionStateAndChange<TElement>(
                            liveLinqFromCollectionState.State.RemoveRange(change.OldStartingIndex, oldItems.Count),
                            ListChangeStrict(CollectionChangeType.Remove, change.OldStartingIndex, oldItems));
                    }
                case NotifyCollectionChangedAction.Replace:
                    {
                        var oldItems = change.OldItems.Cast<TElement>().ToImmutableList();
                        var newItems = change.NewItems.Cast<TElement>();
                        return new LiveLinqFromCollectionStateAndChange<TElement>(
                            liveLinqFromCollectionState.State
                                .RemoveRange(change.OldStartingIndex, oldItems.Count)
                                .InsertRange(change.NewStartingIndex, newItems),
                            ImmutableList.Create(
                                ListChangeStrict(CollectionChangeType.Remove, change.OldStartingIndex, oldItems),
                                ListChangeStrict(CollectionChangeType.Add, change.NewStartingIndex, newItems)));
                    }
                case NotifyCollectionChangedAction.Move:
                    {
                        var items = change.OldItems.Cast<TElement>().ToImmutableList();
                        return new LiveLinqFromCollectionStateAndChange<TElement>(
                            liveLinqFromCollectionState.State
                                .RemoveRange(change.OldStartingIndex, items.Count)
                                .InsertRange(change.NewStartingIndex, items),
                            ImmutableList.Create(
                                ListChangeStrict(CollectionChangeType.Remove, change.OldStartingIndex, items),
                                ListChangeStrict(CollectionChangeType.Add, change.NewStartingIndex, items)));
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        return new LiveLinqFromCollectionStateAndChange<TElement>(
                            ImmutableList<TElement>.Empty,
                            ImmutableList.Create(
                                ListChangeStrict(CollectionChangeType.Remove, 0, liveLinqFromCollectionState.State)));
                    }
                default:
                    throw new ArgumentException($"Unknown list change action: {change.Action}");
            }
        }

        private class LiveLinqFromCollectionStateAndChange<TElement>
        {
            public ImmutableList<TElement> State { get; }
            public ImmutableList<IListChangeStrict<TElement>> MostRecentChanges { get; }

            public LiveLinqFromCollectionStateAndChange()
            {
                State = ImmutableList<TElement>.Empty;
                this.MostRecentChanges = ImmutableList<IListChangeStrict<TElement>>.Empty;
            }

            public LiveLinqFromCollectionStateAndChange(ImmutableList<TElement> state)
            {
                this.State = state;
                this.MostRecentChanges = ImmutableList<IListChangeStrict<TElement>>.Empty;
            }

            public LiveLinqFromCollectionStateAndChange(ImmutableList<TElement> state, ImmutableList<IListChangeStrict<TElement>> mostRecentChanges)
            {
                this.State = state;
                this.MostRecentChanges = mostRecentChanges;
            }

            public LiveLinqFromCollectionStateAndChange(ImmutableList<TElement> state, IListChangeStrict<TElement> mostRecentChange)
            {
                this.State = state;
                this.MostRecentChanges = ImmutableList.Create(mostRecentChange);
            }
        }

        /// <summary>
        ///     Start a LiveLinq method chain. The LiveLinq extension methods only work on a
        ///     ListChanges and not on an ObservableCollection because 1) we can have
        ///     multiple observable collection types (such as BindingList, and any class that
        ///     implements INotifyCollectionChanged), 2) we don't want to have a namespacing conflict
        ///     where, depending upon the argument types, a LINQ method could be the one being used
        ///     or a LiveLinq version of it could be used. We want it to be crystal clear which
        ///     method is being called, because LiveLinq is analogous to LINQ, but the differences
        ///     are very important.
        ///     This extension method produces an IListChanges that never sends collection changed events.
        /// </summary>
        public static IListChangesStrict<TElement> ToLiveLinqUnchangingList<TElement>(this IEnumerable<TElement> source)
        {
            return Observable.Return(source).ToLiveLinq();
        }

        /// <summary>
        ///     Start a LiveLinq method chain. The LiveLinq extension methods only work on a
        ///     ListChanges and not on an ObservableCollection because 1) we can have
        ///     multiple observable collection types (such as BindingList, and any class that
        ///     implements INotifyCollectionChanged), 2) we don't want to have a namespacing conflict
        ///     where, depending upon the argument types, a LINQ method could be the one being used
        ///     or a LiveLinq version of it could be used. We want it to be crystal clear which
        ///     method is being called, because LiveLinq is analogous to LINQ, but the differences
        ///     are very important.
        ///
        ///     Note: the first event should ALWAYS represent the actual state of the list at that time. If the initial state
        ///     of the list is empty, the first event MUST be an empty add event.
        /// </summary>
        public static IListChanges<T> ToLiveLinq<T>(this IObservable<IListChange<T>> listChanges)
        {
            return new ListChanges<T>(listChanges);
        }

        /// <summary>
        ///     Start a LiveLinq method chain. The LiveLinq extension methods only work on a
        ///     ListChanges and not on an ObservableCollection because 1) we can have
        ///     multiple observable collection types (such as BindingList, and any class that
        ///     implements INotifyCollectionChanged), 2) we don't want to have a namespacing conflict
        ///     where, depending upon the argument types, a LINQ method could be the one being used
        ///     or a LiveLinq version of it could be used. We want it to be crystal clear which
        ///     method is being called, because LiveLinq is analogous to LINQ, but the differences
        ///     are very important.
        ///
        ///     Note: the first event should ALWAYS represent the actual state of the list at that time. If the initial state
        ///     of the list is empty, the first event MUST be an empty add event.
        /// </summary>
        public static IListChangesStrict<T> ToLiveLinq<T>(this IObservable<IListChangeStrict<T>> listChanges)
        {
            return new ListChangesStrict<T>(listChanges);
        }

        #endregion
    }
}
