using System.Collections.ObjectModel;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveLinq.Tests.List
{
    [TestClass]
    public partial class ExtensionsTests
    {
        [TestMethod]
        public void ObservableCollection_Add()
        {
            var source = new ObservableCollection<string>();
            var snapshot = source.ToLiveLinq().ToObservableEnumerable().ToBehaviorSubject();
            source.Add("A");
            snapshot.Value.Should().ContainSingle("A");
        }

        [TestMethod]
        public void ObservableCollection_Remove()
        {
            var source = new ObservableCollection<string>() { "A" };
            var snapshot = source.ToLiveLinq().ToObservableEnumerable().ToBehaviorSubject();
            source.RemoveAt(0);
            snapshot.Value.Should().BeEmpty();
        }

        [TestMethod]
        public void ObservableCollection_Clear()
        {
            var source = new ObservableCollection<string>() { "A", "B", "C" };
            var snapshot = source.ToLiveLinq().ToObservableEnumerable().ToBehaviorSubject();
            source.Clear();
            snapshot.Value.Should().BeEmpty();
        }

        [TestMethod]
        public void ObservableCollection_AddRemove()
        {
            var source = new ObservableCollection<string>();
            var snapshot = source.ToLiveLinq().ToObservableEnumerable().ToBehaviorSubject();
            source.Add("A");
            source.RemoveAt(0);
            snapshot.Value.Should().BeEmpty();
        }
    }
}
