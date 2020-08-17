using System;
using LiveLinq.List;

using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    /// <summary>
    /// A LiveLinq query where each item is a key/value pair.
    /// </summary>
    public interface IDictionaryChangesStrict<TKey, out TValue> : IDictionaryChanges<TKey, TValue>
    {
        new IObservable<IDictionaryChangeStrict<TKey, TValue>> AsObservable();
    }
}
