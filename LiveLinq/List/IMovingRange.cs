using UtilityDisposables;

using GenericNumbers;

namespace LiveLinq.List
{
    public interface IMovingRange<out TStatePerMovingRange>
    {
        TStatePerMovingRange State { get; }
        int BaseCount { get; }
        int Count { get; }
        int LastKnownIndex { get; }
        INumberRange<int> Range { get; }
    }
}
