using System;
using System.Collections;
using System.Collections.Generic;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.Sources;
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

        private readonly ObservableDictionaryAdapter<TKey, TValue> _wrapped;

        public ObservableDictionary() : this(new ObservableDictionaryAdapter<TKey, TValue>(new ConcurrentDictionary<TKey, TValue>()))
        {
        }

        private ObservableDictionary(ObservableDictionaryAdapter<TKey, TValue> wrapped) : base(wrapped)
        {
            _wrapped = wrapped;
        }

        public void Dispose()
        {
            AssociatedSubscriptions.Dispose();
        }
    }
}