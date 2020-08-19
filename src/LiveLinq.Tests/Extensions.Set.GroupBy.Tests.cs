using System.Linq;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using LiveLinq.List;
using LiveLinq.Set;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveLinq.Tests
{
    [TestClass]
    public class Extensions_Set_GroupBy_Tests
    {
        [TestMethod]
        public void GroupBy()
        {
            var source = new ObservableSet<string>();
            var result = source.ToLiveLinq()
                .GroupBy(str => str.Length)
                .SelectValue((key, values) => values.ToReadOnlyObservableSet())
                .ToReadOnlyObservableDictionary();
            source.Add("4444");
            source.Add("4444");
            source.Add("55555");
            result.Keys.Count().Should().Be(2);
            result[4].Should().BeEquivalentTo("4444"); // There should only be one item because we're dealing with sets, not lists
            result[5].Should().BeEquivalentTo("55555");
            
            source.Remove("55555");
            
            result.Keys.Count().Should().Be(2);
            result[4].Should().BeEquivalentTo("4444"); // There should only be one item because we're dealing with sets, not lists
            result[5].Should().BeEmpty();
            
            source.Remove("4444");
            result[4].Should().BeEmpty();
        }
    }
}