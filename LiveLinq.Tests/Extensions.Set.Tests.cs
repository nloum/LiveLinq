using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using FluentAssertions;
using LiveLinq.Set;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveLinq.Tests
{
    [TestClass]
    public class Extensions_Set_Tests
    {
        [TestMethod]
        public void SelectObservableNeverShouldBeTreatedAsFalse()
        {
            var uut = new ObservableSet<string>();

            var result = uut.ToLiveLinq().Select((value) =>
                    Observable.Never<string>())
                .ToReadOnlySet();

            uut.Add("a");
            uut.Add("b");

            result.Should().BeEmpty();
        }
        
        [TestMethod]
        public void SelectObservableEmptyShouldBeTreatedAsFalse()
        {
            var uut = new ObservableSet<string>();

            var result = uut.ToLiveLinq().Select((value) =>
                    Observable.Empty<string>())
                .ToReadOnlySet();
            
            uut.Add("a");
            uut.Add("b");

            result.Should().BeEmpty();
        }
        
        [TestMethod]
        public void SelectObservableReturnShouldBeProcessedCorrectly()
        {
            var uut = new ObservableSet<string>();

            var result = uut.ToLiveLinq().Select((value) =>
                    Observable.Return(value))
                .ToReadOnlySet();
            
            uut.Add("a");
            uut.Add("b");

            result.Should().BeEquivalentTo("a", "b");

            uut.Remove("a");
            
            result.Should().BeEquivalentTo("b");

            uut.Remove("b");

            result.Should().BeEmpty();
        }
        
        [TestMethod]
        public void UnendingSelectObservableOnDifferentThreadShouldBeIgnoredIfItemIsRemoved()
        {
            var uut = new ObservableSet<string>();

            var rest = Observable.Interval(TimeSpan.FromSeconds(1)).Select(x => "x")
                .ObserveOn(NewThreadScheduler.Default)
                .SubscribeOn(NewThreadScheduler.Default)
                .Publish();
            var timer = Observable.Return("x").Concat(rest);

            using (rest.Connect())
            {
                var result = uut.ToLiveLinq().Select((value) => timer.AsObservable())
                    .ToReadOnlySet();

                result.Should().BeEmpty();
                
                uut.Add("a");
                result.Should().BeEquivalentTo("x");
                uut.Remove("a");
                result.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void UnendingSelectObservableShouldBeIgnoredIfItemIsRemoved()
        {
            var uut = new ObservableSet<string>();

            var result = uut.ToLiveLinq().Select((value) =>
                    Observable.Return(value).Concat(Observable.Interval(TimeSpan.FromSeconds(.1)).Select(_ => value + _)))
                .ToReadOnlySet();
            
            uut.Add("a");
            uut.Add("b");

            result.Should().BeEquivalentTo("a", "b");

            uut.Remove("a");

            result.Should().BeEquivalentTo("b");
            
            uut.Remove("b");

            result.Should().BeEmpty();
        }
        
        [TestMethod]
        public void WhereObservableNeverShouldBeTreatedAsFalse()
        {
            var uut = new ObservableSet<string>();

            var result = uut.ToLiveLinq().Where((value) =>
                    Observable.Never<bool>())
                .ToReadOnlySet();
            
            uut.Add("a");
            uut.Add("b");

            result.Should().BeEmpty();
        }
        
        [TestMethod]
        public void WhereObservableEmptyShouldBeTreatedAsFalse()
        {
            var uut = new ObservableSet<string>();

            var result = uut.ToLiveLinq().Where((value) =>
                    Observable.Empty<bool>())
                .ToReadOnlySet();
            
            uut.Add("a");
            uut.Add("b");

            result.Should().BeEmpty();
        }
        
        [TestMethod]
        public void WhereObservableReturnShouldBeProcessedCorrectly()
        {
            var uut = new ObservableSet<string>();

            var result = uut.ToLiveLinq().Where((value) =>
                    Observable.Return(true))
                .ToReadOnlySet();
            
            uut.Add("a");
            uut.Add("b");

            result.Should().BeEquivalentTo("a", "b");

            uut.Remove("a");
            
            result.Should().BeEquivalentTo("b");

            uut.Remove("b");

            result.Should().BeEmpty();
        }
        
        [TestMethod]
        public void UnendingWhereObservableOnDifferentThreadShouldBeIgnoredIfItemIsRemoved()
        {
            var uut = new ObservableSet<string>();

            var rest = Observable.Interval(TimeSpan.FromSeconds(1)).Select(x => true)
                .ObserveOn(NewThreadScheduler.Default)
                .SubscribeOn(NewThreadScheduler.Default)
                .Publish();
            var timer = Observable.Return(true).Concat(rest);

            using (rest.Connect())
            {
                var result = uut.ToLiveLinq().Where((value) => timer.AsObservable())
                    .ToReadOnlySet();

                result.Should().BeEmpty();
                
                uut.Add("a");
                result.Should().BeEquivalentTo("a");
                uut.Remove("a");
                result.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void UnendingWhereObservableShouldBeIgnoredIfItemIsRemoved()
        {
            var uut = new ObservableSet<string>();

            var result = uut.ToLiveLinq().Where((value) =>
                    Observable.Return(true).Concat(Observable.Interval(TimeSpan.FromSeconds(.1)).Select(_ => value.Length > 0)))
                .ToReadOnlySet();
            
            uut.Add("a");
            uut.Add("b");

            result.Should().BeEquivalentTo("a", "b");

            uut.Remove("a");

            result.Should().BeEquivalentTo("b");
            
            uut.Remove("b");

            result.Should().BeEmpty();
        }
    
    }
}