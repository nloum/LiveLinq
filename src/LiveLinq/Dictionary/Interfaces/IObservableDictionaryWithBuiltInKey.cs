using ComposableCollections.Dictionary.Interfaces;
using ComposableCollections.Dictionary.WithBuiltInKey.Interfaces;

namespace LiveLinq.Dictionary.Interfaces
{
    public interface IObservableDictionaryWithBuiltInKey<TKey, TValue> : IObservableReadOnlyDictionaryWithBuiltInKey<TKey, TValue>, IDisposableDictionaryWithBuiltInKey<TKey, TValue>
    {
        IObservableDictionary<TKey, TValue> AsObservableDictionary();
    }
}