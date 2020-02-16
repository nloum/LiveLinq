using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveLinq.Set
{
    internal class SetChanges<T> : ISetChanges<T>
    {
        private readonly IObservable<ISetChange<T>> _source;

        public SetChanges(IObservable<ISetChange<T>> source)
        {
            _source = source;
        }

        public IObservable<ISetChange<T>> AsObservable()
        {
            return _source;
        }

        public ISetChanges<TTarget> Cast<TTarget>()
        {
            return _source.Cast<ISetChange<TTarget>>().ToLiveLinq();
        }

        public ISetChanges<TTarget> OfType<TTarget>()
        {
            return _source.OfType<ISetChange<TTarget>>().ToLiveLinq();
        }
    }
}
