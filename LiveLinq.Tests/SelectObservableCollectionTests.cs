using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading;
using FluentAssertions;

using LiveLinq.List;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveLinq.Tests
{
    [TestClass]
    public class SelectObservableCollectionTests
    {
        public SelectObservableCollectionTests()
        {
            if (SynchronizationContext.Current == null)
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }

        [TestMethod]
        public void InitialThenChange()
        {
            var vm = new SelectObservableCollectionViewModel("test1");
            var vms = new ObservableCollection<SelectObservableCollectionViewModel>(new[] { vm });
            var result = vms.ToLiveLinq().Select(avm => avm.Name.AsObservable()).ToReadOnlyObservableList();
            result[0].Should().Be("test1");
            vm.Name.OnNext("test2");
            result[0].Should().Be("test2");
        }

        [TestMethod]
        public void AddThenChange()
        {
            var vm = new SelectObservableCollectionViewModel("test1");
            var vms = new ObservableCollection<SelectObservableCollectionViewModel>();
            var result = vms.ToLiveLinq().Select(avm => avm.Name.AsObservable()).ToReadOnlyObservableList();
            result.Should().BeEmpty();
            vms.Add(vm);
            result.Should().BeEquivalentTo("test1");
            vm.Name.OnNext("test2");
            result.Should().BeEquivalentTo("test2");
        }

        [TestMethod]
        public void AddTwiceThenChange()
        {
            var vm = new SelectObservableCollectionViewModel("test1");
            var vms = new ObservableCollection<SelectObservableCollectionViewModel>();
            var result = vms.ToLiveLinq().Select(avm => avm.Name.AsObservable()).ToReadOnlyObservableList();
            result.Should().BeEmpty();
            vms.Add(vm);
            vms.Add(vm);
            result.Should().BeEquivalentTo("test1", "test1");
            vm.Name.OnNext("test2");
            result.Should().BeEquivalentTo("test2", "test2");
        }

        [TestMethod]
        public void AddThriceThenRemove()
        {
            var vm = new SelectObservableCollectionViewModel("test1");
            var vms = new ObservableCollection<SelectObservableCollectionViewModel>();
            var result = vms.ToLiveLinq().Select(avm => avm.Name.AsObservable()).ToReadOnlyObservableList();
            result.Should().BeEmpty();
            vms.Add(vm);
            vms.Add(vm);
            vms.Add(vm);
            result.Should().BeEquivalentTo("test1", "test1", "test1");
            vms.RemoveAt(0);
            result.Should().BeEquivalentTo("test1", "test1");
        }

        [TestMethod]
        public void Remove()
        {
            var vms = new ObservableCollection<SelectObservableCollectionViewModel>();
            var result = vms.ToLiveLinq().Select(vm => vm.Name.AsObservable()).ToReadOnlyObservableList();
            result.Count.Should().Be(0);
            vms.Add(new SelectObservableCollectionViewModel("test1"));
            result.Count.Should().Be(1);
            vms.RemoveAt(0);
            result.Count.Should().Be(0);
        }

        [TestMethod]
        public void RemoveThenChange()
        {
            var vms = new ObservableCollection<SelectObservableCollectionViewModel>();
            var result = vms.ToLiveLinq().Select(vm => vm.Name.AsObservable()).ToReadOnlyObservableList();
            result.Count.Should().Be(0);
            var tmp = new SelectObservableCollectionViewModel("test1");
            vms.Add(tmp);
            result.Count.Should().Be(1);
            vms.RemoveAt(0);
            result.Count.Should().Be(0);
            tmp.Name.OnNext("test2");
            result.Count.Should().Be(0);
        }
        
        [TestMethod]
        public void SyncTo_InitialThenChange()
        {
            var vm = new SelectObservableCollectionViewModel("test1");
            var vms = new ObservableCollection<SelectObservableCollectionViewModel>(new[] { vm });
            var result = new List<string>();
            vms.ToLiveLinq().Select(avm => avm.Name.AsObservable()).SyncTo(result, false);
            result[0].Should().Be("test1");
            vm.Name.OnNext("test2");
            result[0].Should().Be("test2");
        }

        [TestMethod]
        public void SyncTo_AddThenChange()
        {
            var vm = new SelectObservableCollectionViewModel("test1");
            var vms = new ObservableCollection<SelectObservableCollectionViewModel>();
            var result = new List<string>();
            vms.ToLiveLinq().Select(avm => avm.Name.AsObservable()).SyncTo(result, false);
            result.Should().BeEmpty();
            vms.Add(vm);
            result.Should().BeEquivalentTo("test1");
            vm.Name.OnNext("test2");
            result.Should().BeEquivalentTo("test2");
        }
        
        [TestMethod]
        public void SyncTo_Remove()
        {
            var vms = new ObservableCollection<SelectObservableCollectionViewModel>();
            var result = new List<string>();
            vms.ToLiveLinq().Select(vm => vm.Name.AsObservable()).SyncTo(result, false);
            result.Count.Should().Be(0);
            vms.Add(new SelectObservableCollectionViewModel("test1"));
            result.Count.Should().Be(1);
            vms.RemoveAt(0);
            result.Count.Should().Be(0);
        }

        [TestMethod]
        public void SyncTo_RemoveThenChange()
        {
            var vms = new ObservableCollection<SelectObservableCollectionViewModel>();
            var result = new List<string>();
            vms.ToLiveLinq().Select(vm => vm.Name.AsObservable()).SyncTo(result, false);
            result.Count.Should().Be(0);
            var tmp = new SelectObservableCollectionViewModel("test1");
            vms.Add(tmp);
            result.Count.Should().Be(1);
            vms.RemoveAt(0);
            result.Count.Should().Be(0);
            tmp.Name.OnNext("test2");
            result.Count.Should().Be(0);
        }
    }
}
