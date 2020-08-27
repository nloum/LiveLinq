using System;
using System.Collections.ObjectModel;
using ComposableCollections.Dictionary;
using FluentAssertions;

using LiveLinq.List;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoreCollections;


using static SimpleMonads.Utility;

namespace LiveLinq.Tests
{
    [TestClass]
    public class DictionaryLiveLinqQueryTests
    {
        [TestMethod]
        public void IObservableDictionaryShouldNotHaveRedundantMethodsOrProperties()
        {
            IObservableDictionary<int, string> uut = new ObservableDictionary<int, string>();
            uut[4] = "hi"; // this should compile
            uut[4].Should().Be("hi");
        }
        
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
            source[2] = "Hello";
            get2.Should().Be(Something("Hello"));
        }

        [TestMethod]
        public void ToDictionary_Reset()
        {
            var source = new ObservableCollection<IKeyValue<int, string>>();
            var get2 = Nothing<string>();
            source.ToLiveLinq().ToLiveLinq()[2].Subscribe(val => get2 = val);
            get2.Should().Be(Nothing<string>());
            source.Add(new KeyValue<int, string>(2, "Hi there"));
            get2.Should().Be(Something("Hi there"));
            source.RemoveAt(0);
            source.Add(new KeyValue<int, string>(2, "Hello"));
            get2.Should().Be(Something("Hello"));
        }
    }
}
