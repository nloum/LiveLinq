using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using LiveLinq.Core;

namespace LiveLinq.List.Adapters
{
    public class BindableListAdapter<TItem> : IReadOnlyList<TItem>, INotifyCollectionChanged, IDisposable
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }
        
        private readonly IReadOnlyObservableList<TItem> _items;
        private readonly IDisposable _subscription;

        public BindableListAdapter(IReadOnlyObservableList<TItem> items)
        {
            _items = items;

            _subscription = _items.ToLiveLinq().AsObservable().Subscribe(listChange =>
            {
                if (listChange.Type == CollectionChangeType.Add)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, listChange.Values));
                }
                else
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, listChange.Values));
                }
            });
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public int Count => _items.Count;

        public TItem this[int index] => _items[index];

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}