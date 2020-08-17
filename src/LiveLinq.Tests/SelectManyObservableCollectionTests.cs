using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using FluentAssertions;

using LiveLinq.List;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoreCollections;


namespace LiveLinq.Tests
{
    [TestClass]
    public class SelectManyObservableCollectionTests
    {
        public SelectManyObservableCollectionTests()
        {
            //if (SynchronizationContext.Current == null)
            //    SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }

        [TestMethod]
        public void DisposeSubscription_ThenRemoveSubItems()
        {
            var source = new ObservableList<ObservableList<string>>();
            ImmutableList<string> result = ImmutableList<string>.Empty;
            var subscription = source.ToLiveLinq()
                .SelectMany((list, _) => list.ToLiveLinq())
                .ToObservableEnumerable().Subscribe(value => result = value);
            source.Add(new ObservableList<string>());
            source.Add(new ObservableList<string>());
            source[0].Add("a");
            source[1].Add("c");
            subscription.Dispose();
            source[0].Add("b");
            source[1].Add("d");
            result.Should().ContainInOrder("a", "c");
        }

        [TestMethod]
        public void DisposeSubscription_ThenRemoveItem()
        {
            var source = new ObservableList<ObservableList<string>>();
            ImmutableList<string> result = ImmutableList<string>.Empty;
            var subscription = source.ToLiveLinq()
                .SelectMany((list, _) => list.ToLiveLinq())
                .ToObservableEnumerable().Subscribe(value => result = value);
            source.Add(new ObservableList<string>());
            source.Add(new ObservableList<string>());
            source[0].Add("a");
            source[1].Add("c");
            subscription.Dispose();
            source.RemoveAt(1);
            result.Should().ContainInOrder("a", "c");
        }

        [TestMethod]
        public void AddItemAfterSubscribing()
        {
            var source = new ObservableList<ObservableList<string>>();
            ImmutableList<string> result = ImmutableList<string>.Empty;
            source.ToLiveLinq()
                .SelectMany((list, _) => list.ToLiveLinq())
                .ToObservableEnumerable().Subscribe(value => result = value);
            source.Add(new ObservableList<string>());
            source.Add(new ObservableList<string>());
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void RemoveItemAfterSubscribing()
        {
            var source = new ObservableList<ObservableList<string>>();
            ImmutableList<string> result = ImmutableList<string>.Empty;
            source.ToLiveLinq()
                .SelectMany((list, _) => list.ToLiveLinq())
                .ToObservableEnumerable().Subscribe(value => result = value);
            source.Add(new ObservableList<string>());
            source.RemoveAt(0);
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void AddItemAfterSubscribing_AddSubItemsAfterSubscribing()
        {
            var source = new ObservableList<ObservableList<string>>();
            ImmutableList<string> result = ImmutableList<string>.Empty;
            source.ToLiveLinq()
                .SelectMany((list, _) => list.ToLiveLinq())
                .ToObservableEnumerable().Subscribe(value => result = value);
            source.Add(new ObservableList<string>());
            source.Add(new ObservableList<string>());
            source[0].Add("a");
            source[0].Add("b");
            source[1].Add("c");
            source[1].Add("d");
            result.Should().ContainInOrder("a", "b", "c", "d");
        }

        [TestMethod]
        public void AddItemAfterSubscribing_RemoveSubItemsAfterSubscribing()
        {
            var source = new ObservableList<ObservableList<string>>();
            ImmutableList<string> result = ImmutableList<string>.Empty;
            source.ToLiveLinq()
                .SelectMany((list, _) => list.ToLiveLinq())
                .ToObservableEnumerable().Subscribe(value => result = value);
            source.Add(new ObservableList<string>());
            source.Add(new ObservableList<string>());
            source[0].Add("a");
            source[0].RemoveAt(0);
            source[1].Add("c");
            source[1].RemoveAt(0);
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void RemoveItemAfterSubscribing_AddSubItemsAfterSubscribing()
        {
            var source = new ObservableList<ObservableList<string>>();
            ImmutableList<string> result = ImmutableList<string>.Empty;
            source.ToLiveLinq()
                .SelectMany((list, _) => list.ToLiveLinq())
                .ToObservableEnumerable().Subscribe(value => result = value);
            source.Add(new ObservableList<string>());
            source.Add(new ObservableList<string>());
            var tmp = source[0];
            source.RemoveAt(0);
            tmp.Add("a");
            tmp.Add("b");
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void AddItemBeforeSubscribing_AddSubItemsAfterSubscribing()
        {
            var source = new ObservableList<ObservableList<string>>();
            ImmutableList<string> result = ImmutableList<string>.Empty;
            source.Add(new ObservableList<string>());
            source.Add(new ObservableList<string>());
            source.ToLiveLinq()
                .SelectMany((list, _) => list.ToLiveLinq())
                .ToObservableEnumerable().Subscribe(value => result = value);
            source[0].Add("a");
            source[0].Add("b");
            source[1].Add("c");
            source[1].Add("d");
            result.Should().ContainInOrder("a", "b", "c", "d");
        }

        [TestMethod]
        public void AddItemBeforeSubscribing_AddSubItemsBeforeSubscribing()
        {
            var source = new ObservableList<ObservableList<string>>();
            ImmutableList<string> result = ImmutableList<string>.Empty;
            source.Add(new ObservableList<string>());
            source.Add(new ObservableList<string>());
            source[0].Add("a");
            source[0].Add("b");
            source[1].Add("c");
            source[1].Add("d");
            source.ToLiveLinq()
                .SelectMany((list, _) => list.ToLiveLinq())
                .ToObservableEnumerable().Subscribe(value => result = value);
            result.Should().ContainInOrder("a", "b", "c", "d");
        }

        [TestMethod]
        public void AddItemAfterSubscribing_AddSubItemsAfterSubscribing_RemoveSubItemsAfterSubscribing()
        {
            var source = new ObservableList<ObservableList<string>>();
            ImmutableList<string> result = ImmutableList<string>.Empty;
            source.ToLiveLinq()
                .SelectMany((list, _) => list.ToLiveLinq())
                .ToObservableEnumerable().Subscribe(value => result = value);
            source.Add(new ObservableList<string>());
            source.Add(new ObservableList<string>());
            source[0].Add("a");
            source[0].Add("b");
            source[1].Add("c");
            source[1].Add("d");
            source[0].Clear();
            source[1].Clear();
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void AddItemBeforeSubscribing_AddSubItemsAfterSubscribing_RemoveSubItemsAfterSubscribing()
        {
            var source = new ObservableList<ObservableList<string>>();
            ImmutableList<string> result = ImmutableList<string>.Empty;
            source.Add(new ObservableList<string>());
            source.Add(new ObservableList<string>());
            source.ToLiveLinq()
                .SelectMany((list, _) => list.ToLiveLinq())
                .ToObservableEnumerable().Subscribe(value => result = value);
            source[0].Add("a");
            source[0].Add("b");
            source[1].Add("c");
            source[1].Add("d");
            source[0].Clear();
            source[1].Clear();
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void AddItemBeforeSubscribing_AddSubItemsBeforeSubscribing_RemoveSubItemsBeforeSubscribing()
        {
            var source = new ObservableList<ObservableList<string>>();
            ImmutableList<string> result = ImmutableList<string>.Empty;
            source.Add(new ObservableList<string>());
            source.Add(new ObservableList<string>());
            source[0].Add("a");
            source[0].Add("b");
            source[1].Add("c");
            source[1].Add("d");
            source[0].Clear();
            source[1].Clear();
            source.ToLiveLinq()
                .SelectMany((list, _) => list.ToLiveLinq())
                .ToObservableEnumerable().Subscribe(value => result = value);
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void AddItemBeforeSubscribing_AddSubItemsBeforeSubscribing_RemoveSubItemsAfterSubscribing()
        {
            var source = new ObservableList<ObservableList<string>>();
            ImmutableList<string> result = ImmutableList<string>.Empty;
            source.Add(new ObservableList<string>());
            source.Add(new ObservableList<string>());
            source[0].Add("a");
            source[0].Add("b");
            source[1].Add("c");
            source[1].Add("d");
            source.ToLiveLinq()
                .SelectMany((list, _) => list.ToLiveLinq())
                .ToObservableEnumerable().Subscribe(value => result = value);
            source[0].Clear();
            source[1].Clear();
            result.Should().BeEmpty();
        }
    }
}
