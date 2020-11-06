using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Decorators {
public class ObservableReadCachedDictionaryDecoratorBase<TKey, TValue> : IObservableReadCachedDictionary<TKey, TValue> {
private readonly IObservableReadCachedDictionary<TKey, TValue> _decoratedObject;
public ObservableReadCachedDictionaryDecoratorBase(IObservableReadCachedDictionary<TKey, TValue> decoratedObject) {
_decoratedObject = decoratedObject;
}
LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> IObservableReadOnlyDictionary<TKey, TValue>.ToLiveLinq() {
return _decoratedObject.ToLiveLinq();
}
}
}

