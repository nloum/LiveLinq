using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Decorators {
public class ObservableReadWriteCachedQueryableDictionaryDecoratorBase<TKey, TValue> : IObservableReadWriteCachedQueryableDictionary<TKey, TValue> {
private readonly IObservableReadWriteCachedQueryableDictionary<TKey, TValue> _decoratedObject;
public ObservableReadWriteCachedQueryableDictionaryDecoratorBase(IObservableReadWriteCachedQueryableDictionary<TKey, TValue> decoratedObject) {
_decoratedObject = decoratedObject;
}
LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> IObservableReadOnlyDictionary<TKey, TValue>.ToLiveLinq() {
return _decoratedObject.ToLiveLinq();
}
}
}

