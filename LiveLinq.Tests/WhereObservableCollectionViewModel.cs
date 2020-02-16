using System.Reactive.Subjects;

namespace LiveLinq.Tests
{
    public class WhereObservableCollectionViewModel
    {
        public WhereObservableCollectionViewModel(bool predicate)
        {
            Predicate = new BehaviorSubject<bool>(predicate);
        }

        public BehaviorSubject<bool> Predicate { get; private set; }
    }
}
