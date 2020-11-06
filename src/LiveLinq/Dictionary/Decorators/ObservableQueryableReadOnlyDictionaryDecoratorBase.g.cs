using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Decorators {
public class ObservableQueryableReadOnlyDictionaryDecoratorBase<TKey, TValue> : IObservableQueryableReadOnlyDictionary<TKey, TValue> {
private readonly IObservableQueryableReadOnlyDictionary<TKey, TValue> _decoratedObject;
public ObservableQueryableReadOnlyDictionaryDecoratorBase(IObservableQueryableReadOnlyDictionary<TKey, TValue> decoratedObject) {
_decoratedObject = decoratedObject;
}
LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> IObservableReadOnlyDictionary<TKey, TValue>.ToLiveLinq() {
return _decoratedObject.ToLiveLinq();
}
}
}

