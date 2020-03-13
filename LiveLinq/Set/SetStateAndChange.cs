using System.Collections.Immutable;
using LiveLinq.List;

namespace LiveLinq.Set
{
    public class SetStateAndChange<T>
    {
        public ImmutableHashSet<T> State { get; }
        public ISetChange<T> MostRecentChange { get; }

        internal SetStateAndChange(ImmutableHashSet<T> state, ISetChange<T> mostRecentChange)
        {
            this.State = state;
            this.MostRecentChange = mostRecentChange;
        }

        public SetStateAndChange()
        {
            this.State = ImmutableHashSet<T>.Empty;
        }
    }
}