using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using GenericNumbers;
using MoreCollections;
using UtilityDisposables;
using LiveLinq.Core;

namespace LiveLinq.List
{
    /// <summary>
    /// Base class for implementing an observable list that has low-overhead support for creating LiveLinq queries.
    /// Basically this is like ObservableCollection, but because we have low-level access to the underlying list,
    /// we can guarantee that calling LiveLinq() on this object won't skip changes that are made while the call is happening
    /// without having to maintain the entire state of the collection, which is what happens when you call .LiveLinq()
    /// on an ObservableCollection.
    /// </summary>
    public abstract class ObservableListBase<T> : ProtectedDisposableCollector, IReadOnlyObservableList<T>, IList<T>, IList
    {
        private readonly IListChangesStrict<T> _observableLinq;

        protected ObservableListBase()
        {
            this._observableLinq = Observable
                .Create<IListChangeStrict<T>>(observer => this.ToLiveLinqSubscribe(observer))
                .ToLiveLinq();
        }

        #region Observable stuff

        protected abstract IDisposable ToLiveLinqSubscribe(IObserver<IListChangeStrict<T>> observer);

        public IListChangesStrict<T> ToLiveLinq()
        {
            return this._observableLinq;
        }

        #endregion

        #region IList stuff

        #region Non-mutators

        public abstract IEnumerator<T> GetEnumerator();

        public abstract void CopyTo(T[] array, int arrayIndex);

        public abstract int Count { get; }

        public abstract int IndexOf(T item);

        protected abstract T GetAt(int index);

        public bool Contains(T item)
        {
            return this.IndexOf(item) != -1;
        }

        public bool IsReadOnly => false;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Mutating functions

        public void Add(T item)
        {
            this.Insert(this.Count, item);
        }

        public void Clear()
        {
            this.RemoveRange(0, this.Count);
        }

        public bool Remove(T item)
        {
            lock (SyncRoot)
            {
                var index = this.IndexOf(item);
                if (index < 0) return false;
                this.RemoveRange(index, 1);
                return true;
            }
        }

        public T this[int index]
        {
            get
            {
                return this.GetAt(index);
            }
            set
            {
                lock (SyncRoot)
                {
                    this.RemoveAt(index);
                    this.Insert(index, value);
                }
            }
        }

        public void Move(int oldIndex, int newIndex)
        {
            MoveRange(oldIndex, 1, newIndex);
        }

        public void Insert(int index, T item)
        {
            this.InsertRange(index, item);
        }

        public void RemoveAt(int index)
        {
            this.RemoveRange(index, 1);
        }

        public void RemoveRange(INumberRange<int> range)
        {
            this.RemoveRange(range.LowerBound.ChangeStrictness(false).Value, range.Size);
        }

        public void InsertRange(int index, IEnumerable<T> items)
        {
            InsertRange(index, items.ToArray());
        }

        #region Core mutating functions -- all the other mutating functions call these

        public void MoveRange(int oldIndex, int count, int newIndex)
        {
            lock (SyncRoot)
            {
                UnlockedMoveRange(oldIndex, count, newIndex);
            }
        }

        public void InsertRange(int index, params T[] items)
        {
            lock (SyncRoot)
            {
                UnlockedInsertRange(index, items);
            }
        }

        public void RemoveRange(int index, int count)
        {
            lock (SyncRoot)
            {
                UnlockedRemoveRange(index, count);
            }
        }

        protected virtual void UnlockedMoveRange(int oldIndex, int count, int newIndex)
        {
            var removedItems = UnlockedRemoveRange(oldIndex, count);
            UnlockedInsertRange(newIndex, removedItems);
        }

        protected abstract void UnlockedInsertRange(int index, IReadOnlyList<T> items);

        protected abstract IReadOnlyList<T> UnlockedRemoveRange(int index, int count);

        #endregion

        #endregion

        #endregion

        #region Misc implementations

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing. </param><param name="index">The zero-based index in <paramref name="array"/> at which copying begins. </param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null. </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero. </exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or- The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>.-or-The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
        public void CopyTo(Array array, int index)
        {
            var i = index;
            foreach (var element in this.Take(array.Length - index))
            {
                array.SetValue(element, i);
                i++;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// true if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, false.
        /// </returns>
        public bool IsSynchronized => true;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        public object SyncRoot { get; } = new object();

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <returns>
        /// The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the collection.
        /// </returns>
        /// <param name="value">The object to add to the <see cref="T:System.Collections.IList"/>. </param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
        public int Add(object value)
        {
            lock (SyncRoot)
            {
                var count = Count;
                Insert(count, (T)value);
                return count;
            }
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.IList"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Object"/> is found in the <see cref="T:System.Collections.IList"/>; otherwise, false.
        /// </returns>
        /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList"/>. </param>
        public bool Contains(object value)
        {
            return Contains((T)value);
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <returns>
        /// The index of <paramref name="value"/> if found in the list; otherwise, -1.
        /// </returns>
        /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList"/>. </param>
        public int IndexOf(object value)
        {
            if (!(value is T))
            {
                return -1;
            }

            return IndexOf((T)value);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.IList"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="value"/> should be inserted. </param><param name="value">The object to insert into the <see cref="T:System.Collections.IList"/>. </param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>. </exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception><exception cref="T:System.NullReferenceException"><paramref name="value"/> is null reference in the <see cref="T:System.Collections.IList"/>.</exception>
        public void Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <param name="value">The object to remove from the <see cref="T:System.Collections.IList"/>. </param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
        public void Remove(object value)
        {
            Remove((T)value);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.IList"/> has a fixed size; otherwise, false.
        /// </returns>
        public bool IsFixedSize => false;

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get or set. </param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>. </exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.IList"/> is read-only. </exception>
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (T)value;
            }
        }

        #endregion

        /// <summary>
        /// Applies the list change to this list.
        /// </summary>
        public void Write(IListChange<T> change)
        {
            var range = change.Range.ChangeStrictness(false, true);
            switch (change.Type)
            {
                case CollectionChangeType.Add:
                    {
                        InsertRange(range.LowerBound.Value, change.Values);
                        break;
                    }
                case CollectionChangeType.Remove:
                    {
                        RemoveRange(range);
                        break;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, e);
        }
    }
}
