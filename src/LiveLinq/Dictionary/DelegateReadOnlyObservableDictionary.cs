using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.Base;
using LiveLinq.Dictionary.Interfaces;
using SimpleMonads;

namespace LiveLinq.Dictionary
{
    public class DelegateReadOnlyObservableDictionary<TKey, TValue> : DelegateReadOnlyDictionaryBase<TKey, TValue>, IObservableReadOnlyDictionary<TKey, TValue>
    {
        private IObservableReadOnlyDictionary<TKey, TValue> _wrapped;

        public DelegateReadOnlyObservableDictionary(IObservableReadOnlyDictionary<TKey, TValue> wrapped) : base(wrapped)
        {
            _wrapped = wrapped;
        }

        protected DelegateReadOnlyObservableDictionary()
        {
        }

        public void Dispose()
        {
            _wrapped.Dispose();
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _wrapped.ToLiveLinq();
        }
        
        protected void Initialize(IObservableDictionary<TKey, TValue> wrapped)
        {
            _wrapped = wrapped;
            base.Initialize(wrapped);
        }
    }
}