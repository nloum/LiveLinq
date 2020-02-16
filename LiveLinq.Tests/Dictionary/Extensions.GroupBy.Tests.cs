using FluentAssertions;

using LiveLinq.Dictionary;
using LiveLinq.List;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveLinq.Tests.Dictionary
{
    [TestClass]
    public partial class ExtensionsTests
    {
        [TestMethod]
        public void GroupBy()
        {
            var source = new ObservableList<string>();
            var property = source.ToLiveLinq()
                .GroupBy(str => str.Length)
                .SelectValue((key, value) => value.ToObservableEnumerable().ToBehaviorSubject())
                .ToObservableEnumerable()
                .ToBehaviorSubject();
            source.Add("4444");
            source.Add("4444");
            source.Add("55555");
            property.Value.Count.Should().Be(2);
            property.Value[4].Value.Should().ContainInOrder("4444", "4444");
            property.Value[5].Value.Should().ContainInOrder("55555");
        }
    }
}
