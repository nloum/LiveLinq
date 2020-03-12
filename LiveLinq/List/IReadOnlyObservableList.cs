using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace LiveLinq.List
{
    /// <remarks>
    /// This interfaces can be used for bindings, because it implements INotifyCollectionChanged.
    /// </remarks>
    public interface IReadOnlyObservableList<out T> : INotifyCollectionChanged, IReadOnlyList<T>, IDisposable
    {
        IListChangesStrict<T> ToLiveLinq();
    }
}
