using FluentAssertions;
using LiveLinq.List;
using LiveLinq.Ordered;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveLinq.Tests.Ordered
{
    [TestClass]
    public class ExtensionsTests
    {
        [TestMethod]
        [DataRow("", "a,b,c")]
        [DataRow("", "c,a,b")]
        [DataRow("a,b,c", "")]
        [DataRow("c,a,b", "")]
        [DataRow("a,b,c", "a,b,c")]
        [DataRow("c,a,b", "a,b,c")]
        [DataRow("a,b,c", "c,a,b")]
        [DataRow("c,a,b", "c,a,b")]
        public void ShouldReorderInitialState(string initialItems, string subsequentItems)
        {
            var uut = new ObservableList<string>();
            foreach (var item in initialItems.Split(","))
            {
                uut.Add(item);
            }
            var result = uut.ToLiveLinq().OrderBy(x => x).ToReadOnlyObservableList();
            result.Should().BeInAscendingOrder();

            foreach (var item in subsequentItems.Split(","))
            {
                uut.Add(item);
            }
            result.Should().BeInAscendingOrder();
        }
    }
}