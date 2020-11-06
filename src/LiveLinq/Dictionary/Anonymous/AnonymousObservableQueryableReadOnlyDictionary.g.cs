using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Anonymous {
public class AnonymousObservableQueryableReadOnlyDictionary<TKey, TValue> : IObservableQueryableReadOnlyDictionary<TKey, TValue> {
private readonly Func<LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue>> _toLiveLinq;
public AnonymousObservableQueryableReadOnlyDictionary(Func<LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue>> toLiveLinq) {
_toLiveLinq = toLiveLinq;
}
public virtual LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> ToLiveLinq() {
return _toLiveLinq();
}
}
}

