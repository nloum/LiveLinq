using System;
using System.Collections;
using System.Collections.Generic;
using MoreCollections;
using SimpleMonads;
using UtilityDisposables;

namespace LiveLinq.Dictionary
{
    /// <summary>
    /// This class provides a dictionary that can efficiently have LiveLinq run on it.
    /// </summary>
    /// <typeparam name="TKey">The dictionary key type</typeparam>
    /// <typeparam name="TValue">The dictionary value type</typeparam>
    /// <remarks>
    /// This class is thread-safe.
    /// </remarks>
    public class ObservableDictionary<TKey, TValue> : DelegateObservableDictionary<TKey, TValue>, IDisposable
    {
        internal DisposableCollector AssociatedSubscriptions { get; } = new DisposableCollector();

        private readonly ObservableDictionaryDecorator<TKey, TValue> _wrapped = new ObservableDictionaryDecorator<TKey, TValue>(new ConcurrentDictionaryEx<TKey, TValue>());

        public ObservableDictionary()
        {
            Initialize(_wrapped);
        }

        public void Dispose()
        {
            AssociatedSubscriptions.Dispose();
        }
    }
}