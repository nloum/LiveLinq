using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using FluentAssertions;
using LiveLinq.List;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoreCollections;

namespace LiveLinq.Tests.List
{
    public partial class ExtensionsTests
    {
        static ExtensionsTests()
        {
            if (SynchronizationContext.Current == null)
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }

        #region Beginning a LiveLinq method chain

        [TestMethod]
        public void ObservableCollection_LiveLinq_Add()
        {
            var source = new ObservableCollection<string>();
            using (var result = source.ToLiveLinq().ToReadOnlyObservableList())
            {
                source.Add("A");
                result.Should().BeEquivalentTo("A");
            }
        }

        #endregion

        #region GroupBy tests

        [TestMethod]
        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public void GroupBy_SelectMany()
        {
            var source = new ObservableCollection<int>();
            using (var result = source.ToLiveLinq().GroupBy(i => i % 2 == 0).SelectMany((_, a) => a).ToReadOnlyObservableSet())
            {
                source.Add(1);
                source.Add(2);
                result.Should().ContainInOrder(1, 2);
            }
        }

        [TestMethod]
        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public void GroupBy_InitialState()
        {
            var source = new ObservableCollection<int>();
            source.Add(1);
            source.Add(2);
            var result = source.ToLiveLinq().GroupBy(i => i%2 == 0);
            var snapshot = result.ContainsKey(true).ToBehaviorSubject();
            Thread.Sleep(300);
            snapshot.Value.Should().BeTrue();
        }

        //[TestMethod]
        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public void GroupBy_SubsequentChanges()
        {
            var source = new ObservableCollection<int>();
            var result = source.ToLiveLinq().GroupBy(i => i % 2 == 0);
            var snapshot = result.ContainsKey(true).ToBehaviorSubject();
            snapshot.Value.Should().BeFalse();
            source.Add(1);
            snapshot.Value.Should().BeFalse();
            source.Add(2);
            Thread.Sleep(300);
            snapshot.Value.Should().BeTrue();
            source.RemoveAt(0);
            source.RemoveAt(0);
            snapshot.Value.Should().BeFalse();
        }

        //[TestMethod]
        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public void GroupBy_SubsequentClear()
        {
            var source = new ObservableList<int>();
            var result = source.ToLiveLinq().GroupBy(i => i % 2 == 0);
            var snapshot = result.ContainsKey(true).ToBehaviorSubject();
            snapshot.Value.Should().BeFalse();
            source.Add(1);
            snapshot.Value.Should().BeFalse();
            source.Add(2);
            Thread.Sleep(300);
            snapshot.Value.Should().BeTrue();
            source.Clear();
            snapshot.Value.Should().BeFalse();
        }

        //[TestMethod]
        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public void GroupBy_ObservableKey()
        {
            var source = new ObservableList<BehaviorSubject<int>>();
            var result = source.ToLiveLinq().Select(rp => rp.AsObservable()).GroupBy(i => i % 2 == 0);
            var snapshot = result.ContainsKey(true).ToBehaviorSubject();
            snapshot.Value.Should().BeFalse();
            source.Add(new BehaviorSubject<int>(1));
            snapshot.Value.Should().BeFalse();
            source.Add(new BehaviorSubject<int>(1));
            Thread.Sleep(300);
            snapshot.Value.Should().BeFalse();
            source[1].OnNext(2);
            snapshot.Value.Should().BeTrue();
            source[1].OnNext(1);
            snapshot.Value.Should().BeFalse();
            var tmp = source[1];
            source.RemoveAt(1);
            tmp.OnNext(2);
            snapshot.Value.Should().BeFalse();
        }

        //[TestMethod]
        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public void GroupBy_GetByKey()
        {
            var source = new ObservableCollection<int>();
            var result = source.ToLiveLinq().GroupBy(i => i % 2 == 0);
            using (var evenNumbers = result.Get(true).ToReadOnlyObservableSet())
            using (var nonGroupedEvenNumbers = source.ToLiveLinq().Where(i => i % 2 == 0).ToReadOnlyObservableList())
            {
                nonGroupedEvenNumbers.Count.Should().Be(0);
                evenNumbers.Count.Should().Be(0);
                source.Add(1);
                nonGroupedEvenNumbers.Count.Should().Be(0);
                evenNumbers.Count.Should().Be(0);
                source.Add(2);
                nonGroupedEvenNumbers.Count.Should().Be(1);
                evenNumbers.Count.Should().Be(1);
                source.Add(2);
                nonGroupedEvenNumbers.Count.Should().Be(2);
                evenNumbers.Count.Should().Be(2);
            }
        }

        #endregion

        #region Sort tests

        [TestMethod]
        public void OrderBy_InitialState_ShouldBeSorted_InAscendingingOrder()
        {
            var source = new ObservableCollection<int>();
            source.Add(1);
            source.Add(2);
            source.Add(1);
            source.Add(0);

            using (var sortedResult = source.ToLiveLinq().OrderBy(i => i).ToReadOnlyObservableList())
            {
                sortedResult.ToArray().Should().BeInAscendingOrder();
            }
        }

        [TestMethod]
        public void OrderBy_InitialState_ShouldBeSorted_InDescendingingOrder()
        {
            var source = new ObservableCollection<int>();
            source.Add(1);
            source.Add(2);
            source.Add(1);
            source.Add(0);

            using (var sortedResult = source.ToLiveLinq().OrderByDescending(i => i).ToReadOnlyObservableList())
            {
                sortedResult.ToArray().Should().BeInDescendingOrder();
            }
        }

