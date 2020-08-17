using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Subjects;
using ComposableCollections;
using GenericNumbers.Relational;
using ComposableCollections.Dictionary;
using LiveLinq.Core;
using LiveLinq.List;
using MoreCollections;
using static LiveLinq.Utility;

namespace LiveLinq.Dictionary
{
    /// <summary>
    /// A sorted observable dictionary/list combination.
    /// </summary>
    public class SortedObservableDictionaryList<TKey, TValue> : ObservableListBase<IKeyValue<TKey, TValue>>, IDictionary<TKey, TValue>
    {
        private readonly Func<TKey, TKey, int> _comparer;

        /// <summary>
        /// Creates a new sorted observable dictionary list, with the specified ordering.
        /// </summary>
        /// <param name="comparer">Specifies how the items will be sorted.</param>
        public SortedObservableDictionaryList(Func<TKey, TKey, int> comparer = null)
        {
            if (comparer == null) comparer = (key1, key2) => key1.CompareTo(key2);

            _comparer = comparer;
        }

        #region ObservableListBase implementation

        private readonly object _lock = new object();
        private ImmutableList<IKeyValue<TKey, TValue>> _internalList = ImmutableList<IKeyValue<TKey, TValue>>.Empty;
        private readonly Subject<IListChangeStrict<IKeyValue<TKey, TValue>>> _changes = new Subject<IListChangeStrict<IKeyValue<TKey, TValue>>>();

        /// <summary>
        /// Gets an enumerator that will enumerate all the key/value pairs in this dictionary.
        /// </summary>
        public override IEnumerator<IKeyValue<TKey, TValue>> GetEnumerator()
        {
            return this._internalList.GetEnumerator();
        }

        /// <summary>
        /// Copies the elements of this dictionary to the specified array, starting at the specified index.
        /// </summary>
        public override void CopyTo(IKeyValue<TKey, TValue>[] array, int arrayIndex)
        {
            this._internalList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of items in this dictionary.
        /// </summary>
        public override int Count => this._internalList.Count;

        /// <summary>
        /// Returns the index of the specified key if the key exists in this dictionary and the value at that key is the specified value.
        /// Otherwise, returns -1.
        /// </summary>
        public override int IndexOf(IKeyValue<TKey, TValue> item)
        {
            return _internalList.BinarySearch(kvp => kvp.CompareTo(item));
        }

        /// <summary>
        /// Returns the index of the specified key, or -1 if the key isn't in this dictionary.
        /// </summary>
        public int IndexOf(TKey key)
        {
            return _internalList.BinarySearch(kvp => kvp.Key.CompareTo(key));
        }

        /// <summary>
        /// Returns the key/value pair at the specified index
        /// </summary>
        protected override IKeyValue<TKey, TValue> GetAt(int index)
        {
            return this._internalList[index];
        }

        /// <summary>
        /// Implement this to define the IObservable that becomes a livelinq query whenever someone calls this.LiveLinq().
        /// </summary>
        protected override IDisposable ToLiveLinqSubscribe(IObserver<IListChangeStrict<IKeyValue<TKey, TValue>>> observer)
        {
            lock (_lock)
            {
                if (Count > 0)
                    observer.OnNext(ListChangeStrict(CollectionChangeType.Add, 0, _internalList));
                return _changes.Subscribe(observer);
            }
        }

        #endregion

        #region IDictionary implementation

        private readonly Dictionary<TKey, TValue> _internalDictionary = new Dictionary<TKey, TValue>();

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return _internalDictionary.GetEnumerator();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return Contains(item.ToKeyValue());
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            for (var i = arrayIndex; i < array.Length; i++)
            {
                array[i] = this[i - arrayIndex].ToKeyValuePair();
            }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param><param name="value">The object to use as the value of the element to add.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        public void Add(TKey key, TValue value)
        {
            Add(new KeyValue<TKey, TValue>(key, value));
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise, false.
        /// </returns>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool ContainsKey(TKey key)
        {
            return _internalDictionary.ContainsKey(key);
        }
        
        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <param name="key">The key whose value to get.</param><param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (ContainsKey(key))
            {
                value = _internalDictionary[key];
                return true;
            }
            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(new KeyValue<TKey, TValue>(item.Key, item.Value));
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(new KeyValue<TKey, TValue>(item.Key, item.Value));
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        /// <param name="key">The key of the element to get or set.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key"/> is not found.</exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        public TValue this[TKey key]
        {
            get
            {
                return _internalDictionary[key];
            }
            set
            {
                Add(new KeyValue<TKey, TValue>(key, value));
            }
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        /// <param name="key">The key of the element to remove.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        public bool Remove(TKey key)
        {
            return Remove(new KeyValue<TKey, TValue>(key, this[key]));
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<TKey> Keys => _internalDictionary.Keys;

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<TValue> Values => _internalDictionary.Values;

        #endregion

        #region Core mutating functions

        /// <summary>
        /// Insert the specified range of items, in the order that they are specified, starting at the specified index.
        /// </summary>
        protected override void UnlockedInsertRange(int index, IReadOnlyList<IKeyValue<TKey, TValue>> items)
        {
            for (var i = 0; i < items.Count; i++)
            {
                var sortedIndex = _internalList.GetIndexOfSortedInsert(items[i], (a, b) => _comparer(a.Key, b.Key));
                this._internalList = this._internalList.Insert(sortedIndex, items[i]);
                this._internalDictionary.Add(items[i].Key, items[i].Value);
                this._changes.OnNext(ListChangeStrict(CollectionChangeType.Add, sortedIndex, items[i]));
            }
        }

        /// <summary>
        /// Remove the specified range of items from this list.
        /// </summary>
        protected override IReadOnlyList<IKeyValue<TKey, TValue>>  UnlockedRemoveRange(int index, int count)
        {
            var removedItems = new List<IKeyValue<TKey, TValue>>();
            for (var i = 0; i < count; i++)
            {
                removedItems.Add(this[index]);
                _internalDictionary.Remove(this[index].Key);
            }
            this._internalList = _internalList.RemoveRange(index, count);
            this._changes.OnNext(ListChangeStrict(CollectionChangeType.Remove, index, removedItems.ToImmutableList()));
            return removedItems;
        }

        #endregion
    }
}
