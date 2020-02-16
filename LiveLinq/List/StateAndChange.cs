using System.Collections.Immutable;

using MoreCollections;

namespace LiveLinq.List
{
    public class StateAndChange<T>
    {
        public ImmutableList<T> State { get; }
        public IListChangeStrict<T> MostRecentChange { get; }

        internal StateAndChange(ImmutableList<T> state, IListChangeStrict<T> mostRecentChange)
        {
            this.State = state;
            this.MostRecentChange = mostRecentChange;
        }

        public StateAndChange()
        {
            this.State = ImmutableList<T>.Empty;
        }
    }
}