        [TestMethod]
        public void OrderBy_ShouldBeSorted_InAscendingingOrder()
        {
            var source = new ObservableCollection<int>();
            source.Add(1);
            source.Add(2);

            using (var sortedResult = source.ToLiveLinq().OrderBy(i => i).ToReadOnlyObservableList())
            {
                source.Add(1);
                source.Add(0);
                sortedResult.ToArray().Should().BeInAscendingOrder();
            }
        }

        [TestMethod]
        public void OrderBy_ShouldBeSorted_InDescendingingOrder()
        {
            var source = new ObservableCollection<int>();
            source.Add(1);
            source.Add(2);

            using (var sortedResult = source.ToLiveLinq().OrderByDescending(i => i).ToReadOnlyObservableList())
            {
                source.Add(1);
                source.Add(0);
                sortedResult.ToArray().Should().BeInDescendingOrder();
            }
        }

        [TestMethod]
        public void OrderByThenBy_InitialState_ShouldBeSorted_InAscendingingOrder()
        {
            var source = new ObservableCollection<int>();
            source.Add(1);
            source.Add(2);
            source.Add(1);
            source.Add(0);

            using (var sortedResult = source.ToLiveLinq().OrderBy(i => i % 2 == 0).ThenBy(i => i).ToReadOnlyObservableList())
            {
                sortedResult.ToArray().Should().ContainInOrder(1, 1, 0, 2);
            }
        }

        [TestMethod]
        public void OrderByThenBy_InitialState_ShouldBeSorted_InDescendingingOrder()
        {
            var source = new ObservableCollection<int>();
            source.Add(1);
            source.Add(2);
            source.Add(1);
            source.Add(0);

            using (var sortedResult = source.ToLiveLinq().OrderBy(i => i % 2 == 0).ThenByDescending(i => i).ToReadOnlyObservableList())
            {
                sortedResult.ToArray().Should().ContainInOrder(1, 1, 2, 0);
            }
        }

        [TestMethod]
        public void OrderByThenBy_ShouldBeSorted_InAscendingingOrder()
        {
            var source = new ObservableCollection<int>();
            source.Add(1);
            source.Add(2);

            using (var sortedResult = source.ToLiveLinq().OrderBy(i => i % 2 == 0).ThenBy(i => i).ToReadOnlyObservableList())
            {
                source.Add(1);
                source.Add(0);
                sortedResult.ToArray().Should().ContainInOrder(1, 1, 0, 2);
            }
        }

        [TestMethod]
        public void OrderByThenBy_ShouldBeSorted_InDescendingingOrder()
        {
            var source = new ObservableCollection<int>();
            source.Add(1);
            source.Add(2);

            using (var sortedResult = source.ToLiveLinq().OrderBy(i => i % 2 == 0).ThenByDescending(i => i).ToReadOnlyObservableList())
            {
                source.Add(1);
                source.Add(0);
                sortedResult.ToArray().Should().ContainInOrder(1, 1, 2, 0);
            }
        }

        #endregion

        #region Cast and TypeOf tests

        [TestMethod]
        public void Cast_ShouldWork()
        {
            var source = new ObservableCollection<object>();
            source.Add("Hi");
            source.Add("Howdy");
            var result = source.ToLiveLinq().Cast<string>().ToReadOnlyObservableList();
            result.Should().BeEquivalentTo("Hi", "Howdy");
        }

        [TestMethod]
        public void Cast_ShouldFail()
        {
            var source = new ObservableCollection<object>();
            source.Add("Hi");
            source.Add("Howdy");
            source.Add(3);
            Action action = () => source.ToLiveLinq().Cast<string>().ToReadOnlyObservableList();
            action.Should().Throw<Exception>("Because an attempt was made to cast an int to a string");
        }

        [TestMethod]
        public void TypeOf_ShouldWork()
        {
            var source = new ObservableCollection<object>();
            source.Add("Hi");
            source.Add("Howdy");
            source.Add(3);
            var result = source.ToLiveLinq().OfType<string>().ToReadOnlyObservableList();
            result.Should().BeEquivalentTo("Hi", "Howdy");
        }

        #endregion

        #region Except ObservableCollection tests

