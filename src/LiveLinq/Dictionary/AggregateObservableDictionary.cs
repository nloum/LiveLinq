using System.Collections.Generic;
using System.Linq;

namespace LiveLinq.Dictionary
{
    public class AggregateObservableDictionary<TKey, TValue> : AggregateObservableDictionaryBase<TKey, TValue>
    {
        private readonly IObservableDictionary<TKey, TValue> _mutationsGoHere;

        public AggregateObservableDictionary(IObservableDictionary<TKey, TValue> mutationsGoHere, IEnumerable<IReadOnlyObservableDictionary<TKey, TValue>> wrapped, bool includeWrappedInResults) : base(wrapped.Concat(includeWrappedInResults ? new[]{mutationsGoHere} : Enumerable.Empty<IReadOnlyObservableDictionary<TKey,TValue>>()))
        {
            _mutationsGoHere = mutationsGoHere;
        }

        protected override void AddInternal(TKey key, TValue value)
        {
            _mutationsGoHere.Add(key, value);
        }

        protected override void RemoveInternal(TKey key)
        {
            _mutationsGoHere.Remove(key);
        }
    }
}