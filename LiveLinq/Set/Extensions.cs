using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveLinq.Core;

namespace LiveLinq.Set
{
    public static partial class Extensions
    {
        public static IReadOnlyObservableSet<T> ToReadOnlySet<T>(this ISetChanges<T> source)
        {
            var result = new ObservableSet<T>();
            source.AsObservable().Subscribe(x =>
            {
                if (x.Type == CollectionChangeType.Add)
                {
                    result.AddRange(x.Values);
                }
                else if (x.Type == CollectionChangeType.Remove)
                {
                    result.RemoveRange(x.Values);
                }
                else
                {
                    throw new ArgumentException($"Unknown change type {x.Type}");
                }
            });
            return result;
        }

        public static ISetChanges<T> ToLiveLinq<T>(this IObservable<ISetChange<T>> source)
        {
            return new SetChanges<T>(source);
        }
    }
}
