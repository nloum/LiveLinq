using System;

using MoreCollections;

namespace LiveLinq.List
{
    public interface IListChangesStrict<out T> : IListChanges<T>
    {
        new IObservable<IListChangeStrict<T>> AsObservable();
    }
}
