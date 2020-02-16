using System;
using System.Collections.ObjectModel;

using FluentAssertions;

using LiveLinq.List;
using LiveLinq.Dictionary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoreCollections;
using static MoreCollections.Utility;

using static SimpleMonads.Utility;

namespace LiveLinq.Tests
{
    [TestClass]
    public class DictionaryLiveLinqQueryTests
    {
        [TestMethod]
        public void ToDictionary_Get()
        {
            var source = new ObservableDictionary<int, string>();
            var get2 = Nothing<string>();
            source.ToLiveLinq()[2].Subscribe(val => get2 = val);
            get2.Should().Be(Nothing<string>());
            source.Add(2, "Hi there");
            get2.Should().Be(Something("Hi there"));
        }

        [TestMethod]
        public void ToDictionary_Unset()
        {
            var source = new ObservableDictionary<int, string>();
            var get2 = Nothing<string>();
            source.ToLiveLinq()[2].Subscribe(val => get2 = val);
            get2.Should().Be(Nothing<string>());
            source.Add(2, "Hi there");
            get2.Should().Be(Something("Hi there"));
            source.Remove(2);
            get2.Should().Be(Nothing<string>());
        }

        [TestMethod]
        public void ToDictionary_Unset_Set()
        {
            var source = new ObservableDictionary<int, string>();
            var get2 = Nothing<string>();
            source.ToLiveLinq()[2].Subscribe(val => get2 = val);
            get2.Should().Be(Nothing<string>());
            source.Add(2, "Hi there");
            get2.Should().Be(Something("Hi there"));
            source.Remove(2);
            get2.Should().Be(Nothing<string>());
            source.Add(2, "Hello");
            get2.Should().Be(Something("Hello"));
        }

        [TestMethod]
        public void ToDictionary_Reset()
        {
            var source = new ObservableCollection<IKeyValuePair<int, string>>();
            var get2 = Nothing<string>();
            source.ToLiveLinq().ToLiveLinq()[2].Subscribe(val => get2 = val);
            get2.Should().Be(Nothing<string>());
            source.Add(KeyValuePair(2, "Hi there"));
            get2.Should().Be(Something("Hi there"));
            source.RemoveAt(0);
            source.Add(KeyValuePair(2, "Hello"));
            get2.Should().Be(Something("Hello"));
        }
    }
}
