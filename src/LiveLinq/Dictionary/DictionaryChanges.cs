using System;

using SimpleMonads;

using LiveLinq.List;

using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    internal class DictionaryChanges<TKey, TValue> : IDictionaryChanges<TKey, TValue>
    {
        private readonly IObservable<IDictionaryChange<TKey, TValue>> _observable;

        public DictionaryChanges(IObservable<IDictionaryChange<TKey, TValue>> observable)
        {
            this._observable = observable;
        }

        public IObservable<IDictionaryChange<TKey, TValue>> AsObservable()
        {
            return _observable;
        }
        
        public IObservable<IMaybe<TValue>> this[TKey key] => Utility.GetAtKey(this, key);

        public IObservable<IMaybe<TValue>> this[IObservable<TKey> keys] => Utility.GetAtKey(this, keys);

        public IObservable<IMaybe<TValue>> this[IObservable<IMaybe<TKey>> keys] => Utility.GetAtKey(this, keys);
    }
}
