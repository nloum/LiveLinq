using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using LiveLinq.Dictionary;
using LiveLinq.Set;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveLinq.Tests
{
    [TestClass]
    public class Extensions_Set_Where_Tests
    {
        [TestMethod]
        public void EmptyWhere()
        {
            var source = new ObservableDictionary<int, string>();
            //source.Add("B");
            var concatMe = new List<string>() { "A" };
            var result = source.ToLiveLinq().ValuesAsSet().Concat(concatMe.ToLiveLinqUnchangingSet(), true)
                .Select(x => x)
                .ToReadOnlyObservableSet();
            result.Should().BeEquivalentTo("A");
        }
    }
}