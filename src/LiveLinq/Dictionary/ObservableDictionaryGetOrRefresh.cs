using System;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.Decorators;
using ComposableCollections.Dictionary.Sources;
using UtilityDisposables;

namespace LiveLinq.Dictionary
{
    /// <summary>
    /// An observable dictionary that allows you to refresh items when they are requested.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <remarks>
    /// This class is thread-safe.
    /// </remarks>
    public class ObservableDictionaryGetOrRefresh<TKey, TValue> : DelegateObservableDictionary<TKey, TValue>, IDisposable
    {
        internal DisposableCollector AssociatedSubscriptions { get; } = new DisposableCollector();

        public ObservableDictionaryGetOrRefresh(RefreshValue<TKey, TValue> refreshValue)
        {
            Initialize(new ObservableDictionaryDecorator<TKey, TValue>(new DictionaryGetOrRefreshDecorator<TKey, TValue>(new ConcurrentDictionary<TKey, TValue>(), refreshValue)));
        }

        public ObservableDictionaryGetOrRefresh(RefreshValueWithOptionalPersistence<TKey, TValue> refreshValue)
        {
            Initialize(new ObservableDictionaryDecorator<TKey, TValue>(new DictionaryGetOrRefreshDecorator<TKey, TValue>(new ConcurrentDictionary<TKey, TValue>(), refreshValue)));
        }

        public void Dispose()
        {
            AssociatedSubscriptions.Dispose();
        }
    }
}