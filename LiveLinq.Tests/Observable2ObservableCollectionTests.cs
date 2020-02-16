using System.Reactive.Subjects;
using System.Threading;
using FluentAssertions;

using LiveLinq.List;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveLinq.Tests
{
    [TestClass]
    public class Observable2ObservableCollectionTests
    {
        public Observable2ObservableCollectionTests()
        {
            if (SynchronizationContext.Current == null)
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }

        [TestMethod]
        public void Basic()
        {
            var prop = new BehaviorSubject<int>(3);
            IReadOnlyObservableList<int> coll = prop.ToLiveLinq<int>().ToReadOnlyObservableList();
            coll.Count.Should().Be(1);
            coll[0].Should().Be(3);
            prop.OnNext(4);
            coll[0].Should().Be(4);
        }
    }
}
