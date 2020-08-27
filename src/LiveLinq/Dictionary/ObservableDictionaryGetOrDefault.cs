using System;
using System.Collections;
using System.Collections.Generic;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.Decorators;
using ComposableCollections.Dictionary.Sources;
using SimpleMonads;
using UtilityDisposables;

namespace LiveLinq.Dictionary
{
    /// <summary>
    /// An observable dictionary that allows you to add items when they are requested.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <remarks>
    /// This class is thread-safe.
    /// </remarks>
    public class ObservableDictionaryGetOrDefault<TKey, TValue> : DelegateObservableDictionary<TKey, TValue>, IDisposable
    {
        internal DisposableCollector AssociatedSubscriptions { get; } = new DisposableCollector();

        public ObservableDictionaryGetOrDefault(GetDefaultValue<TKey, TValue> getDefaultValue)
        {
            Initialize(new ObservableDictionaryDecorator<TKey, TValue>(new DictionaryGetOrDefaultDecorator<TKey, TValue>(new ConcurrentDictionary<TKey, TValue>(), getDefaultValue)));
        }
        
        public ObservableDictionaryGetOrDefault(GetDefaultValueWithOptionalPersistence<TKey, TValue> getDefaultValue)
        {
            Initialize(new ObservableDictionaryDecorator<TKey, TValue>(new DictionaryGetOrDefaultDecorator<TKey, TValue>(new ConcurrentDictionary<TKey, TValue>(), getDefaultValue)));
        }

        public void Dispose()
        {
            AssociatedSubscriptions.Dispose();
        }
    }
}