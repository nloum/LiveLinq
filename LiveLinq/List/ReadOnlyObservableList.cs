using System;

namespace LiveLinq.List
{
    internal class ReadOnlyObservableList<T> : ObservableList<T>
    {
        public ReadOnlyObservableList(IListChanges<T> source)
        {
            Disposes(source.AsObservable().Subscribe(Mutate));
        }
    }
}
