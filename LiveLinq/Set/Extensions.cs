using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveLinq.Set
{
    public static partial class Extensions
    {
        public static ISetChanges<T> ToLiveLinq<T>(this IObservable<ISetChange<T>> source)
        {
            return new SetChanges<T>(source);
        }
    }
}
