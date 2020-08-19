using System;
using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    public interface IObservableDictionary<TKey, TValue> : IDisposableDictionary<TKey, TValue>, IReadOnlyObservableDictionary<TKey, TValue>
    {
    }
}