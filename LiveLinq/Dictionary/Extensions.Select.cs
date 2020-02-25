using System;
using System.Reactive.Linq;

using MoreCollections;
using static MoreCollections.Utility;

namespace LiveLinq.Dictionary
{
    public static partial class Extensions
    {
        public static IDictionaryChanges<TKeyResult, TValueResult> Select<TKeySource, TValueSource, TKeyResult, TValueResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TValueSource, TKeyResult> keySelector,
            Func<TKeySource, TValueSource, TValueResult> valueSelector)
        {
            return source.SelectMany((key, value) => Observable.Return(Utility.DictionaryAdd(KeyValuePair(keySelector(key, value), valueSelector(key, value)))).ToLiveLinq());
        }

        public static IDictionaryChanges<TKeyResult, TValueResult> Select<TKeySource, TValueSource, TKeyResult, TValueResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TKeyResult> keySelector,
            Func<TKeySource, TValueSource, TValueResult> valueSelector)
        {
            return source.SelectMany((key, value) => Observable.Return(Utility.DictionaryAdd(KeyValuePair(keySelector(key), valueSelector(key, value)))).ToLiveLinq());
        }
        
        public static IDictionaryChanges<TKeyResult, TValueResult> Select<TKeySource, TValueSource, TKeyResult, TValueResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TValueSource, TKeyResult> keySelector,
            Func<TValueSource, TValueResult> valueSelector)
        {
            return source.SelectMany((key, value) => Observable.Return(Utility.DictionaryAdd(KeyValuePair(keySelector(key, value), valueSelector(value)))).ToLiveLinq());
        }
        public static IDictionaryChanges<TKeyResult, TValueResult> Select<TKeySource, TValueSource, TKeyResult, TValueResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TKeyResult> keySelector,
            Func<TValueSource, TValueResult> valueSelector)
        {
            return source.SelectMany((key, value) => Observable.Return(Utility.DictionaryAdd(KeyValuePair(keySelector(key), valueSelector(value)))).ToLiveLinq());
        }

        public static IDictionaryChanges<TKeyResult, TValueResult> Select<TKeySource, TValueSource, TKeyResult, TValueResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TValueSource, IKeyValuePair<TKeyResult, TValueResult>> selector)
        {
            return source.SelectMany((key, value) => Observable.Return(Utility.DictionaryAdd(selector(key, value))).ToLiveLinq());
        }

        public static IDictionaryChanges<TKeyResult, TValueResult> Select<TKeySource, TValueSource, TKeyResult, TValueResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TValueSource, IObservable<IKeyValuePair<TKeyResult, TValueResult>>> selector)
        {
            return source.SelectMany((key, value) => selector(key, value).Select(newKeyValuePair => Utility.DictionaryAdd(newKeyValuePair)).ToLiveLinq());
        }

        public static IDictionaryChanges<TKeySource, TValueResult> SelectValue<TKeySource, TValueSource, TValueResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TValueSource, TValueResult> selector)
        {
            return source.SelectMany((key, value) => Observable.Return(Utility.DictionaryAdd(KeyValuePair(key, selector(key, value)))).ToLiveLinq());
        }

        public static IDictionaryChanges<TKeySource, TValueResult> SelectValue<TKeySource, TValueSource, TValueResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TValueSource, TValueResult> selector)
        {
            return source.SelectMany((key, value) => Observable.Return(Utility.DictionaryAdd(KeyValuePair(key, selector(value)))).ToLiveLinq());
        }

        public static IDictionaryChanges<TKeySource, TValueResult> SelectValue<TKeySource, TValueSource, TValueResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TValueSource, IObservable<TValueResult>> selector)
        {
            return source.SelectMany((key, value) => selector(key, value).Select(newValue => Utility.DictionaryAdd(KeyValuePair(key, newValue))).ToLiveLinq());
        }

        public static IDictionaryChanges<TKeySource, TValueResult> SelectValue<TKeySource, TValueSource, TValueResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TValueSource, IObservable<TValueResult>> selector)
        {
            return source.SelectMany((key, value) => selector(value).Select(newValue => Utility.DictionaryAdd(KeyValuePair(key, newValue))).ToLiveLinq());
        }

        public static IDictionaryChanges<TKeyResult, TValueSource> SelectKey<TKeySource, TValueSource, TKeyResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TValueSource, TKeyResult> selector)
        {
            return source.SelectMany((key, value) => Observable.Return(Utility.DictionaryAdd(KeyValuePair(selector(key, value), value))).ToLiveLinq());
        }

        public static IDictionaryChanges<TKeyResult, TValueSource> SelectKey<TKeySource, TValueSource, TKeyResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TKeyResult> selector)
        {
            return source.SelectMany((key, value) => Observable.Return(Utility.DictionaryAdd(KeyValuePair(selector(key), value))).ToLiveLinq());
        }
        
        public static IDictionaryChanges<TKeyResult, TValueSource> SelectKey<TKeySource, TValueSource, TKeyResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TValueSource, IObservable<TKeyResult>> selector)
        {
            return source.SelectMany((key, value) => selector(key, value).Select(newKey => Utility.DictionaryAdd(KeyValuePair(newKey, value))).ToLiveLinq());
        }

        public static IDictionaryChanges<TKeyResult, TValueSource> SelectKey<TKeySource, TValueSource, TKeyResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, IObservable<TKeyResult>> selector)
        {
            return source.SelectMany((key, value) => selector(key).Select(newKey => Utility.DictionaryAdd(KeyValuePair(newKey, value))).ToLiveLinq());
        }
    }
}
