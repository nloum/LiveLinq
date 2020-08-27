using System;
using LiveLinq.Core;

namespace LiveLinq.Set
{
    public class ReadOnlyObservableSet<T> : ObservableSet<T>
    {
        public ReadOnlyObservableSet(ISetChanges<T> source)
        {
            Disposes(source.AsObservable().Subscribe(Write));
        }
        
        /// <summary>
        /// Applies the set change to this set.
        /// </summary>
        private void Write(ISetChange<T> change)
        {
            switch (change.Type)
            {
                case CollectionChangeType.Add:
                {
                    AddRange(change.Values);
                    break;
                }
                case CollectionChangeType.Remove:
                {
                    RemoveRange(change.Values);
                    break;
                }
                default:
                    throw new NotImplementedException();
            }
        }
    }
}