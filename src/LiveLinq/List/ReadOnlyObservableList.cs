using System;

namespace LiveLinq.List
{
    /// <remarks>
    /// This class can be used for bindings, because it implements INotifyCollectionChanged.
    /// </remarks>
    internal class ReadOnlyObservableList<T> : ObservableList<T>
    {
        public ReadOnlyObservableList(IListChanges<T> source)
        {
            Disposes(source.AsObservable().Subscribe(Write));
        }
    }
}
