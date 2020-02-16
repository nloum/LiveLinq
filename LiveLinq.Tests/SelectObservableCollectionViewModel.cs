using System.Reactive.Subjects;

namespace LiveLinq.Tests
{
    public class SelectObservableCollectionViewModel
    {
        public SelectObservableCollectionViewModel(string name)
        {
            Name = new BehaviorSubject<string>(name);
        }

        public BehaviorSubject<string> Name { get; private set; }
    }
}
