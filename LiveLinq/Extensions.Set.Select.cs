using System;
using System.Collections.Generic;
using System.Text;
using LiveLinq.Set;

namespace LiveLinq
{
    public static partial class Extensions
    {
		public static ISetChanges<T2> Select<T1, T2>(this ISetChanges<T1> source, Func<T1, IObservable<T2>> selector)
        {
            throw new NotImplementedException();
        }
    }
}
