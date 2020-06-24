using System;
using System.Collections;
using System.Collections.Generic;

using GenericNumbers;

using MoreCollections;

namespace LiveLinq.List
{
    public class MovingRanges<TStatePerMovingRange> : IReadOnlyList<IMovingRange<TStatePerMovingRange>>
    {
        private readonly List<MovingRange<TStatePerMovingRange>> _movingRanges = new List<MovingRange<TStatePerMovingRange>>();

        /// <summary>
        /// This variable records in which subcollection the most recent change occurred.
        /// It gets updated every time a subcollection changes.
        /// It is used when a subcollection changes to determine if the subcollection's BaseCount and LastKnownIndex properties
        /// need to be recalculated.
        /// </summary>
        private int _lastKnownIndicesGreaterThanThisAreInvalid = -1;

        public int Count => _movingRanges.Count;

        public IMovingRange<TStatePerMovingRange> this[int index] => this._movingRanges[index];

        public void UpdateCount(IMovingRange<TStatePerMovingRange> movingRange, int newCount)
        {
            ((MovingRange<TStatePerMovingRange>)movingRange).Count = newCount;
            this._lastKnownIndicesGreaterThanThisAreInvalid = movingRange.LastKnownIndex;
        }
        
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)this._movingRanges).GetEnumerator();
        }

        IEnumerator<IMovingRange<TStatePerMovingRange>> IEnumerable<IMovingRange<TStatePerMovingRange>>.GetEnumerator()
        {
            return _movingRanges.GetEnumerator();
        }

        public void RecalculateSubCollectionsOffset(
            IMovingRange<TStatePerMovingRange> movingRange)
        {
            if (movingRange.LastKnownIndex > this._lastKnownIndicesGreaterThanThisAreInvalid)
            {
                var currentBaseCount = this._lastKnownIndicesGreaterThanThisAreInvalid >= 0
                                           ? this._movingRanges[this._lastKnownIndicesGreaterThanThisAreInvalid].BaseCount
                                             + this._movingRanges[this._lastKnownIndicesGreaterThanThisAreInvalid].Count
                                           : 0;
                var i = this._lastKnownIndicesGreaterThanThisAreInvalid + 1;
                while (i < this._movingRanges.Count)
                {
                    this._movingRanges[i].LastKnownIndex = i;
                    this._movingRanges[i].BaseCount = currentBaseCount;
                    if (ReferenceEquals(this._movingRanges[i], movingRange))
                    {
                        break;
                    }
                    currentBaseCount += this._movingRanges[i].Count;
                    i++;
                }
            }
        }

        public IMovingRange<TStatePerMovingRange> Insert(int index, TStatePerMovingRange state)
        {
            var movingRange = new MovingRange<TStatePerMovingRange>(state);
            this._movingRanges.Insert(index, movingRange);
            movingRange.LastKnownIndex = index;
            this._lastKnownIndicesGreaterThanThisAreInvalid = Math.Min(this._lastKnownIndicesGreaterThanThisAreInvalid, index - 1);
            return movingRange;
        }

        public void RemoveAt(int index)
        {
            this._movingRanges.RemoveAt(index);
            this._lastKnownIndicesGreaterThanThisAreInvalid = index - 1;
        }

        public void RemoveRange(INumberRange<int> range)
        {
            this._movingRanges.RemoveRange(range);
            this._lastKnownIndicesGreaterThanThisAreInvalid = range.LowerBound.ChangeStrictness(true).Value;
        }
    }
}
