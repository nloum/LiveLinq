using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LiveLinq.Core;
using UtilityDisposables;

namespace LiveLinq.Set
{
    public class ObservableSet<T> : ProtectedDisposableCollector, IObservableReadOnlySet<T>
    {
        private readonly object _lock = new object();
        private ImmutableHashSet<T> _source = ImmutableHashSet<T>.Empty;
        private readonly Subject<ISetChange<T>> _setChanges = new Subject<ISetChange<T>>();

        public ObservableSet()
        {
        }

        public int Count => _source.Count;

        public void AddRange(IEnumerable<T> elements)
        {
            lock (_lock)
            {
                var elementsList = elements.ToImmutableArray();
                foreach (var item in elementsList)
                {
                    _source = _source.Add(item);
                }
                _setChanges.OnNext(Utility.SetChange<T>(Core.CollectionChangeType.Add, elementsList));
            }
        }

        public void RemoveRange(IEnumerable<T> elements)
        {
            lock (_lock)
            {
                var elementsList = elements.ToImmutableArray();
                foreach (var item in elementsList)
                {
                    _source = _source.Remove(item);
                }
                _setChanges.OnNext(Utility.SetChange<T>(Core.CollectionChangeType.Remove, elementsList));
            }
        }

        public void Add(T element)
        {
            lock (_lock)
            {
                _source = _source.Add(element);
                _setChanges.OnNext(Utility.SetChange<T>(Core.CollectionChangeType.Add, element));
            }
        }

        public void Remove(T element)
        {
            lock (_lock)
            {
                _source = _source.Remove(element);
                _setChanges.OnNext(Utility.SetChange<T>(Core.CollectionChangeType.Remove, element));
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        public ISetChanges<T> ToLiveLinq()
        {
            lock (_lock)
            {
                if (Count > 0)
                {
                    return new SetChanges<T>(Observable.Return(Utility.SetChange(CollectionChangeType.Add, _source.AsEnumerable())).Concat(_setChanges));
                }
            
                return new SetChanges<T>(_setChanges);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
