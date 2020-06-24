using System.Reactive.Subjects;

namespace LiveLinq.Tests
{
    public class NestedViewModel
    {
        public NestedViewModel(SelectObservableCollectionViewModel property)
        {
            Property = new BehaviorSubject<SelectObservableCollectionViewModel>(property);
        }

        public BehaviorSubject<SelectObservableCollectionViewModel> Property { get; private set; }
    }
}
