using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace LiveLinq.List
{
    public interface IReadOnlyObservableList<out T> : INotifyCollectionChanged, IReadOnlyList<T>, IDisposable
    {
        IListChangesStrict<T> ToLiveLinq();
    }
}
