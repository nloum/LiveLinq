using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveLinq.Core;
using LiveLinq.Set;

namespace LiveLinq
{
    public static partial class Utility
    {
        public static ISetChange<T> EmptySetChange<T>(CollectionChangeType type = CollectionChangeType.Add)
        {
            return new SetChange<T>(type, ImmutableList<T>.Empty);
        }

        public static ISetChanges<T> EmptySetChanges<T>()
        {
            return new SetChanges<T>(Observable.Return(new SetChange<T>(CollectionChangeType.Add, ImmutableList<T>.Empty)));
        }
        
        public static ISetChange<T> SetChange<T>(CollectionChangeType type, params T[] elements)
        {
            return new SetChange<T>(type, elements.ToImmutableList());
        }

        public static ISetChange<T> SetChange<T>(CollectionChangeType type, ImmutableList<T> elements)
        {
            return new SetChange<T>(type, elements);
        }

        public static ISetChange<T> SetChange<T>(CollectionChangeType type, IEnumerable<T> elements)
        {
            return new SetChange<T>(type, elements.ToImmutableList());
        }
    }
}
