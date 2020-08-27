using System;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary
{
    public interface IObservableDictionary<TKey, TValue> : IDisposableDictionary<TKey, TValue>, IReadOnlyObservableDictionary<TKey, TValue>
    {
    }
}