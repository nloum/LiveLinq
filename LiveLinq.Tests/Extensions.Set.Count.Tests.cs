using FluentAssertions;
using LiveLinq.Set;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;

namespace LiveLinq.Tests
{
    [TestClass]
    public class Extensions_Set_Count_Tests
    {
        [TestMethod]
        public void CountShouldWork()
        {
            var uut = new ObservableSet<string>();
            uut.Add("a");

            var result = uut.ToLiveLinq().Count().ToReadOnlyReactiveProperty();

            result.Value.Should().Be(1);
           
            uut.Add("b");
            result.Value.Should().Be(2);
            
            uut.Remove("a");
            result.Value.Should().Be(1);
        
            uut.Remove("b");
            result.Value.Should().Be(0);
        }
    }
}