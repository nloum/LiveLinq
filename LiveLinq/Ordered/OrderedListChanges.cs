using System;

using SimpleMonads;
using MoreCollections;
using LiveLinq.List;

namespace LiveLinq.Ordered
{
    internal class OrderedListChanges<T> : IOrderedListChanges<T>
    {
        public Func<T, T, int> Comparer { get; }

        private readonly IObservable<IListChangeStrict<T>> _changes;

        /// <summary>
        /// Construct a wrapper object that makes an <see cref="IReadOnlyObservableList{T}"/>
        /// into an <see cref="IListChanges{T}"/>.
        /// </summary>
        /// <param name="readOnlyList">The object to wrap</param>
        public OrderedListChanges(IObservable<IListChangeStrict<T>> changes, Func<T, T, int> comparer)
        {
            this.Comparer = comparer;
            if (changes == null)
                throw new ArgumentNullException(nameof(changes));
            this._changes = changes;
        }

        public IObservable<IListChange<T>> AsObservable()
        {
            return this._changes;
        }

        IObservable<IListChangeStrict<T>> IListChangesStrict<T>.AsObservable()
        {
            return this._changes;
        }

        public IListChanges<TTarget> OfType<TTarget>()
        {
            return this.Where(t1 => t1 is TTarget).Cast<TTarget>();
        }

        public IListChanges<TTarget> Cast<TTarget>()
        {
            return this.Select(t1 =>
            {
                var t2 = (TTarget)(object)t1;
                return t2;
            });
        }

        IObservable<IMaybe<T>> IListChanges<T>.this[int index] => Utility.GetAtIndex(this, index);

        IObservable<IMaybe<T>> IListChanges<T>.this[IObservable<int> index] => Utility.GetAtIndex(this, index);
    }
}
