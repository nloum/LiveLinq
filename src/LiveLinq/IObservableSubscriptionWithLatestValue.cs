using System;
using SimpleMonads;

namespace LiveLinq
{
    public interface IObservableSubscriptionWithLatestValue<T> : IDisposable
    {
        IMaybe<T> LatestValue { get; }
    }
}