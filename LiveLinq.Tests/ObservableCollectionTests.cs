using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading;
using FluentAssertions;

using LiveLinq.List;
using LiveLinq.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveLinq.Tests
{
    [TestClass]
    public class ObservableCollectionTests
    {
        public ObservableCollectionTests()
        {
            if (SynchronizationContext.Current == null)
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }

        [TestMethod]
        public void WhereThenSelect()
        {
            var coll1 = new ObservableList<SelectObservableCollectionViewModel>();
            var result = coll1.ToLiveLinq().Where(nested => nested.Name.Select(val => val.Length > 2).ToBehaviorSubject())
                .Select(nested => nested.Name.AsObservable()).ToReadOnlyObservableList();
            result.Count.Should().Be(0);

            coll1.Add(new SelectObservableCollectionViewModel(string.Empty));
            result.Count.Should().Be(0);

            coll1[0].Name.OnNext("test");
            result.Count.Should().Be(1);
            result[0].Should().Be("test");

            coll1[0].Name.OnNext("test2");
            result.Count.Should().Be(1);
            result[0].Should().Be("test2");

            coll1[0].Name.OnNext("t");
            result.Count.Should().Be(0);
        }

        [TestMethod]
        public void SelectTwice()
        {
            var coll1 = new ObservableCollection<NestedViewModel>();
            var result = coll1.ToLiveLinq()
                .Select(nested => nested.Property.AsObservable())
                .Select(kvp => kvp.Name.AsObservable()).ToReadOnlyObservableList();
            result.Count.Should().Be(0);

            coll1.Add(new NestedViewModel(new SelectObservableCollectionViewModel("test")));
            result.Count.Should().Be(1);
            result[0].Should().Be("test");

            coll1[0].Property.Value.Name.OnNext("test2");
            result[0].Should().Be("test2");
        }

        [TestMethod]
        public void SelectThenExcept()
        {
            var coll1 = new ObservableCollection<SelectObservableCollectionViewModel>();
            coll1.Add(new SelectObservableCollectionViewModel("test"));
            var coll2 = new ObservableCollection<string>(new[] {"test"});
            var result1 = coll1.ToLiveLinq().Select(vm => vm.Name.AsObservable()).ToReadOnlyObservableList();
            var result2 = result1.ToLiveLinq().Except(coll2.ToLiveLinq()).ToReadOnlyObservableList();
            result1.Count.Should().Be(1);
            result1[0].Should().Be("test");
            result2.Count.Should().Be(0);

            coll1[0].Name.OnNext("test2");
            result1.Count.Should().Be(1);
            result1[0].Should().Be("test2");
            result2.Count.Should().Be(1);
            result2[0].Should().Be("test2");

            coll2[0] = "test2";
            result1.Count.Should().Be(1);
            result1[0].Should().Be("test2");
            result2.Count.Should().Be(0);

            coll2[0] = "test3";
            result1.Count.Should().Be(1);
            result1[0].Should().Be("test2");
            result2.Count.Should().Be(1);
            result2[0].Should().Be("test2");
        }

        [TestMethod]
        public void SelectThenExceptThenUnion()
        {
            var coll1 = new ObservableCollection<SelectObservableCollectionViewModel>();
            coll1.Add(new SelectObservableCollectionViewModel("test"));
            var coll2 = new ObservableCollection<string>(new[] { "test" });
            var result1 = coll1.ToLiveLinq().Select(vm => vm.Name.AsObservable()).ToReadOnlyObservableList();
            var result2 = result1.ToLiveLinq().Except(coll2.ToLiveLinq()).ToReadOnlyObservableList();
            var tmp = new SelectObservableCollectionViewModel("ha");
            var result3 = result2.ToLiveLinq().Concat(tmp.Name.ToLiveLinq<string>()).ToReadOnlyObservableList();
            result1.Count.Should().Be(1);
            result1[0].Should().Be("test");
            result2.Count.Should().Be(0);
            result3.Count.Should().Be(1);
            result3[0].Should().Be("ha");

            coll1[0].Name.OnNext("test2");
            result1.Count.Should().Be(1);
            result1[0].Should().Be("test2");
            result2.Count.Should().Be(1);
            result2[0].Should().Be("test2");
            result3.Should().BeEquivalentTo("test2", "ha");

            coll2[0] = "test2";
            result1.Count.Should().Be(1);
            result1[0].Should().Be("test2");
            result2.Count.Should().Be(0);
            result3.Should().BeEquivalentTo("ha");

            coll2[0] = "test3";
            result1.Count.Should().Be(1);
            result1[0].Should().Be("test2");
            result2.Should().BeEquivalentTo("test2");
            result3.Should().BeEquivalentTo("test2", "ha");

            coll2[0] = "ha";
            result3.Should().BeEquivalentTo("test2", "ha");
        }
    }
}
