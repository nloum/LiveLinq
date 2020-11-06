using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Decorators {
public class ObservableReadCachedQueryableReadOnlyDictionaryDecoratorBase<TKey, TValue> : IObservableReadCachedQueryableReadOnlyDictionary<TKey, TValue> {
private readonly IObservableReadCachedQueryableReadOnlyDictionary<TKey, TValue> _decoratedObject;
public ObservableReadCachedQueryableReadOnlyDictionaryDecoratorBase(IObservableReadCachedQueryableReadOnlyDictionary<TKey, TValue> decoratedObject) {
_decoratedObject = decoratedObject;
}
LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> IObservableReadOnlyDictionary<TKey, TValue>.ToLiveLinq() {
return _decoratedObject.ToLiveLinq();
}
}
}

