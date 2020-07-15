using System;
using System.Collections.Generic;
using System.Linq;
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
        public void SetAddRangeSubscribeShouldProcessAllItemsInRange()
        {
            var source = new ObservableSet<string>();
            var items = new List<string>();
            using (source.ToLiveLinq().Subscribe(str => { items.Add(str); }, (str, mode) => { }))
            {
                source.AddRange(new [] { "A", "B", "C", "D" });
            }
            items.Should().BeEquivalentTo("A", "B", "C", "D");
        }
    
        [TestMethod]
        public void SelectObservableNeverShouldBeTreatedAsFalse()
        {
            var uut = new ObservableSet<string>();

            var result = uut.ToLiveLinq().Select((value) =>
                    Observable.Never<string>())
                .ToReadOnlyObservableSet();

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
                .ToReadOnlyObservableSet();
            
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
                .ToReadOnlyObservableSet();
            
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
                    .ToReadOnlyObservableSet();

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
                .ToReadOnlyObservableSet();
            
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
                .ToReadOnlyObservableSet();
            
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
                .ToReadOnlyObservableSet();
            
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
                .ToReadOnlyObservableSet();
            
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
                    .ToReadOnlyObservableSet();

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
                .ToReadOnlyObservableSet();
            
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