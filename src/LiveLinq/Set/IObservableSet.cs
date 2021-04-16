using ComposableCollections.Set;

namespace LiveLinq.Set
{
    /// <summary>
    /// A set data structure that provides callbacks for when it changes.
    /// </summary>
    /// <typeparam name="TValue">The type of item in the set</typeparam>
    public interface IObservableSet<TValue> : IComposableSet<TValue>, IObservableReadOnlySet<TValue>
    {
        
    }
}