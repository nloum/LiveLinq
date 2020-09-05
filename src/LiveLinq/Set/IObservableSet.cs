using ComposableCollections.Set;

namespace LiveLinq.Set
{
    public interface IObservableSet<TValue> : IComposableSet<TValue>, IObservableReadOnlySet<TValue>
    {
        
    }
}