using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using ComposableCollections;
using ComposableCollections.Dictionary;
using SimpleMonads;
using LiveLinq.Ordered;

using MoreCollections;
using LiveLinq.Dictionary;
using LiveLinq.Core;
using LiveLinq.List;
using LiveLinq.Set;

namespace LiveLinq
{
    public static partial class Extensions
    {
        /// <summary>
        /// Returns a facade on top of the collection that was passed in. This facade intercepts all
        /// calls that change the collection and fires out LiveLinq change events.
        /// </summary>
        public static IObservableDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this IComposableDictionary<TKey, TValue> source)
        {
            return new ObservableDictionaryDecorator<TKey, TValue>(source);
        }
        
        /// <summary>
        /// Returns a facade on top of the collection that was passed in. This facade intercepts all
        /// calls that change the collection and fires out LiveLinq change events to the specified IObserver.
        /// </summary>
        public static IComposableDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this IComposableDictionary<TKey, TValue> source, IObserver<IDictionaryChangeStrict<TKey, TValue>> observer)
        {
            return new ObservableDictionaryDecoratorBase<TKey, TValue>(source, observer.OnNext);
        }

        /// <summary>
        /// Returns a facade on top of the collection that was passed in. This facade exposes the events fired on the
        /// IObserver as changes to this collection.
        /// </summary>
        public static IReadOnlyObservableDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this IComposableReadOnlyDictionary<TKey, TValue> source, IObservable<IDictionaryChangeStrict<TKey, TValue>> observer)
        {
            return new ReadOnlyObservableDictionaryDecorator<TKey, TValue>(source, observer);
        }

        /// <summary>
        /// Returns a facade on top of the collection that was passed in. This facade exposes the events fired on the
        /// subject as changes to this collection. In addition, changes to this dictionary will also fire events.
        /// This is an easy way to share change events across multiple dictionary instances of the same type.
        /// </summary>
        public static IObservableDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this IComposableDictionary<TKey, TValue> source, ISubject<IDictionaryChangeStrict<TKey, TValue>> subject)
        {
            return new ObservableDictionaryDecorator<TKey, TValue>(source, subject, subject.OnNext);
        }

        /// <summary>
        /// Returns a facade on top of the collection that was passed in. This facade exposes the events fired on the
        /// subject as changes to this collection. In addition, changes to this dictionary will also fire events.
        /// This is an easy way to share change events across multiple dictionary instances of the same type.
        /// </summary>
        public static IObservableDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this IComposableDictionary<TKey, TValue> source, IObservable<IDictionaryChangeStrict<TKey, TValue>> observable, Action<IDictionaryChangeStrict<TKey, TValue>> onChange)
        {
            return new ObservableDictionaryDecorator<TKey, TValue>(source, observable, onChange);
        }

        /// <summary>
        /// Returns a facade on top of the collection that was passed in. This facade intercepts all
        /// calls that change the collection and fires out LiveLinq change events.
        /// </summary>
        public static IObservableTransactionalDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(
            this ITransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue>, IDisposableDictionary<TKey, TValue>> source, bool fireInitialState = true)
        {
            return new ObservableTransactionalDictionaryDecorator<TKey, TValue>(source, fireInitialState);
        }

        /// <summary>
        /// Creates a facade on top of the specified IObservableDictionary that has a built-in key, which means you're telling
        /// the object how to get the key from a value. That means any API where you pass in a TValue, you
        /// won't have to tell the API what the key is.
        /// </summary>
        public static IObservableDictionaryWithBuiltInKey<TKey, TValue> WithBuiltInKey<TKey, TValue>(this IObservableDictionary<TKey, TValue> source, Func<TValue, TKey> key)
        {
            return new AnonymousObservableDictionaryWithBuiltInKeyAdapter<TKey, TValue>(source, key);
        }
        
        /// <summary>
        /// Creates a facade on top of the specified IObservableDictionary that has a built-in key, which means you're telling
        /// the object how to get the key from a value. That means any API where you pass in a TValue, you
        /// won't have to tell the API what the key is.
        /// </summary>
        public static IReadOnlyObservableDictionaryWithBuiltInKey<TKey, TValue> WithBuiltInKey<TKey, TValue>(this IReadOnlyObservableDictionary<TKey, TValue> source, Func<TValue, TKey> key)
        {
            return new AnonymousReadOnlyObservableDictionaryWithBuiltInKeyAdapter<TKey, TValue>(source, key);
        }

        /// <summary>
        /// Creates a facade on top of the specified IObservableDictionary that has a built-in key, which means you're telling
        /// the object how to get the key from a value. That means any API where you pass in a TValue, you
        /// won't have to tell the API what the key is.
        /// </summary>
        public static IReadOnlyObservableTransactionalDictionaryWithBuiltInKey<TKey, TValue> WithBuiltInKey<TKey, TValue>(this IReadOnlyObservableTransactionalDictionary<TKey, TValue> source, Func<TValue, TKey> key)
        {
            var withBuiltInKey = source.WithBuiltInKey(key);
            return new AnonymousReadOnlyObservableTransactionalDictionaryWithBuiltInKey<TKey, TValue>(withBuiltInKey.BeginRead, source.ToLiveLinq);
        }

        /// <summary>
        /// Creates a facade on top of the specified IObservableDictionary that lets you optionally create values when
        /// they're accessed, on demand.
        /// </summary>
        public static IObservableDictionary<TKey, TValue> WithDefaultValue<TKey, TValue>(this IObservableDictionary<TKey, TValue> source, GetDefaultValue<TKey, TValue> getDefaultValue)
        {
            return new ObservableDictionaryGetOrDefaultDecorator<TKey, TValue>(source, getDefaultValue);
        }

        /// <summary>
        /// Creates a facade on top of the specified IObservableDictionary that lets you optionally create values when
        /// they're accessed, on demand.
        /// </summary>
        public static IObservableTransactionalDictionary<TKey, TValue> WithDefaultValue<TKey, TValue>(this IObservableTransactionalDictionary<TKey, TValue> source, GetDefaultValue<TKey, TValue> getDefaultValue)
        {
            return new AnonymousObservableTransactionalDictionary<TKey, TValue>(() =>
            {
                var result = source.BeginWrite();
                return new DisposableDictionaryDecorator<TKey, TValue>(result.WithDefaultValue(getDefaultValue),
                    result);
            }, () =>
            {
                var result = source.BeginWrite();
                return new DisposableDictionaryDecorator<TKey, TValue>(result.WithDefaultValue(getDefaultValue),
                    result);
            }, source.ToLiveLinq);
        }

        /// <summary>
        /// Creates a facade on top of the specified IObservableDictionary that lets you optionally create values when
        /// they're accessed, on demand.
        /// </summary>
        public static IObservableDictionary<TKey, TValue> WithDefaultValue<TKey, TValue>(this IObservableDictionary<TKey, TValue> source, Func<TKey, TValue> getDefaultValue, bool persist = true)
        {
            return new ObservableDictionaryGetOrDefaultDecorator<TKey, TValue>(source,
                (TKey key, out IMaybe<TValue> value, out bool b) =>
                {
                    value = getDefaultValue(key).ToMaybe();
                    b = persist;
                });
        }

        /// <summary>
        /// Creates a facade on top of the specified IObservableDictionary that lets you optionally create values when
        /// they're accessed, on demand.
        /// </summary>
        public static IObservableTransactionalDictionary<TKey, TValue> WithDefaultValue<TKey, TValue>(this IObservableTransactionalDictionary<TKey, TValue> source, Func<TKey, TValue> getDefaultValue)
        {
            return new AnonymousObservableTransactionalDictionary<TKey, TValue>(() =>
            {
                var result = source.BeginWrite();
                return new DisposableDictionaryDecorator<TKey, TValue>(result.WithDefaultValue(getDefaultValue),
                    result);
            }, () =>
            {
                var result = source.BeginWrite();
                return new DisposableDictionaryDecorator<TKey, TValue>(result.WithDefaultValue(getDefaultValue),
                    result);
            }, source.ToLiveLinq);
        }

        /// <summary>
        /// Creates a facade on top of the specified IObservableDictionary that lets you optionally update values when
        /// they're accessed, on demand.
        /// </summary>
        public static IObservableDictionary<TKey, TValue> WithRefreshing<TKey, TValue>(this IObservableDictionary<TKey, TValue> source, RefreshValue<TKey, TValue> refreshValue)
        {
            return new ObservableDictionaryGetOrRefreshDecorator<TKey, TValue>(source, refreshValue);
        }
        
        /// <summary>
        /// Creates a facade on top of the specified IObservableDictionary that lets you optionally create values when
        /// they're accessed, on demand.
        /// </summary>
        public static IObservableTransactionalDictionary<TKey, TValue> WithRefreshing<TKey, TValue>(this IObservableTransactionalDictionary<TKey, TValue> source, RefreshValue<TKey, TValue> refreshValue)
        {
            return new AnonymousObservableTransactionalDictionary<TKey, TValue>(() =>
            {
                var result = source.BeginWrite();
                return new DisposableDictionaryDecorator<TKey, TValue>(result.WithRefreshing(refreshValue),
                    result);
            }, () =>
            {
                var result = source.BeginWrite();
                return new DisposableDictionaryDecorator<TKey, TValue>(result.WithRefreshing(refreshValue),
                    result);
            }, source.ToLiveLinq);
        }

        /// <summary>
        /// A special ToReadOnlyObservableDictionary that works well for IDictionaryChanges{TKey, ISetChanges{TValue}}
        /// so that you can read the set results in each group easily. This works well for results from the .GroupBy
        /// LiveLinq method.
        /// </summary>
        public static IReadOnlyObservableDictionary<TKey, IReadOnlyObservableSet<TValue>> ToReadOnlyObservableDictionary<TKey, TValue>(
            this IDictionaryChanges<TKey, ISetChanges<TValue>> changes)
        {
            var result = new ObservableDictionaryGetOrDefault<TKey, ObservableSet<TValue>>(
                (TKey key, out IMaybe<ObservableSet<TValue>> maybeValue, out bool persist) =>
                {
                    persist = true;
                    maybeValue = new ObservableSet<TValue>().ToMaybe();
                });

            var disposable = changes.Subscribe((key, setChanges) =>
            {
                return setChanges.Subscribe(value => result[key].Add(value), (value, _) => result[key].Remove(value));
            }, (key, value, setChangesSubscription) =>
            {
                // TODO - remove the key/value pair from result if there are no subscriptions,
                // so we don't have a memory leak as new groups get added.
                setChangesSubscription.Dispose();
            });

            result.AssociatedSubscriptions.Disposes(disposable);

            return result;
        }
        
        public static IDictionaryChangesStrict<TKey, TValue> OtherwiseEmpty<TKey, TValue>(
            this IMaybe<IDictionaryChangesStrict<TKey, TValue>> maybe)
        {
            return maybe.Otherwise(() => Utility.EmptyDictionaryChangesStrict<TKey, TValue>());
        }

        public static IDictionaryChanges<TKey, TValue> OtherwiseEmpty<TKey, TValue>(
            this IMaybe<IDictionaryChanges<TKey, TValue>> maybe)
        {
            return maybe.Otherwise(() => Utility.EmptyDictionaryChanges<TKey, TValue>());
        }
        
        public static IReadOnlyObservableDictionary<TKey, TValue> ToReadOnlyObservableDictionary<TKey, TValue>(
            this IDictionaryChanges<TKey, TValue> changes)
        {
            var result = new ObservableDictionary<TKey, TValue>();
            result.AssociatedSubscriptions.Disposes(changes.AsObservable().Subscribe(change =>
            {
                if (change.Type == CollectionChangeType.Add)
                {
                    result.AddRange(change.KeyValuePairs);
                }
                else if (change.Type == CollectionChangeType.Remove)
                {
                    result.RemoveRange(change.KeyValuePairs.Select(x => x.Key));
                }
            }));
            return result;
        }

        #region ContainsKey

        #region Non-strict
        
        /// <summary>
        /// Similar to the Dictionary.ContainsKey extension method, except this returns an event stream
        /// that fires a new event every time the key is removed or added again.
        /// </summary>
        /// <param name="key">The key you are interested in</param>
        public static IObservable<bool> ContainsKey<TKey, TValue>(
            this IDictionaryChanges<TKey, TValue> source,
            TKey key)
        {
            return source[key].Select(m => m.HasValue);
        }
        
        /// <summary>
        /// Similar to the Dictionary.ContainsKey extension method, except this returns an event stream
        /// that fires a new event every time the key is removed or added again.
        /// </summary>
        /// <param name="key">The event stream of keys you are interested in. Every time a new key comes along
        /// in this event stream, this method checks to see if source contains that key, and fires a new
        /// event, even if the new event is the same as the previous event.</param>
        public static IObservable<bool> ContainsKey<TKey, TValue>(
            this IDictionaryChanges<TKey, TValue> source,
            IObservable<TKey> key)
        {
            return source[key].Select(m => m.HasValue);
        }
        
        /// <summary>
        /// Similar to the Dictionary.ContainsKey extension method, except this returns an event stream
        /// that fires a new event every time the key is removed or added again.
        /// </summary>
        /// <param name="key">The event stream of keys you are interested in. Every time a new key comes along
        /// in this event stream, this method checks to see if source contains that key, and fires a new
        /// event, even if the new event is the same as the previous event.</param>
        public static IObservable<bool> ContainsKey<TKey, TValue>(
            this IDictionaryChanges<TKey, TValue> source,
            IObservable<IMaybe<TKey>> key)
        {
            return source[key].Select(m => m.HasValue);
        }

        #endregion

        #region Strict

        /// <summary>
        /// Similar to the Dictionary.ContainsKey extension method, except this returns an event stream
        /// that fires a new event every time the key is removed or added again.
        /// </summary>
        /// <param name="key">The key you are interested in</param>
        public static IObservable<bool> ContainsKey<TKey, TValue>(
            this IDictionaryChangesStrict<TKey, TValue> source,
            TKey key)
        {
            return source[key].Select(m => m.HasValue);
        }
        
        /// <summary>
        /// Similar to the Dictionary.ContainsKey extension method, except this returns an event stream
        /// that fires a new event every time the key is removed or added again.
        /// </summary>
        /// <param name="key">The event stream of keys you are interested in. Every time a new key comes along
        /// in this event stream, this method checks to see if source contains that key, and fires a new
        /// event, even if the new event is the same as the previous event.</param>
        public static IObservable<bool> ContainsKey<TKey, TValue>(
            this IDictionaryChangesStrict<TKey, TValue> source,
            IObservable<TKey> key)
        {
            return source[key].Select(m => m.HasValue);
        }
        
        /// <summary>
        /// Similar to the Dictionary.ContainsKey extension method, except this returns an event stream
        /// that fires a new event every time the key is removed or added again.
        /// </summary>
        /// <param name="key">The event stream of keys you are interested in. Every time a new key comes along
        /// in this event stream, this method checks to see if source contains that key, and fires a new
        /// event, even if the new event is the same as the previous event.</param>
        public static IObservable<bool> ContainsKey<TKey, TValue>(
            this IDictionaryChangesStrict<TKey, TValue> source,
            IObservable<IMaybe<TKey>> key)
        {
            return source[key].Select(m => m.HasValue);
        }

        #endregion

        #endregion

        public static IDictionaryChangesStrict<TKey, TValue> MakeStrictExpensively<TKey, TValue>(
            this IDictionaryChanges<TKey, TValue> source)
        {
            return source.ToObservableStateAndChange()
                .Select(sac => sac.MostRecentChange)
                .ToLiveLinq();
        }

        /// <summary>
        /// A LiveLinq query of keys from the dictionary. Analogous to the Keys property of a Dictionary.
        /// </summary>
        public static IListChanges<TKey> KeysAsList<TKey, TValue>(
            this IDictionaryChangesStrict<TKey, TValue> source)
        {
            return source.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Key);
        }

        /// <summary>
        /// A LiveLinq query of values from the dictionary. Analogous to the Values property of a Dictionary.
        /// </summary>
        public static IListChanges<TValue> ValuesAsList<TKey, TValue>(
            this IDictionaryChangesStrict<TKey, TValue> source)
        {
            return source.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value);
        }

        public static ISetChanges<TKey> KeysAsSet<TKey, TValue>(this IDictionaryChanges<TKey, TValue> dictionaryChanges)
        {
            return dictionaryChanges.AsObservable().Select(dictionaryChange =>
            {
                return Utility.SetChange(dictionaryChange.Type, dictionaryChange.KeyValuePairs.Select(x => x.Key));
            }).ToLiveLinq();
        }

        public static ISetChanges<IKeyValue<TKey, TValue>> KeyValuePairsAsSet<TKey, TValue>(this IDictionaryChanges<TKey, TValue> dictionaryChanges)
        {
            return dictionaryChanges.AsObservable().Select(dictionaryChange =>
            {
                return Utility.SetChange(dictionaryChange.Type, dictionaryChange.KeyValuePairs.ToImmutableList());
            }).ToLiveLinq();
        }

        /// <summary>
        /// Selects the values of the dictionary. Warning: if there are duplicate values anywhere in the dictionary, they will
        /// be reduced to a single distinct value.
        /// </summary>
        public static ISetChanges<TValue> ValuesAsSet<TKey, TValue>(this IDictionaryChanges<TKey, TValue> dictionaryChanges)
        {
            return dictionaryChanges.AsObservable().Select(dictionaryChange =>
            {
                return Utility.SetChange(dictionaryChange.Type, dictionaryChange.KeyValuePairs.Select(x => x.Value));
            }).ToLiveLinq();
        }

        /// <summary>
        /// Converts a strict list change to a dictionary change. There isn't an overload of this function that converts
        /// a non-strict list change to a non-strict dictionary change, because we don't know the key values of the list removes
        /// when the range of items to be removed is specified, but not the actual values to be removed.
        /// </summary>
        public static IDictionaryChangeStrict<TKey, TValue> ToDictionaryChange<TKey, TValue>(
            this IListChangeStrict<IKeyValue<TKey, TValue>> listChange)
        {
            if (listChange.Type == CollectionChangeType.Add)
                return Utility.DictionaryAdd(listChange.Values);
            return Utility.DictionaryRemove(listChange.Values);
        }

        /// <summary>
        /// Applies the dictionary change to the specified <see cref="ImmutableDictionary{T}"/>.
        /// </summary>
        public static ImmutableDictionary<TKey, TValue> Mutate<TKey, TValue>(this ImmutableDictionary<TKey, TValue> subject, IDictionaryChange<TKey, TValue> change)
        {
            switch (change.Type)
            {
                case CollectionChangeType.Add:
                    {
                        var result = subject.AddRange(change.KeyValuePairs.Select(kvp => new KeyValuePair<TKey, TValue>(kvp.Key, kvp.Value)));
                        return result;
                    }
                case CollectionChangeType.Remove:
                    {
                        var result = subject.RemoveRange(change.Keys);
                        return result;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Applies the dictionary change to the specified <see cref="ImmutableDictionary{T}"/>.
        /// </summary>
        public static void Mutate<TKey, TValue>(this IDictionary<TKey, TValue> subject, IDictionaryChange<TKey, TValue> change)
        {
            switch (change.Type)
            {
                case CollectionChangeType.Add:
                    subject.AddRange(change.KeyValuePairs.Select(kvp => new KeyValuePair<TKey, TValue>(kvp.Key, kvp.Value)));
                    break;
                case CollectionChangeType.Remove:
                    subject.RemoveRange(change.Keys);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static IDictionaryChangesStrict<TKey, TValue> ToDictionaryLiveLinq<TItem, TKey, TValue>(
            this ISetChanges<TItem> setChanges, Func<TItem, TKey> keySelector, Func<TItem, TValue> valueSelector)
        {
            return setChanges.AsObservable().Select(setChange =>
            {
                switch (setChange.Type)
                {
                    case CollectionChangeType.Add:
                        return Utility.DictionaryAdd(setChange.Values.Select(item =>
                            new KeyValuePair<TKey, TValue>(keySelector(item), valueSelector(item))));
                    case CollectionChangeType.Remove:
                        return Utility.DictionaryRemove(setChange.Values.Select(item =>
                            new KeyValuePair<TKey, TValue>(keySelector(item), valueSelector(item))));
                    default:
                        throw new ArgumentException($"Unknown collection change type: {setChange.Type}");
                }
            }).ToLiveLinq();
        }
        
        public static IDictionaryChanges<TKey, TValue> ToDictionaryLiveLinq<TItem, TKey, TValue>(
            this ISetChanges<TItem> setChanges, Func<TItem, IObservable<TKey>> keySelector, Func<TItem, TValue> valueSelector)
        {
            return setChanges.AsObservable().Select(setChange =>
            {
                switch (setChange.Type)
                {
                    case CollectionChangeType.Add:
                        return Utility.DictionaryAdd(setChange.Values.Select(item =>
                            new KeyValuePair<IObservable<TKey>, TValue>(keySelector(item), valueSelector(item))));
                    case CollectionChangeType.Remove:
                        return Utility.DictionaryRemove(setChange.Values.Select(item =>
                            new KeyValuePair<IObservable<TKey>, TValue>(keySelector(item), valueSelector(item))));
                    default:
                        throw new ArgumentException($"Unknown collection change type: {setChange.Type}");
                }
            }).ToLiveLinq()
                .SelectKey(key => key.AsObservable());
        }
        
        public static IDictionaryChanges<TKey, TValue> ToDictionaryLiveLinq<TItem, TKey, TValue>(
            this ISetChanges<TItem> setChanges, Func<TItem, IObservable<TKey>> keySelector, Func<TItem, IObservable<TValue>> valueSelector)
        {
            return setChanges.AsObservable().Select(setChange =>
                {
                    switch (setChange.Type)
                    {
                        case CollectionChangeType.Add:
                            return Utility.DictionaryAdd(setChange.Values.Select(item =>
                                new KeyValuePair<IObservable<TKey>, IObservable<TValue>>(keySelector(item), valueSelector(item))));
                        case CollectionChangeType.Remove:
                            return Utility.DictionaryRemove(setChange.Values.Select(item =>
                                new KeyValuePair<IObservable<TKey>, IObservable<TValue>>(keySelector(item), valueSelector(item))));
                        default:
                            throw new ArgumentException($"Unknown collection change type: {setChange.Type}");
                    }
                }).ToLiveLinq()
                .SelectKey(key => key.AsObservable())
                .SelectValue(value => value.AsObservable());
        }
        
        public static IDictionaryChanges<TKey, TValue> ToDictionaryLiveLinq<TItem, TKey, TValue>(
            this ISetChanges<TItem> setChanges, Func<TItem, TKey> keySelector, Func<TItem, IObservable<TValue>> valueSelector)
        {
            return setChanges.AsObservable().Select(setChange =>
                {
                    switch (setChange.Type)
                    {
                        case CollectionChangeType.Add:
                            return Utility.DictionaryAdd(setChange.Values.Select(item =>
                                new KeyValuePair<TKey, IObservable<TValue>>(keySelector(item), valueSelector(item))));
                        case CollectionChangeType.Remove:
                            return Utility.DictionaryRemove(setChange.Values.Select(item =>
                                new KeyValuePair<TKey, IObservable<TValue>>(keySelector(item), valueSelector(item))));
                        default:
                            throw new ArgumentException($"Unknown collection change type: {setChange.Type}");
                    }
                }).ToLiveLinq()
                .SelectValue(value => value.AsObservable());
        }
    }
}
