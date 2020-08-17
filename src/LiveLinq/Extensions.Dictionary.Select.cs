using System;
using System.Reactive.Linq;
using ComposableCollections.Dictionary;
using LiveLinq.Dictionary;
using MoreCollections;


namespace LiveLinq
{
    public static partial class Extensions
    {
        public static IDictionaryChanges<TKeyResult, TValueResult> Select<TKeySource, TValueSource, TKeyResult, TValueResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TValueSource, TKeyResult> keySelector,
            Func<TKeySource, TValueSource, TValueResult> valueSelector)
        {
            return source.SelectMany((key, value) => Observable.Return(Utility.DictionaryAdd(new KeyValue<TKeyResult, TValueResult>(keySelector(key, value), valueSelector(key, value)))).ToLiveLinq());
        }

        public static IDictionaryChanges<TKeyResult, TValueResult> Select<TKeySource, TValueSource, TKeyResult, TValueResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TKeyResult> keySelector,
            Func<TKeySource, TValueSource, TValueResult> valueSelector)
        {
            return source.SelectMany((key, value) => Observable.Return(Utility.DictionaryAdd(new KeyValue<TKeyResult, TValueResult>(keySelector(key), valueSelector(key, value)))).ToLiveLinq());
        }
        
        public static IDictionaryChanges<TKeyResult, TValueResult> Select<TKeySource, TValueSource, TKeyResult, TValueResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TValueSource, TKeyResult> keySelector,
            Func<TValueSource, TValueResult> valueSelector)
        {
            return source.SelectMany((key, value) => Observable.Return(Utility.DictionaryAdd(new KeyValue<TKeyResult, TValueResult>(keySelector(key, value), valueSelector(value)))).ToLiveLinq());
        }
        public static IDictionaryChanges<TKeyResult, TValueResult> Select<TKeySource, TValueSource, TKeyResult, TValueResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TKeyResult> keySelector,
            Func<TValueSource, TValueResult> valueSelector)
        {
            return source.SelectMany((key, value) => Observable.Return(Utility.DictionaryAdd(new KeyValue<TKeyResult, TValueResult>(keySelector(key), valueSelector(value)))).ToLiveLinq());
        }

        public static IDictionaryChanges<TKeyResult, TValueResult> Select<TKeySource, TValueSource, TKeyResult, TValueResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TValueSource, IKeyValue<TKeyResult, TValueResult>> selector)
        {
            return source.SelectMany((key, value) => Observable.Return(Utility.DictionaryAdd(selector(key, value))).ToLiveLinq());
        }

        public static IDictionaryChanges<TKeyResult, TValueResult> Select<TKeySource, TValueSource, TKeyResult, TValueResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TValueSource, IObservable<IKeyValue<TKeyResult, TValueResult>>> selector)
        {
            return source.SelectMany((key, value) => selector(key, value).Select(newKeyValuePair => Utility.DictionaryAdd(newKeyValuePair)).ToLiveLinq());
        }

        public static IDictionaryChanges<TKeySource, TValueResult> SelectValue<TKeySource, TValueSource, TValueResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TValueSource, TValueResult> selector)
        {
            return source.SelectMany((key, value) => Observable.Return(Utility.DictionaryAdd(new KeyValue<TKeySource, TValueResult>(key, selector(key, value)))).ToLiveLinq());
        }

        public static IDictionaryChanges<TKeySource, TValueResult> SelectValue<TKeySource, TValueSource, TValueResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TValueSource, TValueResult> selector)
        {
            return source.SelectMany((key, value) => Observable.Return(Utility.DictionaryAdd(new KeyValue<TKeySource, TValueResult>(key, selector(value)))).ToLiveLinq());
        }

        public static IDictionaryChanges<TKeySource, TValueResult> SelectValue<TKeySource, TValueSource, TValueResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TValueSource, IObservable<TValueResult>> selector)
        {
            return source.SelectMany((key, value) => selector(key, value).Select(newValue => Utility.DictionaryAdd(new KeyValue<TKeySource, TValueResult>(key, newValue))).ToLiveLinq());
        }

        public static IDictionaryChanges<TKeySource, TValueResult> SelectValue<TKeySource, TValueSource, TValueResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TValueSource, IObservable<TValueResult>> selector)
        {
            return source.SelectMany((key, value) => selector(value).Select(newValue => Utility.DictionaryAdd(new KeyValue<TKeySource, TValueResult>(key, newValue))).ToLiveLinq());
        }

        public static IDictionaryChanges<TKeyResult, TValueSource> SelectKey<TKeySource, TValueSource, TKeyResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TValueSource, TKeyResult> selector)
        {
            return source.SelectMany((key, value) => Observable.Return(Utility.DictionaryAdd(new KeyValue<TKeyResult, TValueSource>(selector(key, value), value))).ToLiveLinq());
        }

        public static IDictionaryChanges<TKeyResult, TValueSource> SelectKey<TKeySource, TValueSource, TKeyResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TKeyResult> selector)
        {
            return source.SelectMany((key, value) => Observable.Return(Utility.DictionaryAdd(new KeyValue<TKeyResult, TValueSource>(selector(key), value))).ToLiveLinq());
        }
        
        public static IDictionaryChanges<TKeyResult, TValueSource> SelectKey<TKeySource, TValueSource, TKeyResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, TValueSource, IObservable<TKeyResult>> selector)
        {
            return source.SelectMany((key, value) => selector(key, value).Select(newKey => Utility.DictionaryAdd(new KeyValue<TKeyResult, TValueSource>(newKey, value))).ToLiveLinq());
        }

        public static IDictionaryChanges<TKeyResult, TValueSource> SelectKey<TKeySource, TValueSource, TKeyResult>(
            this IDictionaryChanges<TKeySource, TValueSource> source,
            Func<TKeySource, IObservable<TKeyResult>> selector)
        {
            return source.SelectMany((key, value) => selector(key).Select(newKey => Utility.DictionaryAdd(new KeyValue<TKeyResult, TValueSource>(newKey, value))).ToLiveLinq());
        }
    }
}
