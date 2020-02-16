using UtilityDisposables;

using GenericNumbers;

namespace LiveLinq.List
{
    /// <summary>
    /// SelectMany creates exactly one of these objects for each subcollection in its input.
    /// </summary>
    internal class MovingRange<TStatePerMovingRange> : IMovingRange<TStatePerMovingRange>
    {
        public MovingRange(TStatePerMovingRange state)
        {
            this.State = state;
        }

        public TStatePerMovingRange State { get; }

        /// <summary>
        /// The instantaneously-updated number of items in this SelectMany result.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// This is the number of total items in the SelectMany result that are earlier in the SelectMany
        /// result than the items that come from the collection represented by this <see cref="MovingRange{TStatePerMovingRange}"/>.
        /// </summary>
        public int BaseCount { get; set; }

        /// <summary>
        /// The last known index of this collection in the collection of collections that make up the input
        ///  for the SelectMany query.
        /// </summary>
        public int LastKnownIndex { get; set; }

        public INumberRange<int> Range => GenericNumbers.NumbersUtility.Range(this.BaseCount, this.BaseCount + this.Count);
    }
}
