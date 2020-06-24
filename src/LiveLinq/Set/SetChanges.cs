using System;
using System.Linq;
using System.Reactive.Linq;
using static LiveLinq.Utility;

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
            return _source.Select(x => SetChange(x.Type, x.Values.Cast<TTarget>())).ToLiveLinq();
        }

        public ISetChanges<TTarget> OfType<TTarget>()
        {
            return this.Where(x => x is TTarget).AsObservable().Select(x => SetChange(x.Type, x.Values.Cast<TTarget>())).ToLiveLinq();
        }
    }
}
