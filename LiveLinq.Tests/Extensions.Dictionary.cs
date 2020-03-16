using FluentAssertions;
using LiveLinq.Dictionary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveLinq.Tests
{
    [TestClass]
    public class Extensions_Dictionary
    {
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