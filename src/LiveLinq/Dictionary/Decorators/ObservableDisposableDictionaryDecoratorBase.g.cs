using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Decorators {
public class ObservableDisposableDictionaryDecoratorBase<TKey, TValue> : IObservableDisposableDictionary<TKey, TValue> {
private readonly IObservableDisposableDictionary<TKey, TValue> _decoratedObject;
public ObservableDisposableDictionaryDecoratorBase(IObservableDisposableDictionary<TKey, TValue> decoratedObject) {
_decoratedObject = decoratedObject;
}
LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> IObservableReadOnlyDictionary<TKey, TValue>.ToLiveLinq() {
return _decoratedObject.ToLiveLinq();
}
}
}

