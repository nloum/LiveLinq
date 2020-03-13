using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Reactive.Subjects;
using LiveLinq.Core;
using static LiveLinq.Utility;

namespace LiveLinq.List
{
    /// <summary>
    /// Class that implements an observable list that has low-overhead support for creating LiveLinq queries.
    /// Basically this is like ObservableCollection, but because we have low-level access to the underlying list,
    /// we can guarantee that calling LiveLinq() on this object won't skip changes that are made while the call is happening
    /// without having to maintain the entire state of the collection, which is what happens when you call .LiveLinq()
    /// on an ObservableCollection.
    /// </summary>
    /// <remarks>
    /// This class can be used for bindings, because it implements INotifyCollectionChanged.
    /// </remarks>
    public class ObservableList<T> : ObservableListBase<T>
    {
        private ImmutableList<T> _internalList = ImmutableList<T>.Empty;
        private readonly Subject<IListChangeStrict<T>> _changes = new Subject<IListChangeStrict<T>>();

        public override IEnumerator<T> GetEnumerator()
        {
            return this._internalList.GetEnumerator();
        }

        public override void CopyTo(T[] array, int arrayIndex)
        {
            this._internalList.CopyTo(array, arrayIndex);
        }

        public override int Count => this._internalList.Count;

        public override int IndexOf(T item)
        {
            return this._internalList.IndexOf(item);
        }

        protected override T GetAt(int index)
        {
            return this._internalList[index];
        }

        protected override IDisposable ToLiveLinqSubscribe(IObserver<IListChangeStrict<T>> observer)
        {
            lock (SyncRoot)
            {
                if (Count > 0)
                {
                    observer.OnNext(ListChangeStrict(CollectionChangeType.Add, 0, _internalList));
                }
                else
                {
                    observer.OnNext(Utility<T>.EmptyChange);
                }
                return _changes.Subscribe(observer);
            }
        }

        protected override void UnlockedInsertRange(int index, IReadOnlyList<T> items)
        {
            this._internalList = this._internalList.InsertRange(index, items);
            this._changes.OnNext(ListChangeStrict<T>(CollectionChangeType.Add, index, items));
            var eventChange = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)items, index);
            OnCollectionChanged(eventChange);
        }

        protected override IReadOnlyList<T> UnlockedRemoveRange(int index, int count)
        {
            var removedItems = new List<T>();
            for (var i = 0; i < count; i++)
            {
                removedItems.Add(this[index + i]);
            }
            this._internalList = _internalList.RemoveRange(index, count);
            this._changes.OnNext(ListChangeStrict(CollectionChangeType.Remove, index, removedItems.ToImmutableList()));
            var eventChange = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItems, index);
            OnCollectionChanged(eventChange);
            return removedItems;
        }
    }
}
