using System.Collections.Immutable;
using System.Linq;
using MoreCollections;
using LiveLinq.Core;
using static LiveLinq.Dictionary.Utility;

namespace LiveLinq.Dictionary
{
    public class DictionaryStateAndChange<TKey, TValue>
    {
        public ImmutableDictionary<TKey, TValue> State { get; }
        public IDictionaryChangeStrict<TKey, TValue> MostRecentChange { get; }

        private DictionaryStateAndChange(ImmutableDictionary<TKey, TValue> state, IDictionaryChangeStrict<TKey, TValue> mostRecentChange)
        {
            this.State = state;
            this.MostRecentChange = mostRecentChange;
        }

        public DictionaryStateAndChange()
        {
            this.State = ImmutableDictionary<TKey, TValue>.Empty;
        }

        public DictionaryStateAndChange<TKey, TValue> Accumulate(IDictionaryChangeStrict<TKey, TValue> change)
        {
            return new DictionaryStateAndChange<TKey, TValue>(State.Mutate(change), change);
        }

        public DictionaryStateAndChange<TKey, TValue> Accumulate(IDictionaryChange<TKey, TValue> change)
        {
            var strictChange = change.Type == CollectionChangeType.Add
                ? DictionaryAdd(change.Items)
                : DictionaryRemove(change.Keys.ToDictionary(key => key, key => State[key]));
            return new DictionaryStateAndChange<TKey, TValue>(State.Mutate(strictChange), strictChange);
        }
    }
}