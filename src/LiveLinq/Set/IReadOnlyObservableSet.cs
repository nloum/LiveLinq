using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveLinq.Set
{
    public interface IReadOnlyObservableSet<out T> : IEnumerable<T>
    {
        ISetChanges<T> ToLiveLinq();
    }
}
