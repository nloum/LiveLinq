using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreCollections;
using SimpleMonads;

namespace LiveLinq.Dictionary
{
    public class DelegateReadOnlyObservableDictionary<TKey, TValue> : DelegateReadOnlyDictionaryEx<TKey, TValue>, IReadOnlyObservableDictionary<TKey, TValue>
    {
        private IReadOnlyObservableDictionary<TKey, TValue> _wrapped;

        public DelegateReadOnlyObservableDictionary(IReadOnlyObservableDictionary<TKey, TValue> wrapped) : base(wrapped)
        {
            _wrapped = wrapped;
        }

        protected DelegateReadOnlyObservableDictionary()
        {
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _wrapped.ToLiveLinq();
        }
        
        protected virtual void Initialize(IObservableDictionary<TKey, TValue> wrapped)
        {
            _wrapped = wrapped;
            base.Initialize(wrapped);
        }
    }
}