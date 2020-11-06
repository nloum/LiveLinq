using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Decorators {
public class ObservableWriteCachedQueryableDictionaryDecoratorBase<TKey, TValue> : IObservableWriteCachedQueryableDictionary<TKey, TValue> {
private readonly IObservableWriteCachedQueryableDictionary<TKey, TValue> _decoratedObject;
public ObservableWriteCachedQueryableDictionaryDecoratorBase(IObservableWriteCachedQueryableDictionary<TKey, TValue> decoratedObject) {
_decoratedObject = decoratedObject;
}
LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> IObservableReadOnlyDictionary<TKey, TValue>.ToLiveLinq() {
return _decoratedObject.ToLiveLinq();
}
}
}

