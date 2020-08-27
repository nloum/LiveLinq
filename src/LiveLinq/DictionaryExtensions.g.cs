using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Adapters;
using LiveLinq.Dictionary.Interfaces;
using System.Linq;
using System.Linq.Expressions;
using ComposableCollections.Common;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.Adapters;
using ComposableCollections.Dictionary.Decorators;
using ComposableCollections.Dictionary.Interfaces;
using ComposableCollections.Dictionary.Sources;
using ComposableCollections.Dictionary.Transactional;
using ComposableCollections.Dictionary.WithBuiltInKey;
using ComposableCollections.Dictionary.WithBuiltInKey.Interfaces;
using UtilityDisposables;

namespace LiveLinq
{
    public static partial class DictionaryExtensions
    {
        #region WithLiveLinq

        public static IObservableCachedDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this ICachedDictionary<TKey, TValue> source)
        {
            var decorated = new ObservableDictionaryDecorator<TKey, TValue>(source);
            return new ObservableCachedDictionaryAdapter<TKey, TValue>(decorated, source.AsBypassCache,
                source.AsNeverFlush, source.FlushCache, source.GetWrites, Disposable.Empty, decorated.ToLiveLinq);
        }

        public static IObservableCachedDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this ICachedDisposableDictionary<TKey, TValue> source)
        {
            var decorated = new ObservableDictionaryDecorator<TKey, TValue>(source);
            return new ObservableCachedDictionaryAdapter<TKey, TValue>(decorated, source.AsBypassCache,
                source.AsNeverFlush, source.FlushCache, source.GetWrites, source, decorated.ToLiveLinq);
        }

        public static IObservableCachedQueryableDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this ICachedDisposableQueryableDictionary<TKey, TValue> source)
        {
            var decorated = new ObservableDictionaryDecorator<TKey, TValue>(source);
            return new ObservableCachedQueryableDictionaryAdapter<TKey, TValue>(decorated, source.AsBypassCache,
                source.AsNeverFlush, source.FlushCache, source.GetWrites, source, source.Values, decorated.ToLiveLinq);
        }

        public static IObservableCachedQueryableDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this ICachedQueryableDictionary<TKey, TValue> source)
        {
            var decorated = new ObservableDictionaryDecorator<TKey, TValue>(source);
            return new ObservableCachedQueryableDictionaryAdapter<TKey, TValue>(decorated, source.AsBypassCache,
                source.AsNeverFlush, source.FlushCache, source.GetWrites, Disposable.Empty, source.Values,
                decorated.ToLiveLinq);
        }

        public static IObservableDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this IComposableDictionary<TKey, TValue> source)
        {
            var decorated = new ObservableDictionaryDecorator<TKey, TValue>(source);
            return new ObservableDictionaryAdapter<TKey, TValue>(decorated, Disposable.Empty, decorated.ToLiveLinq);
        }

        public static IObservableDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this IDisposableDictionary<TKey, TValue> source)
        {
            var decorated = new ObservableDictionaryDecorator<TKey, TValue>(source);
            return new ObservableDictionaryAdapter<TKey, TValue>(decorated, source, decorated.ToLiveLinq);
        }

        public static IObservableQueryableDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this IDisposableQueryableDictionary<TKey, TValue> source)
        {
            var decorated = new ObservableDictionaryDecorator<TKey, TValue>(source);
            return new ObservableQueryableDictionaryAdapter<TKey, TValue>(decorated, source, source.Values,
                decorated.ToLiveLinq);
        }

        public static IObservableQueryableDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this IQueryableDictionary<TKey, TValue> source)
        {
            var decorated = new ObservableDictionaryDecorator<TKey, TValue>(source);
            return new ObservableQueryableDictionaryAdapter<TKey, TValue>(decorated, Disposable.Empty, source.Values,
                decorated.ToLiveLinq);
        }

        #endregion

        #region WithLiveLinq with separate observable and observer for sharing changes across dictionaries

        public static IObservableCachedDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this ICachedDictionary<TKey, TValue> source, IObservable<IDictionaryChangeStrict<TKey, TValue>> observable,
            Action<IDictionaryChangeStrict<TKey, TValue>> onChange)
        {
            var decorated = new ObservableDictionaryDecorator<TKey, TValue>(source, observable, onChange);
            return new ObservableCachedDictionaryAdapter<TKey, TValue>(decorated, source.AsBypassCache,
                source.AsNeverFlush, source.FlushCache, source.GetWrites, Disposable.Empty, decorated.ToLiveLinq);
        }

        public static IObservableCachedDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this ICachedDisposableDictionary<TKey, TValue> source,
            IObservable<IDictionaryChangeStrict<TKey, TValue>> observable,
            Action<IDictionaryChangeStrict<TKey, TValue>> onChange)
        {
            var decorated = new ObservableDictionaryDecorator<TKey, TValue>(source, observable, onChange);
            return new ObservableCachedDictionaryAdapter<TKey, TValue>(decorated, source.AsBypassCache,
                source.AsNeverFlush, source.FlushCache, source.GetWrites, source, decorated.ToLiveLinq);
        }

        public static IObservableCachedQueryableDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this ICachedDisposableQueryableDictionary<TKey, TValue> source,
            IObservable<IDictionaryChangeStrict<TKey, TValue>> observable,
            Action<IDictionaryChangeStrict<TKey, TValue>> onChange)
        {
            var decorated = new ObservableDictionaryDecorator<TKey, TValue>(source, observable, onChange);
            return new ObservableCachedQueryableDictionaryAdapter<TKey, TValue>(decorated, source.AsBypassCache,
                source.AsNeverFlush, source.FlushCache, source.GetWrites, source, source.Values, decorated.ToLiveLinq);
        }

        public static IObservableCachedQueryableDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this ICachedQueryableDictionary<TKey, TValue> source,
            IObservable<IDictionaryChangeStrict<TKey, TValue>> observable,
            Action<IDictionaryChangeStrict<TKey, TValue>> onChange)
        {
            var decorated = new ObservableDictionaryDecorator<TKey, TValue>(source, observable, onChange);
            return new ObservableCachedQueryableDictionaryAdapter<TKey, TValue>(decorated, source.AsBypassCache,
                source.AsNeverFlush, source.FlushCache, source.GetWrites, Disposable.Empty, source.Values,
                decorated.ToLiveLinq);
        }

        public static IObservableDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this IComposableDictionary<TKey, TValue> source,
            IObservable<IDictionaryChangeStrict<TKey, TValue>> observable,
            Action<IDictionaryChangeStrict<TKey, TValue>> onChange)
        {
            var decorated = new ObservableDictionaryDecorator<TKey, TValue>(source, observable, onChange);
            return new ObservableDictionaryAdapter<TKey, TValue>(decorated, Disposable.Empty, decorated.ToLiveLinq);
        }

        public static IObservableDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this IDisposableDictionary<TKey, TValue> source,
            IObservable<IDictionaryChangeStrict<TKey, TValue>> observable,
            Action<IDictionaryChangeStrict<TKey, TValue>> onChange)
        {
            var decorated = new ObservableDictionaryDecorator<TKey, TValue>(source, observable, onChange);
            return new ObservableDictionaryAdapter<TKey, TValue>(decorated, source, decorated.ToLiveLinq);
        }

        public static IObservableQueryableDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this IDisposableQueryableDictionary<TKey, TValue> source,
            IObservable<IDictionaryChangeStrict<TKey, TValue>> observable,
            Action<IDictionaryChangeStrict<TKey, TValue>> onChange)
        {
            var decorated = new ObservableDictionaryDecorator<TKey, TValue>(source, observable, onChange);
            return new ObservableQueryableDictionaryAdapter<TKey, TValue>(decorated, source, source.Values,
                decorated.ToLiveLinq);
        }

        public static IObservableQueryableDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this IQueryableDictionary<TKey, TValue> source,
            IObservable<IDictionaryChangeStrict<TKey, TValue>> observable,
            Action<IDictionaryChangeStrict<TKey, TValue>> onChange)
        {
            var decorated = new ObservableDictionaryDecorator<TKey, TValue>(source, observable, onChange);
            return new ObservableQueryableDictionaryAdapter<TKey, TValue>(decorated, Disposable.Empty, source.Values,
                decorated.ToLiveLinq);
        }

        #endregion

        #region WithLiveLinq for read-only observables

        public static IObservableReadOnlyDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this IComposableReadOnlyDictionary<TKey, TValue> source,
            IObservable<IDictionaryChangeStrict<TKey, TValue>> changes)
        {
            return new ObservableReadOnlyDictionaryAdapter<TKey, TValue>(source, Disposable.Empty,
                () => changes.ToLiveLinq());
        }

        public static IObservableQueryableReadOnlyDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this IDisposableQueryableReadOnlyDictionary<TKey, TValue> source,
            IObservable<IDictionaryChangeStrict<TKey, TValue>> changes)
        {
            return new ObservableQueryableReadOnlyDictionaryAdapter<TKey, TValue>(source, source, source.Values,
                () => changes.ToLiveLinq());
        }

        public static IObservableReadOnlyDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this IDisposableReadOnlyDictionary<TKey, TValue> source,
            IObservable<IDictionaryChangeStrict<TKey, TValue>> changes)
        {
            return new ObservableReadOnlyDictionaryAdapter<TKey, TValue>(source, source, () => changes.ToLiveLinq());
        }

        public static IObservableQueryableReadOnlyDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this IQueryableReadOnlyDictionary<TKey, TValue> source,
            IObservable<IDictionaryChangeStrict<TKey, TValue>> changes)
        {
            return new ObservableQueryableReadOnlyDictionaryAdapter<TKey, TValue>(source, Disposable.Empty,
                source.Values, () => changes.ToLiveLinq());
        }

        #endregion

        #region WithLiveLinq - transactional

        public static
            ITransactionalCollection<IObservableReadOnlyDictionary<TKey, TValue>,
                IObservableCachedDictionary<TKey, TValue>> WithLiveLinq<TKey, TValue>(
                this ITransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue>,
                    ICachedDisposableDictionary<TKey, TValue>> source,
                IObservable<IDictionaryChangeStrict<TKey, TValue>> observable,
                Action<IDictionaryChangeStrict<TKey, TValue>> onChange)
        {
            return new AnonymousTransactionalCollection<IObservableReadOnlyDictionary<TKey, TValue>,
                IObservableCachedDictionary<TKey, TValue>>(
                () => source.BeginRead().WithLiveLinq(observable),
                () => source.BeginWrite().WithLiveLinq(observable, onChange));
        }

        public static
            ITransactionalCollection<IObservableQueryableReadOnlyDictionary<TKey, TValue>,
                IObservableCachedQueryableDictionary<TKey, TValue>> WithLiveLinq<TKey, TValue>(
                this ITransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey, TValue>,
                    ICachedDisposableQueryableDictionary<TKey, TValue>> source,
                IObservable<IDictionaryChangeStrict<TKey, TValue>> observable,
                Action<IDictionaryChangeStrict<TKey, TValue>> onChange)
        {
            return new AnonymousTransactionalCollection<IObservableQueryableReadOnlyDictionary<TKey, TValue>,
                IObservableCachedQueryableDictionary<TKey, TValue>>(
                () => source.BeginRead().WithLiveLinq(observable),
                () => source.BeginWrite().WithLiveLinq(observable, onChange));
        }

        public static
            ITransactionalCollection<IObservableReadOnlyDictionary<TKey, TValue>, IObservableDictionary<TKey, TValue>>
            WithLiveLinq<TKey, TValue>(
                this ITransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue>,
                    IDisposableDictionary<TKey, TValue>> source,
                IObservable<IDictionaryChangeStrict<TKey, TValue>> observable,
                Action<IDictionaryChangeStrict<TKey, TValue>> onChange)
        {
            return new AnonymousTransactionalCollection<IObservableReadOnlyDictionary<TKey, TValue>,
                IObservableDictionary<TKey, TValue>>(
                () => source.BeginRead().WithLiveLinq(observable),
                () => source.BeginWrite().WithLiveLinq(observable, onChange));
        }

        public static
            ITransactionalCollection<IObservableQueryableReadOnlyDictionary<TKey, TValue>,
                IObservableQueryableDictionary<TKey, TValue>> WithLiveLinq<TKey, TValue>(
                this ITransactionalCollection<IDisposableQueryableReadOnlyDictionary<TKey, TValue>,
                    IDisposableQueryableDictionary<TKey, TValue>> source,
                IObservable<IDictionaryChangeStrict<TKey, TValue>> observable,
                Action<IDictionaryChangeStrict<TKey, TValue>> onChange)
        {
            return new AnonymousTransactionalCollection<IObservableQueryableReadOnlyDictionary<TKey, TValue>,
                IObservableQueryableDictionary<TKey, TValue>>(
                () => source.BeginRead().WithLiveLinq(observable),
                () => source.BeginWrite().WithLiveLinq(observable, onChange));
        }

        #endregion
    }
}