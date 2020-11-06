using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Anonymous {
public class AnonymousObservableDisposableReadOnlyDictionary<TKey, TValue> : IObservableDisposableReadOnlyDictionary<TKey, TValue> {
private readonly Func<LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue>> _toLiveLinq;
public AnonymousObservableDisposableReadOnlyDictionary(Func<LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue>> toLiveLinq) {
_toLiveLinq = toLiveLinq;
}
public virtual LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> ToLiveLinq() {
return _toLiveLinq();
}
}
}

