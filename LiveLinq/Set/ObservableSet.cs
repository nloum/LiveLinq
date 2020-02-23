using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace LiveLinq.Set
{
    public class ObservableSet<T> : IReadOnlyObservableSet<T>
    {
        private readonly HashSet<T> _source = new HashSet<T>();
        private readonly Subject<ISetChange<T>> _setChanges = new Subject<ISetChange<T>>();

        public ObservableSet()
        {
        }

        public void AddRange(IEnumerable<T> elements)
        {
            var elementsList = elements.ToImmutableArray();
            foreach (var item in elementsList)
            {
                _source.Add(item);
            }
            _setChanges.OnNext(Utility.SetChange<T>(Core.CollectionChangeType.Add, elementsList));
        }

        public void RemoveRange(IEnumerable<T> elements)
        {
            var elementsList = elements.ToImmutableArray();
            foreach (var item in elementsList)
            {
                _source.Add(item);
            }
            _setChanges.OnNext(Utility.SetChange<T>(Core.CollectionChangeType.Remove, elementsList));
        }

        public void Add(T element)
        {
            _source.Add(element);
            _setChanges.OnNext(Utility.SetChange<T>(Core.CollectionChangeType.Add, element));
        }

        public void Remove(T element)
        {
            _source.Remove(element);
            _setChanges.OnNext(Utility.SetChange<T>(Core.CollectionChangeType.Remove, element));
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        public ISetChanges<T> ToLiveLinq()
        {
            return new SetChanges<T>(_setChanges);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
