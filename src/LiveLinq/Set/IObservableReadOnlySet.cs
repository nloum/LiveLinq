using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveLinq.Set
{
    public interface IObservableReadOnlySet<out T> : IReadOnlyCollection<T>, IDisposable
    {
        ISetChanges<T> ToLiveLinq();
    }
}
