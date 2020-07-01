using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using FluentAssertions;
using LiveLinq.Dictionary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveLinq.Tests
{
    [TestClass]
    public class Extensions_Dictionary
    {
        [TestMethod]
        public void SelectObservableNeverShouldBeTreatedAsFalse()
        {
            var uut = new ObservableDictionary<int, string>();

            var result = uut.ToLiveLinq().SelectValue((key, value) =>
                    Observable.Never<string>())
                .ToReadOnlyObservableDictionary();
            
            uut[1] = "a";
            uut[2] = "b";

            result.Values.Should().BeEmpty();
        }
        
        [TestMethod]
        public void SelectObservableEmptyShouldBeTreatedAsFalse()
        {
            var uut = new ObservableDictionary<int, string>();

            var result = uut.ToLiveLinq().SelectValue((key, value) =>
                    Observable.Empty<string>())
                .ToReadOnlyObservableDictionary();
            
            uut[1] = "a";
            uut[2] = "b";

            result.Values.Should().BeEmpty();
        }
        
        [TestMethod]
        public void SelectObservableReturnShouldBeProcessedCorrectly()
        {
            var uut = new ObservableDictionary<int, string>();

            var result = uut.ToLiveLinq().SelectValue((key, value) =>
                    Observable.Return(value))
                .ToReadOnlyObservableDictionary();
            
            uut[1] = "a";
            uut[2] = "b";

            result.Values.Should().BeEquivalentTo("a", "b");

            uut.Remove(1);
            
            result.Values.Should().BeEquivalentTo("b");

            uut.Remove(2);

            result.Values.Should().BeEmpty();
        }
        
        [TestMethod]
        public void UnendingSelectObservableOnDifferentThreadShouldBeIgnoredIfItemIsRemoved()
        {
            var uut = new ObservableDictionary<int, string>();

            var timer = Observable.Interval(TimeSpan.FromSeconds(1)).Select(x => "x")
                .ObserveOn(NewThreadScheduler.Default)
                .SubscribeOn(NewThreadScheduler.Default)
                .Publish();

            using (timer.Connect())
            {
                var result = uut.ToLiveLinq().SelectValue((key, value) => key == 1 ? timer : Observable.Return(value))
                    .ToReadOnlyObservableDictionary();

                result.Values.Should().BeEmpty();
                
                uut[2] = "a";
                result.Values.Should().BeEquivalentTo("a");
                uut.Remove(2);
                result.Values.Should().BeEmpty();

                uut[1] = "a";
                result.Values.Should().BeEmpty();
                uut.Remove(1);
                result.Values.Should().BeEmpty();

                uut[2] = "a";
                result.Values.Should().BeEquivalentTo("a");
                uut.Remove(2);
                result.Values.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void UnendingSelectObservableShouldBeIgnoredIfItemIsRemoved()
        {
            var uut = new ObservableDictionary<int, string>();

            var result = uut.ToLiveLinq().SelectValue((key, value) =>
                    Observable.Return(value).Concat(Observable.Interval(TimeSpan.FromSeconds(.1)).Select(_ => value + _)))
                .ToReadOnlyObservableDictionary();
            
            uut[1] = "a";
            uut[2] = "b";

            result.Values.Should().BeEquivalentTo("a", "b");

            uut.Remove(1);

            result.Values.Should().BeEquivalentTo("b");
            
            uut.Remove(2);

            result.Values.Should().BeEmpty();
        }
        
        [TestMethod]
        public void WhereObservableNeverShouldBeTreatedAsFalse()
        {
            var uut = new ObservableDictionary<int, string>();

            var result = uut.ToLiveLinq().Where((key, value) =>
                    Observable.Never<bool>())
                .ToReadOnlyObservableDictionary();
            
            uut[1] = "a";
            uut[2] = "b";

            result.Values.Should().BeEmpty();
        }
        
        [TestMethod]
        public void WhereObservableEmptyShouldBeTreatedAsFalse()
        {
            var uut = new ObservableDictionary<int, string>();

            var result = uut.ToLiveLinq().Where((key, value) =>
                    Observable.Empty<bool>())
                .ToReadOnlyObservableDictionary();
            
            uut[1] = "a";
            uut[2] = "b";

            result.Values.Should().BeEmpty();
        }
        
        [TestMethod]
        public void WhereObservableReturnShouldBeProcessedCorrectly()
        {
            var uut = new ObservableDictionary<int, string>();

            var result = uut.ToLiveLinq().Where((key, value) =>
                    Observable.Return(true))
                .ToReadOnlyObservableDictionary();
            
            uut[1] = "a";
            uut[2] = "b";

            result.Values.Should().BeEquivalentTo("a", "b");

            uut.Remove(1);
            
            result.Values.Should().BeEquivalentTo("b");

            uut.Remove(2);

            result.Values.Should().BeEmpty();
        }
        
        [TestMethod]
        public void UnendingWhereObservableOnDifferentThreadShouldBeIgnoredIfItemIsRemoved()
        {
            var uut = new ObservableDictionary<int, string>();

            var timer = Observable.Interval(TimeSpan.FromSeconds(1)).Select(x => true)
                .ObserveOn(NewThreadScheduler.Default)
                .SubscribeOn(NewThreadScheduler.Default)
                .Publish();

            using (timer.Connect())
            {
                var result = uut.ToLiveLinq().Where((key, value) => key == 1 ? timer : Observable.Return(true))
                    .ToReadOnlyObservableDictionary();

                result.Values.Should().BeEmpty();
                
                uut[2] = "a";
                result.Values.Should().BeEquivalentTo("a");
                uut.Remove(2);
                result.Values.Should().BeEmpty();

                uut[1] = "a";
                result.Values.Should().BeEmpty();
                uut.Remove(1);
                result.Values.Should().BeEmpty();

                uut[2] = "a";
                result.Values.Should().BeEquivalentTo("a");
                uut.Remove(2);
                result.Values.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void UnendingWhereObservableShouldBeIgnoredIfItemIsRemoved()
        {
            var uut = new ObservableDictionary<int, string>();

            var result = uut.ToLiveLinq().Where((key, value) =>
                    Observable.Return(true).Concat(Observable.Interval(TimeSpan.FromSeconds(.1)).Select(_ => value.Length > 0)))
                .ToReadOnlyObservableDictionary();
            
            uut[1] = "a";
            uut[2] = "b";

            result.Values.Should().BeEquivalentTo("a", "b");

            uut.Remove(1);

            result.Values.Should().BeEquivalentTo("b");
            
            uut.Remove(2);

            result.Values.Should().BeEmpty();
        }
    
        [TestMethod]
        public void ValuesAsSetShouldWork()
        {
            var uut = new ObservableDictionary<int, string>();
            uut[2] = "a";
            uut[3] = "b";
            uut.Add(4, "c");

            var values = uut.ToLiveLinq().ValuesAsSet().ToReadOnlyObservableSet();
            values.Should().BeEquivalentTo("a", "b", "c");
            
            uut.Add(5, "d");
            uut[6] = "e";

            values.Should().BeEquivalentTo("a", "b", "c", "d", "e");
        }
        
        [TestMethod]
        public void KeysAsSetShouldWork()
        {
            var uut = new ObservableDictionary<int, string>();
            uut[2] = "a";
            uut[3] = "b";
            uut.Add(4, "c");

            var values = uut.ToLiveLinq().KeysAsSet().ToReadOnlyObservableSet();
            values.Should().BeEquivalentTo(2, 3, 4);
            
            uut.Add(5, "d");
            uut[6] = "e";

            values.Should().BeEquivalentTo(2, 3, 4, 5, 6);
        }
    }
}