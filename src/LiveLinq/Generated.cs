using System;
using System.Reactive.Subjects;
using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Adapters;
using LiveLinq.Dictionary.Interfaces;

namespace LiveLinq {
public static partial class Extensions {

public static IObservableDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(this IComposableDictionary<TKey, TValue> source) {
return new ObservableDictionaryAdapter<TKey, TValue>(source);
}


public static IObservableDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(this IComposableDictionary<TKey, TValue> source, Subject<IDictionaryChangeStrict<TKey, TValue>> subject) {
return new ObservableDictionaryAdapter<TKey, TValue>(source, subject);
}


public static IObservableReadOnlyDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(this IComposableReadOnlyDictionary<TKey, TValue> source, IObservable<IDictionaryChangeStrict<TKey, TValue>> observable, Action<IDictionaryChangeStrict<TKey, TValue>> onChange) {
return new ObservableDictionaryAdapter<TKey, TValue>(source, observable, onChange);
}


public static IObservableReadOnlyDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(this IComposableDictionary<TKey, TValue> source, IObservable<IDictionaryChangeStrict<TKey, TValue>> observable) {
return new ObservableReadOnlyDictionaryAdapter<TKey, TValue>(source, observable);
}

}
}
