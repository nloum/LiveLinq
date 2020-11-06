using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Decorators {
public class ObservableDisposableQueryableReadOnlyDictionaryDecoratorBase<TKey, TValue> : IObservableDisposableQueryableReadOnlyDictionary<TKey, TValue> {
private readonly IObservableDisposableQueryableReadOnlyDictionary<TKey, TValue> _decoratedObject;
public ObservableDisposableQueryableReadOnlyDictionaryDecoratorBase(IObservableDisposableQueryableReadOnlyDictionary<TKey, TValue> decoratedObject) {
_decoratedObject = decoratedObject;
}
LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> IObservableReadOnlyDictionary<TKey, TValue>.ToLiveLinq() {
return _decoratedObject.ToLiveLinq();
}
}
}

