using System;
using System.Reactive.Subjects;

namespace LiveLinq.Tests
{
    public static class TestExtensions
    {
        public static BehaviorSubject<T> ToBehaviorSubject<T>(this IObservable<T> source)
        {
            var result = new BehaviorSubject<T>(default);
            source.Subscribe(result);
            return result;
        }
    }
}