        [TestMethod]
        public void InitialExceptObservableCollection_ShouldSyncWithClearSource1()
        {
            var source1 = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var source2 = new ObservableCollection<int>(new[] { 2 });
            using (var synced = source1.ToLiveLinq().Except(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                synced.Should().BeEquivalentTo(1, 3);
                source1.Clear();
                synced.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void InitialExceptObservableCollection_ShouldSyncWithClearSource2()
        {
            var source1 = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var source2 = new ObservableCollection<int>(new[] { 2 });
            using (var synced = source1.ToLiveLinq().Except(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                synced.Should().BeEquivalentTo(1, 3);
                source2.Clear();
                synced.Should().BeEquivalentTo(1, 2, 3);
            }
        }

        [TestMethod]
        public void InitialExceptObservableCollection_ShouldSyncWithReplaceSource1()
        {
            var source1 = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var source2 = new ObservableCollection<int>(new[] { 2 });
            using (var synced = source1.ToLiveLinq().Except(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                synced.Should().BeEquivalentTo(1, 3);
                source1[1] = 4;
                synced.Should().BeEquivalentTo(1, 4, 3);
            }
        }

        [TestMethod]
        public void InitialExceptObservableCollection_ShouldSyncWithReplaceSource2()
        {
            var source1 = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var source2 = new ObservableCollection<int>(new[] { 2 });
            using (var synced = source1.ToLiveLinq().Except(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                synced.Should().BeEquivalentTo(1, 3);
                source2[0] = 1;
                synced.Should().BeEquivalentTo(2, 3);
            }
        }

        [TestMethod]
        public void InitialExceptObservableCollection_ShouldSyncWithAddSource1()
        {
            var source1 = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var source2 = new ObservableCollection<int>(new[] { 2 });
            using (var synced = source1.ToLiveLinq().Except(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                synced.Should().BeEquivalentTo(1, 3);
                source1.Add(4);
                synced.Should().BeEquivalentTo(1, 3, 4);
            }
        }

        [TestMethod]
        public void InitialExceptObservableCollection_ShouldSyncWithAddSource2()
        {
            var source1 = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var source2 = new ObservableCollection<int>(new[] { 2 });
            using (var synced = source1.ToLiveLinq().Except(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                synced.Should().BeEquivalentTo(1, 3);
                source2.Add(1);
                synced.Should().BeEquivalentTo(3);
            }
        }

        [TestMethod]
        public void InitialExceptObservableCollection_ShouldSyncWithRemoveSource1()
        {
            var source1 = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var source2 = new ObservableCollection<int>(new[] { 2 });
            using (var synced = source1.ToLiveLinq().Except(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                synced.Should().BeEquivalentTo(1, 3);
                source1.RemoveAt(0);
                synced.Should().BeEquivalentTo(3);
            }
        }

        [TestMethod]
        public void InitialExceptObservableCollection_ShouldSyncWithRemoveSource2()
        {
            var source1 = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var source2 = new ObservableCollection<int>(new[] { 2 });
            using (var synced = source1.ToLiveLinq().Except(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                synced.Should().BeEquivalentTo(1, 3);
                source2.RemoveAt(0);
                synced.Should().BeEquivalentTo(1, 2, 3);
            }
        }

        [TestMethod]
        public void InitialExceptObservableCollection_ShouldNotSyncAfterDisposal()
        {
            var source1 = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var source2 = new ObservableCollection<int>(new[] { 2 });
            var synced = source1.ToLiveLinq().Except(source2.ToLiveLinq()).ToReadOnlyObservableList();
            synced.Dispose();
            source1.Clear();
            synced.Should().BeEquivalentTo(1, 3);
        }

        [TestMethod]
        public void InitialExceptObservableCollection_ShouldNotStopAtOneIntersection()
        {
            var source1 = new ObservableCollection<int>(new[] { 1, 2, 2, 3 });
            var source2 = new ObservableCollection<int>(new[] { 2 });
            var synced = source1.ToLiveLinq().Except(source2.ToLiveLinq()).ToReadOnlyObservableList();
            synced.Dispose();
            source1.Clear();
            synced.Should().BeEquivalentTo(1, 3);
        }

        [TestMethod]
        public void InitialExceptObservableCollection_ShouldIgnoreNonexistentExceptions()
        {
            var source1 = new ObservableCollection<string>(new[] { "test", string.Empty });
            var source2 = new ObservableCollection<string>(new[] { "test" });
            var synced = source1.ToLiveLinq().Except(source2.ToLiveLinq()).ToReadOnlyObservableList();
            synced.Should().BeEquivalentTo(string.Empty);
            synced.Dispose();
            source1.Clear();
            synced.Should().BeEquivalentTo(string.Empty);
        }

        [TestMethod]
        public void NonInitialExceptObservableCollection_ShouldSyncWithClearSource1()
        {
            var source1 = new ObservableCollection<int>();
            var source2 = new ObservableCollection<int>();
            using (var synced = source1.ToLiveLinq().Except(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                source1.AddRange(1, 2, 3);
                synced.Should().BeEquivalentTo(1, 2, 3);
                source2.Add(2);
                synced.Should().BeEquivalentTo(1, 3);
                source1.Clear();
                synced.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void NonInitialExceptObservableCollection_ShouldSyncWithClearSource2()
        {
            var source1 = new ObservableCollection<int>();
            var source2 = new ObservableCollection<int>();
            using (var synced = source1.ToLiveLinq().Except(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                source1.AddRange(1, 2, 3);
                synced.Should().BeEquivalentTo(1, 2, 3);
                source2.Add(2);
                synced.Should().BeEquivalentTo(1, 3);
                source2.Clear();
                synced.Should().BeEquivalentTo(1, 2, 3);
            }
        }

        [TestMethod]
        public void NonInitialExceptObservableCollection_ShouldSyncWithReplaceSource1()
        {
            var source1 = new ObservableCollection<int>();
            var source2 = new ObservableCollection<int>();
            using (var synced = source1.ToLiveLinq().Except(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                source1.AddRange(1, 2, 3);
                synced.Should().BeEquivalentTo(1, 2, 3);
                source2.Add(2);
                synced.Should().BeEquivalentTo(1, 3);
                source1[1] = 4;
                synced.Should().BeEquivalentTo(1, 4, 3);
            }
        }

        [TestMethod]
        public void NonInitialExceptObservableCollection_ShouldSyncWithReplaceSource2()
        {
            var source1 = new ObservableCollection<int>();
            var source2 = new ObservableCollection<int>();
            using (var synced = source1.ToLiveLinq().Except(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                source1.AddRange(1, 2, 3);
                synced.Should().BeEquivalentTo(1, 2, 3);
                source2.Add(2);
                synced.Should().BeEquivalentTo(1, 3);
                source2[0] = 1;
                synced.Should().BeEquivalentTo(2, 3);
            }
        }

        [TestMethod]
        public void NonInitialExceptObservableCollection_ShouldSyncWithAddSource1()
        {
            var source1 = new ObservableCollection<int>();
            var source2 = new ObservableCollection<int>();
            using (var synced = source1.ToLiveLinq().Except(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                source1.AddRange(1, 2, 3);
                synced.Should().BeEquivalentTo(1, 2, 3);
                source2.Add(2);
                synced.Should().BeEquivalentTo(1, 3);
                source1.Add(4);
                synced.Should().BeEquivalentTo(1, 3, 4);
            }
        }

        [TestMethod]
        public void NonInitialExceptObservableCollection_ShouldSyncWithAddSource2()
        {
            var source1 = new ObservableCollection<int>();
            var source2 = new ObservableCollection<int>();
            using (var synced = source1.ToLiveLinq().Except(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                source1.AddRange(1, 2, 3);
                synced.Should().BeEquivalentTo(1, 2, 3);
                source2.Add(2);
                synced.Should().BeEquivalentTo(1, 3);
                source2.Add(1);
                synced.Should().BeEquivalentTo(3);
            }
        }

        [TestMethod]
        public void NonInitialExceptObservableCollection_ShouldSyncWithRemoveSource1()
        {
            var source1 = new ObservableCollection<int>();
            var source2 = new ObservableCollection<int>();
            using (var synced = source1.ToLiveLinq().Except(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                source1.AddRange(1, 2, 3);
                synced.Should().BeEquivalentTo(1, 2, 3);
                source2.Add(2);
                synced.Should().BeEquivalentTo(1, 3);
                source1.RemoveAt(0);
                synced.Should().BeEquivalentTo(3);
            }
        }

        [TestMethod]
        public void NonInitialExceptObservableCollection_ShouldSyncWithRemoveSource2()
        {
            var source1 = new ObservableCollection<int>();
            var source2 = new ObservableCollection<int>();
            using (var synced = source1.ToLiveLinq().Except(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                source1.AddRange(1, 2, 3);
                synced.Should().BeEquivalentTo(1, 2, 3);
                source2.Add(2);
                synced.Should().BeEquivalentTo(1, 3);
                source2.RemoveAt(0);
                synced.Should().BeEquivalentTo(1, 2, 3);
            }
        }

        [TestMethod]
        public void NonInitialExceptObservableCollection_ShouldNotSyncAfterDisposal()
        {
            var source1 = new ObservableCollection<int>();
            var source2 = new ObservableCollection<int>();
            var synced = source1.ToLiveLinq().Except(source2.ToLiveLinq()).ToReadOnlyObservableList();
            source1.AddRange(1, 2, 3);
            synced.Should().BeEquivalentTo(1, 2, 3);
            source2.Add(2);
            synced.Should().BeEquivalentTo(1, 3);
            synced.Dispose();
        }

        [TestMethod]
        public void NonInitialExceptObservableCollection_ShouldNotStopAtOneIntersection()
        {
            var source1 = new ObservableCollection<int>();
            var source2 = new ObservableCollection<int>();
            var synced = source1.ToLiveLinq().Except(source2.ToLiveLinq()).ToReadOnlyObservableList();
            source1.AddRange(1, 2, 2, 3);
            synced.Should().BeEquivalentTo(1, 2, 2, 3);
            source2.Add(2);
            synced.Should().BeEquivalentTo(1, 3);
            synced.Dispose();
        }

        [TestMethod]
        public void NonInitialExceptObservableCollection_ShouldIgnoreNonexistentExceptions()
        {
            var source1 = new ObservableCollection<string>();
            var source2 = new ObservableCollection<string>();
            var synced = source1.ToLiveLinq().Except(source2.ToLiveLinq()).ToReadOnlyObservableList();
            source1.AddRange("test", string.Empty);
            synced.Should().BeEquivalentTo("test", string.Empty);
            source2.Add("test");
            synced.Should().BeEquivalentTo(string.Empty);
            synced.Dispose();
        }

        #endregion

        #region Concat ObservableCollection tests

        [TestMethod]
        public void ConcatObservableList_ShouldSyncWithClearSource1()
        {
            var source1 = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var source2 = new ObservableCollection<int>(new[] { 4 });
            using (var synced = source1.ToLiveLinq().Concat(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                synced.Should().BeEquivalentTo(1, 2, 3, 4);
                source1.Clear();
                synced.Should().BeEquivalentTo(4);
            }
        }

        [TestMethod]
        public void ConcatObservableList_ShouldSyncWithClearSource2()
        {
            var source1 = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var source2 = new ObservableCollection<int>(new[] { 4 });
            using (var synced = source1.ToLiveLinq().Concat(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                synced.Should().BeEquivalentTo(1, 2, 3, 4);
                source2.Clear();
                synced.Should().BeEquivalentTo(1, 2, 3);
            }
        }

        [TestMethod]
        public void ConcatObservableList_ShouldSyncWithAddSource1()
        {
            var source1 = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var source2 = new ObservableCollection<int>(new[] { 4 });
            using (var synced = source1.ToLiveLinq().Concat(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                synced.Should().BeEquivalentTo(1, 2, 3, 4);
                source1.Add(5);
                synced.Should().BeEquivalentTo(1, 2, 3, 4, 5);
            }
        }

        [TestMethod]
        public void ConcatObservableList_ShouldSyncWithAddSource2()
        {
            var source1 = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var source2 = new ObservableCollection<int>(new[] { 4 });
            using (var synced = source1.ToLiveLinq().Concat(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                synced.Should().BeEquivalentTo(1, 2, 3, 4);
                source2.Add(5);
                synced.Should().BeEquivalentTo(1, 2, 3, 4, 5);
            }
        }

        [TestMethod]
        public void ConcatObservableList_ShouldSyncWithRemoveSource1()
        {
            var source1 = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var source2 = new ObservableCollection<int>(new[] { 4 });
            using (var synced = source1.ToLiveLinq().Concat(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                synced.Should().BeEquivalentTo(1, 2, 3, 4);
                source1.RemoveAt(0);
                synced.Should().BeEquivalentTo(2, 3, 4);
            }
        }

        [TestMethod]
        public void ConcatObservableList_ShouldSyncWithRemoveSource2()
        {
            var source1 = new ObservableCollection<int>(new[] { 1, 3 });
            var source2 = new ObservableCollection<int>(new[] { 2, 4 });
            using (var synced = source1.ToLiveLinq().Concat(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                synced.Should().BeEquivalentTo(1, 2, 3, 4);
                source2.RemoveAt(1);
                synced.Should().BeEquivalentTo(1, 2, 3);
            }
        }

        [TestMethod]
        public void ConcatObservableList_ShouldHaveDuplicates()
        {
            var source1 = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var source2 = new ObservableCollection<int>(new[] { 2, 4 });
            using (var synced = source1.ToLiveLinq().Concat(source2.ToLiveLinq()).ToReadOnlyObservableList())
            {
                synced.Should().BeEquivalentTo(1, 2, 3, 2, 4);
            }
        }

        [TestMethod]
        public void ConcatObservableList_ShouldNotSyncAfterDisposal()
        {
            var source1 = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var source2 = new ObservableCollection<int>(new[] { 4 });
            var synced = source1.ToLiveLinq().Concat(source2.ToLiveLinq()).ToReadOnlyObservableList();
            synced.Dispose();
            source1.Clear();
            synced.Should().BeEquivalentTo(1, 2, 3, 4);
        }

        #endregion

        #region Where ObservableCollection tests

        [TestMethod]
        public void WhereObservableCollection_ShouldSyncPropertyWithClear()
        {
            var source = new ObservableCollection<int>(new[] { 1, 2, 3 });
            using (var synced = source.ToLiveLinq().Where(i => i % 2 == 0).ToReadOnlyObservableList())
            {
                synced.Should().BeEquivalentTo(2);
                source.Clear();
                synced.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void WhereObservableCollection_ShouldSyncInitialState()
        {
            var source = new ObservableCollection<int>(new[] { 1, 2, 3 });
            using (var synced = source.ToLiveLinq().Where(i => i % 2 == 0).ToReadOnlyObservableList())
            {
                synced.Should().BeEquivalentTo(2);
            }
        }

        [TestMethod]
        public void WhereObservableCollection_ShouldSyncPriorToDisposal()
        {
            var source = new ObservableCollection<int>();
            using (var synced = source.ToLiveLinq().Where(i => i % 2 == 0).ToReadOnlyObservableList())
            {
                source.Add(4);
                source.Add(5);
                synced.Should().BeEquivalentTo(4);
            }
        }

        [TestMethod]
        public void WhereObservableCollection_ShouldNotSyncAfterDisposal()
        {
            var source = new ObservableCollection<int>();
            var synced = source.ToLiveLinq().Where(i => i % 2 == 0).ToReadOnlyObservableList();
            synced.Dispose();
            source.Add(4);
            source.Add(5);
            synced.Should().BeEmpty();
        }

        #endregion

        #region Select ObservableCollection tests

        [TestMethod]
        public void SelectObservableCollection_ShouldSupportUpdateValueAfterPreviousItemRemoved()
        {
            var source = new ObservableCollection<BehaviorSubject<int>>(new[] { new BehaviorSubject<int>(1), new BehaviorSubject<int>(2), new BehaviorSubject<int>(3) });
            using (var synced = source.ToLiveLinq().Select(i => i.AsObservable()).ToReadOnlyObservableList())
            {
                synced.Should().ContainInOrder(1, 2, 3);
                source.RemoveAt(0);
                source[1].OnNext(4);
                synced.Should().ContainInOrder(2, 4);
            }
        }

        [TestMethod]
        public void SelectObservableCollection_ShouldSupportUpdateValueAfterPreviousItemInserted()
        {
            var source = new ObservableCollection<BehaviorSubject<int>>(new[] { new BehaviorSubject<int>(1), new BehaviorSubject<int>(2), new BehaviorSubject<int>(3) });
            using (var synced = source.ToLiveLinq().Select(i => i.AsObservable()).ToReadOnlyObservableList())
            {
                synced.Should().ContainInOrder(1, 2, 3);
                source.Insert(0, new BehaviorSubject<int>(-1));
                source[3].OnNext(4);
                synced.Should().ContainInOrder(-1, 1, 2, 4);
            }
        }

        [TestMethod]
        public void SelectObservableCollection_ShouldSyncPropertyWithClear()
        {
            var source = new ObservableCollection<int>(new[] { 1, 2, 3 });
            using (var synced = source.ToLiveLinq().Select(i => (ulong)i).ToReadOnlyObservableList())
            {
                synced.Should().ContainInOrder((ulong)1, (ulong)2, (ulong)3);
                source.Clear();
                synced.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void SelectObservableCollection_ShouldSyncInitialState()
        {
            var source = new ObservableCollection<int>(new[] { 1, 2, 3 });
            using (var synced = source.ToLiveLinq().Select(i => (ulong)i).ToReadOnlyObservableList())
            {
                synced.Should().ContainInOrder((ulong)1, (ulong)2, (ulong)3);
            }
        }

        [TestMethod]
        public void SelectObservableCollection_ShouldSyncPriorToDisposal()
        {
            var source = new ObservableCollection<int>();
            using (var synced = source.ToLiveLinq().Select(i => (ulong)i).ToReadOnlyObservableList())
            {
                source.Add(4);
                synced.Should().ContainInOrder((ulong)4);
            }
        }

        [TestMethod]
        public void SelectObservableCollection_ShouldNotSyncAfterDisposal()
        {
            var source = new ObservableCollection<int>();
            var synced = source.ToLiveLinq().Select(i => (ulong)i).ToReadOnlyObservableList();
            synced.Dispose();
            source.Add(4);
            synced.Should().BeEmpty();
        }

        [TestMethod]
        public void SelectObservableCollection_ShouldSupportAdd()
        {
            var source = new ObservableCollection<int>();
            using (var synced = source.ToLiveLinq().Select(i => (ulong)i).ToReadOnlyObservableList())
            {
                source.Add(3);
                source.Add(4);
                synced.Should().ContainInOrder((ulong)3, (ulong)4);
            }
        }

        [TestMethod]
        public void SelectObservableCollection_ShouldSupportClear()
        {
            var source = new ObservableCollection<int>();
            using (var synced = source.ToLiveLinq().Select(i => (ulong)i).ToReadOnlyObservableList())
            {
                source.Add(3);
                source.Add(4);
                synced.Should().ContainInOrder((ulong)3, (ulong)4);
                source.Clear();
                synced.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void SelectObservableCollection_ShouldSupportMove()
        {
            var source = new ObservableCollection<int>();
            using (var synced = source.ToLiveLinq().Select(i => (ulong)i).ToReadOnlyObservableList())
            {
                source.Insert(0, 3);
                source.Insert(1, 4);
                synced.Should().ContainInOrder((ulong)3, (ulong)4);
                source.Move(0, 1);
                synced.Should().ContainInOrder((ulong)4, (ulong)3);
            }
        }

        [TestMethod]
        public void SelectObservableCollection_ShouldSupportInsert()
        {
            var source = new ObservableCollection<int>();
            using (var synced = source.ToLiveLinq().Select(i => (ulong)i).ToReadOnlyObservableList())
            {
                source.Insert(0, 3);
                source.Insert(1, 4);
                source.Insert(0, 2);
                synced.Should().ContainInOrder((ulong)2, (ulong)3, (ulong)4);
            }
        }

        [TestMethod]
        public void SelectObservableCollection_ShouldSupportRemove()
        {
            var source = new ObservableCollection<int>();
            using (var synced = source.ToLiveLinq().Select(i => (ulong)i).ToReadOnlyObservableList())
            {
                source.Insert(0, 2);
                source.Insert(1, 3);
                source.Insert(2, 4);
                synced.Should().ContainInOrder((ulong)2, (ulong)3, (ulong)4);
                source.Remove(3);
                synced.Should().ContainInOrder((ulong)2, (ulong)4);
            }
        }

        [TestMethod]
        public void SelectObservableCollection_ShouldSupportRemoveAt()
        {
            var source = new ObservableCollection<int>();
            using (var synced = source.ToLiveLinq().Select(i => (ulong)i).ToReadOnlyObservableList())
            {
                source.Insert(0, 3);
                source.Insert(1, 4);
                source.Insert(2, 5);
                synced.Should().ContainInOrder((ulong)3, (ulong)4, (ulong)5);
                source.RemoveAt(1);
                synced.Should().ContainInOrder((ulong)3, (ulong)5);
            }
        }

        private class CustomObservableCollection : ObservableCollection<int>
        {
            public void PublicOnCollectionChanged(NotifyCollectionChangedEventArgs args)
            {
                OnCollectionChanged(args);
            }
        }

        [TestMethod]
        public void Select_ShouldNotAcceptNoIndexOnAdd()
        {
            var item = 3;
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[] { item }, -1);
            var source = new CustomObservableCollection();
            Action action = () =>
                {
                    using (var result = source.ToLiveLinq().Select(i => i).ToReadOnlyObservableList())
                    {
                        source.PublicOnCollectionChanged(args);
                        result.Should().ContainInOrder(item);
                    }
                };
            action.Should().Throw<Exception>();
        }

        [TestMethod]
        public void Select_ShouldNotAcceptNoIndexOnRemove()
        {
            var item = 3;
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[] { item }, -1);
            var source = new CustomObservableCollection();
            using (var result = source.ToLiveLinq().Select(i => i).ToReadOnlyObservableList())
            {
                source.Insert(0, 1);
                source.Insert(1, item);
                source.Insert(2, 2);
                result.Should().ContainInOrder(1, item, 2);
                Action action = () => source.PublicOnCollectionChanged(args);
                action.Should().Throw<ArgumentException>();
            }
        }

        [TestMethod]
        public void Select_ShouldNotAcceptNoIndexOnReplace()
        {
            var oldItem = 3;
            var newItem = 4;
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new[] { newItem }, new[] { oldItem });
            var source = new CustomObservableCollection();
            using (var result = source.ToLiveLinq().Select(i => i).ToReadOnlyObservableList())
            {
                source.Add(1);
                source.Add(oldItem);
                source.Add(2);
                result.Should().ContainInOrder(1, oldItem, 2);
                Action action = () => source.PublicOnCollectionChanged(args);
                action.Should().Throw<ArgumentException>();
            }
        }

        #endregion

        #region SelectMany ObservableCollection tests

        [TestMethod]
        public void SelectManyObservableCollection_ShouldSyncPropertyWithClear()
        {
            var source = new ObservableCollection<ObservableCollection<int>>(new[]
            {
                new ObservableCollection<int>() { 1, 2, 3 },
                new ObservableCollection<int>() { 4, 5, 6 },
                new ObservableCollection<int>() { 7, 8, 9 },
            });
            using (var synced = source.ToLiveLinq().SelectMany(i => i.ToLiveLinq()).ToReadOnlyObservableList())
            {
                synced.Should().BeEquivalentTo(1, 2, 3, 4, 5, 6, 7, 8, 9);
                source.Clear();
                synced.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void SelectManyObservableCollection_ShouldSyncInitialState()
        {
            var source = new ObservableCollection<ObservableCollection<int>>(new[]
            {
                new ObservableCollection<int>() { 1, 2, 3 },
                new ObservableCollection<int>() { 4, 5, 6 },
                new ObservableCollection<int>() { 7, 8, 9 },
            });
            using (var synced = source.ToLiveLinq().SelectMany(i => i.ToLiveLinq()).ToReadOnlyObservableList())
            {
                synced.Should().BeEquivalentTo(1, 2, 3, 4, 5, 6, 7, 8, 9);
            }
        }

        [TestMethod]
        public void SelectManyObservableCollection_ShouldSyncPriorToDisposal()
        {
            var source = new ObservableCollection<ObservableCollection<int>>();
            using (var synced = source.ToLiveLinq().SelectMany(i => i.ToLiveLinq()).ToReadOnlyObservableList())
            {
                source.Add(new ObservableCollection<int>() { 4 });
                synced.Should().BeEquivalentTo(4);
            }
        }

        [TestMethod]
        public void SelectManyObservableCollection_ShouldNotSyncAfterDisposal()
        {
            var source = new ObservableCollection<ObservableCollection<int>>();
            var synced = source.ToLiveLinq().Select(i => i.ToLiveLinq()).ToReadOnlyObservableList();
            synced.Dispose();
            source.Add(new ObservableCollection<int>() { 4 });
            synced.Should().BeEmpty();
        }

        [TestMethod]
        public void SelectManyObservableSubCollection_ShouldSyncPropertyWithClear()
        {
            var source = new ObservableCollection<ObservableCollection<int>>(new[]
            {
                new ObservableCollection<int>() { 1, 2, 3 },
                new ObservableCollection<int>() { 4, 5, 6 },
                new ObservableCollection<int>() { 7, 8, 9 },
            });
            using (var synced = source.ToLiveLinq().SelectMany(i => i.ToLiveLinq()).ToReadOnlyObservableList())
            {
                synced.Should().BeEquivalentTo(1, 2, 3, 4, 5, 6, 7, 8, 9);
                source[0].Clear();
                synced.Should().BeEquivalentTo(4, 5, 6, 7, 8, 9);
            }
        }

        [TestMethod]
        public void SelectManyObservableSubCollection_ShouldSyncPriorToDisposal()
        {
            var source = new ObservableCollection<ObservableCollection<int>>();
            using (var synced = source.ToLiveLinq().SelectMany(i => i.ToLiveLinq()).ToReadOnlyObservableList())
            {
                source.Add(new ObservableCollection<int>() { 4 });
                synced.Should().BeEquivalentTo(4);
                source[0].Add(5);
                synced.Should().BeEquivalentTo(4, 5);
                source[0].RemoveAt(0);
                synced.Should().BeEquivalentTo(5);
            }
        }

        [TestMethod]
        public void SelectManyObservableSubCollection_ShouldNotSyncAfterDisposal()
        {
            var source = new ObservableCollection<ObservableCollection<int>>();
            var synced = source.ToLiveLinq().SelectMany(i => i.ToLiveLinq()).ToReadOnlyObservableList();
            source.Add(new ObservableCollection<int>() { 4 });
            synced.Should().BeEquivalentTo(4);
            synced.Dispose();
            source[0].Add(5);
            synced.Should().BeEquivalentTo(4);
        }

        #endregion

        #region SyncTo tests

        [TestMethod]
        public void SyncTo_DontSyncInitialValues()
        {
            var obsColl = new ObservableCollection<int>(new[] { 1, 2, 4 });
            var list = new List<int>() { 1, 2, 4 };
            using (obsColl.ToLiveLinq().SyncTo(list, true))
            {
                list.Should().BeEquivalentTo(1, 2, 4);
                obsColl.Clear();
                list.Count.Should().Be(0);
            }
        }

        [TestMethod]
        public void SyncTo_ShouldSyncProperlyWithClear()
        {
            var obsColl = new ObservableCollection<int>(new[] { 1, 2, 4 });
            var list = new List<int>();
            using (obsColl.ToLiveLinq().SyncTo(list, false))
            {
                list.Should().BeEquivalentTo(1, 2, 4);
                obsColl.Clear();
                list.Count.Should().Be(0);
            }
        }

        [TestMethod]
        public void SyncTo_ShouldSyncInitialState()
        {
            var obsColl = new ObservableCollection<int>(new[] { 1, 2, 4 });
            var list = new List<int>();
            using (obsColl.ToLiveLinq().SyncTo(list, false))
            {
                list.Should().BeEquivalentTo(1, 2, 4);
            }
        }
        
        [TestMethod]
        public void SyncTo_ShouldSyncPriorToDisposal()
        {
            var obsColl = new ObservableCollection<int>();
            var list = new List<int>();
            using (obsColl.ToLiveLinq().SyncTo(list, false))
            {
                list.Count.Should().Be(0);
                obsColl.Add(12);
                list.Count.Should().Be(1);
            }
        }
        
        [TestMethod]
        public void SyncTo_ShouldIgnorePreExistingItemsInTarget()
        {
            var obsColl = new ObservableCollection<int>();
            var list = new List<int>() { 5 };
            using (obsColl.ToLiveLinq().SyncTo(list, false))
            {
                list.Count.Should().Be(1);
                obsColl.Add(12);
                list.Count.Should().Be(2);
            }
        }

        [TestMethod]
        public void SyncTo_ShouldNotSyncAfterDisposal()
        {
            var obsColl = new ObservableCollection<int>();
            var list = new List<int>();
            using (obsColl.ToLiveLinq().SyncTo(list, false))
            {
                list.Count.Should().Be(0);
                obsColl.Add(12);
                list.Count.Should().Be(1);
            }
            obsColl.Add(14);
            list.Count.Should().Be(1);
        }

        #endregion

        #region Subscribe
        
        [TestMethod]
        public void Subscribe_Unsubscribe()
        {
            var source = new ObservableCollection<BehaviorSubject<int>>();
            var output = new List<int>();
            var mainDisposable = source.ToLiveLinq().Subscribe(
                (rp) => rp.Subscribe(output.Add),
                (_, disp) => disp.Dispose());
            source.Add(new BehaviorSubject<int>(0));
            source[0].OnNext(1);
            var tmp = source[0];
            source[0] = new BehaviorSubject<int>(2);
            tmp.OnNext(-1);
            mainDisposable.Dispose();
            source[0].OnNext(-2);
            Thread.Sleep(300);
            output.Should().BeEquivalentTo(0, 1, 2);
        }

        [TestMethod]
        public void Subscribe_Clear()
        {
            var source = new ObservableCollection<int>();
            var output = new List<int>();

            using (source.ToLiveLinq().Subscribe(
                (val) => output.Add(val),
                (val) => output.Remove(val)))
            {
                source.Add(0);
                source.Add(1);
                output.Should().BeEquivalentTo(0, 1);
                source.Clear();
                output.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void Subscribe_Remove()
        {
            var source = new ObservableCollection<int>();
            var output = new List<int>();

            using (source.ToLiveLinq().Subscribe(
                (val) => output.Add(val),
                (val) => output.Remove(val)))
            {
                source.Add(0);
                source.Add(1);
                output.Should().BeEquivalentTo(0, 1);
                source.RemoveAt(0);
                output.Should().BeEquivalentTo(1);
            }
        }

        [TestMethod]
        public void Subscribe_AddInitialState()
        {
            var source = new ObservableCollection<int>();
            var output = new List<int>();
            source.Add(0);
            source.Add(1);

            using (source.ToLiveLinq().Subscribe(
                (val) => output.Add(val),
                (val) => output.Remove(val)))
            {
                output.Should().BeEquivalentTo(0, 1);
            }
        }

        [TestMethod]
        public void Subscribe_RemoveFinalState()
        {
            var source = new ObservableCollection<int>();
            var output = new List<int>();

            using (source.ToLiveLinq().Subscribe(
                (val) => output.Add(val),
                (val) => output.Remove(val)))
            {
                source.Add(0);
                source.Add(1);
                output.Should().BeEquivalentTo(0, 1);
                source.Clear();
                output.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void Subscribe_AddIndicesShouldBeCorrect()
        {
            var source = new ObservableList<string>();
            source.AddRange("a", "b", "c");

            var result = new Dictionary<int, string>();
            var items =
                source.ToLiveLinq().Subscribe(
                    (item, index) =>
                    {
                        result[index] = item;
                    },
                    (item, oldIndex, newIndex) =>
                    {
                        result[oldIndex].Should().Be(item);
                        result.Remove(oldIndex);
                        result.Add(newIndex, item);
                    },
                    (item, oldIndex, removalMove) =>
                    {
                        result[oldIndex].Should().Be(item);
                        result.Remove(oldIndex);
                    },
                    true, true);
            source.Add("d");
            result.Count.Should().Be(4);
            result[0].Should().Be("a");
            result[1].Should().Be("b");
            result[2].Should().Be("c");
            result[3].Should().Be("d");
            items.Dispose();

            result.Should().BeEmpty();
        }
        
        #endregion

        #region Misc tests

        [TestMethod]
        public void DoPrevious()
        {
            var subj = new Subject<int>();
            var list = new List<int>();
            subj.DoPrevious(list.Add, false, false).Subscribe();
            subj.OnNext(1);
            subj.OnNext(2);
            subj.OnNext(3);
            list.Should().BeEquivalentTo(1, 2);
        }

        [TestMethod]
        public void ObserveCount()
        {
            var coll = new ObservableCollection<int>();
            var prop = coll.ToLiveLinq().Count().ToBehaviorSubject();
            prop.Value.Should().Be(0);
            coll.Add(4);
            prop.Value.Should().Be(1);
            coll[0] = 5;
            prop.Value.Should().Be(1);
            coll.Add(2);
            prop.Value.Should().Be(2);
            coll.RemoveAt(0);
            prop.Value.Should().Be(1);
            coll.RemoveAt(0);
            prop.Value.Should().Be(0);
        }

        #endregion
    }
}
