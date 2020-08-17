using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    public interface IObservableDictionaryWithBuiltInKey<TKey, TValue> : IDictionaryWithBuiltInKey<TKey, TValue>, IReadOnlyObservableDictionaryWithBuiltInKey<TKey, TValue>
    {
    }
}