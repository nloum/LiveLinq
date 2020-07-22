using FluentAssertions;
using LiveLinq.Dictionary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleMonads;

namespace LiveLinq.Tests.Dictionary
{
    [TestClass]
    public class ObservableDictionaryGetOrDefaultTests
    {
        [TestMethod]
        public void ShouldReturnDefaultValue()
        {
            var uut = new ObservableDictionaryGetOrDefault<int, string>(
                (int key, out IMaybe<string> value, out bool persist) =>
                {
                    persist = true;
                    value = "Hi".ToMaybe();
                });

            uut[4].Should().Be("Hi");
        }
    }
}