using System.Collections.Immutable;

using MoreCollections;

namespace LiveLinq.List
{
    public class ListStateAndChange<T>
    {
        public ImmutableList<T> State { get; }
        public IListChangeStrict<T> MostRecentChange { get; }

        internal ListStateAndChange(ImmutableList<T> state, IListChangeStrict<T> mostRecentChange)
        {
            this.State = state;
            this.MostRecentChange = mostRecentChange;
        }

        public ListStateAndChange()
        {
            this.State = ImmutableList<T>.Empty;
        }
    }
}
