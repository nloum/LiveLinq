using System;
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

            var values = uut.ToLiveLinq().ValuesAsSet().ToReadOnlySet();
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

            var values = uut.ToLiveLinq().KeysAsSet().ToReadOnlySet();
            values.Should().BeEquivalentTo(2, 3, 4);
            
            uut.Add(5, "d");
            uut[6] = "e";

            values.Should().BeEquivalentTo(2, 3, 4, 5, 6);
        }
    }
}