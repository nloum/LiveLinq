using System;
using System.Collections.Generic;
using System.Text;
using LiveLinq.Set;
using System.Reactive.Linq;

namespace LiveLinq
{
    public static partial class Extensions
    {
		public static ISetChanges<T> Concat<T>(this ISetChanges<T> left, ISetChanges<T> right, bool assumeNotEqual = false)
        {
			if (!assumeNotEqual)
            {
                throw new NotImplementedException();
            }

            return new SetChanges<T>(left.AsObservable().Merge(right.AsObservable()));
        }
    }
}
