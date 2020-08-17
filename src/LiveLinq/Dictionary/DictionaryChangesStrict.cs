using System;

using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    internal class DictionaryChangesStrict<TKey, TValue> : DictionaryChanges<TKey, TValue>, IDictionaryChangesStrict<TKey, TValue>
    {
        private readonly IObservable<IDictionaryChangeStrict<TKey, TValue>> _observable;

        public DictionaryChangesStrict(IObservable<IDictionaryChangeStrict<TKey, TValue>> observable)
            : base(observable)
        {
            this._observable = observable;
        }

        new public IObservable<IDictionaryChangeStrict<TKey, TValue>> AsObservable()
        {
            return this._observable;
        }
    }
}
