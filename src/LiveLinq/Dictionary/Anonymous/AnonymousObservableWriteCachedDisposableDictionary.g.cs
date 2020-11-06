using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Anonymous {
public class AnonymousObservableWriteCachedDisposableDictionary<TKey, TValue> : IObservableWriteCachedDisposableDictionary<TKey, TValue> {
private readonly Func<LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue>> _toLiveLinq;
public AnonymousObservableWriteCachedDisposableDictionary(Func<LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue>> toLiveLinq) {
_toLiveLinq = toLiveLinq;
}
public virtual LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> ToLiveLinq() {
return _toLiveLinq();
}
}
}

