using System;
using SimpleMonads;

using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    /// <summary>
    /// A dictionary-style LiveLinq query. This is created by the LiveLinq ToDictionary extension method.
    /// </summary>
    public interface IDictionaryChanges<TKey, out TValue>
    {
        /// <summary>
        /// An event stream of changes that, when applied to a list, result in a list that represents the most
        /// recent state of the query.
        /// 
        /// Note: the first event should ALWAYS represent the actual state of the list at that time. If the initial state
        /// of the list is empty, the first event will be an empty add event.
        /// </summary>
        IObservable<IDictionaryChange<TKey, TValue>> AsObservable();
        
        IObservable<IMaybe<TValue>> this[TKey key] { get; }
        IObservable<IMaybe<TValue>> this[IObservable<TKey> keys] { get; }
        IObservable<IMaybe<TValue>> this[IObservable<IMaybe<TKey>> keys] { get; }
    }
}
