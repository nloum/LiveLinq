using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading;
using FluentAssertions;

using LiveLinq.List;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveLinq.Tests
{
    [TestClass]
    public class WhereObservableCollectionTests
    {
        public WhereObservableCollectionTests()
        {
            if (SynchronizationContext.Current == null)
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }

        [TestMethod]
        public void ChangeIncludeToTrue()
        {
            var vms =
                new ObservableCollection<WhereObservableCollectionViewModel>(new[]
                {new WhereObservableCollectionViewModel(false)});
            var result = vms.ToLiveLinq().Where(vm => vm.Predicate).ToReadOnlyObservableList();
            result.Count.Should().Be(0);
            vms[0].Predicate.OnNext(true);
            result.Count.Should().Be(1);
        }

        [TestMethod]
        public void AddThenChangeIncludeToTrue()
        {
            var vms = new ObservableCollection<WhereObservableCollectionViewModel>();
            var result = vms.ToLiveLinq().Where(vm => vm.Predicate).ToReadOnlyObservableList();
            result.Count.Should().Be(0);
            vms.Add(new WhereObservableCollectionViewModel(false));
            result.Count.Should().Be(0);
            vms[0].Predicate.OnNext(true);
            result.Count.Should().Be(1);
        }

        [TestMethod]
        public void AddThenChangeIncludeToFalse()
        {
            var vms = new ObservableCollection<WhereObservableCollectionViewModel>();
            var result = vms.ToLiveLinq().Where(vm => vm.Predicate).ToReadOnlyObservableList();
            result.Count.Should().Be(0);
            vms.Add(new WhereObservableCollectionViewModel(true));
            result.Count.Should().Be(1);
            vms[0].Predicate.OnNext(false);
            result.Count.Should().Be(0);
        }

        [TestMethod]
        public void RemoveThenChangeIncludeToTrue()
        {
            var vms = new ObservableCollection<WhereObservableCollectionViewModel>();
            var result = vms.ToLiveLinq().Where(vm2 => vm2.Predicate).ToReadOnlyObservableList();
            result.Count.Should().Be(0);
            var vm = new WhereObservableCollectionViewModel(false);
            vms.Add(vm);
            result.Count.Should().Be(0);
            vms.RemoveAt(0);
            vm.Predicate.OnNext(true);
            result.Count.Should().Be(0);
        }
    }
}
