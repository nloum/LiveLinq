using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;

namespace LiveLinq.Dictionary
{
    public abstract class AggregateObservableDictionaryBase<TKey, TValue> : ObservableDictionaryBaseWithAbstractLiveLinq<TKey, TValue>
    {
        private readonly ImmutableList<IReadOnlyObservableDictionary<TKey, TValue>> _wrapped;

        protected AggregateObservableDictionaryBase(IEnumerable<IReadOnlyObservableDictionary<TKey, TValue>> wrapped)
        {
            _wrapped = wrapped.ToImmutableList();
        }

        public override IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return Observable.Merge(_wrapped.Select(x => x.ToLiveLinq().AsObservable())).ToLiveLinq();
        }

        public override bool ContainsKey(TKey key)
        {
            return _wrapped.Any(x => x.ContainsKey(key));
        }

        public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _wrapped.SelectMany(x => x).GetEnumerator();
        }

        public override int Count => _wrapped.Sum(x => x.Count);
        public override bool TryGetValue(TKey key, out TValue value)
        {
            foreach (var item in _wrapped)
            {
                if (item.TryGetValue(key, out value))
                {
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}