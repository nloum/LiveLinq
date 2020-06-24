using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveLinq.Core;

namespace LiveLinq.Set
{
    public interface ISetChange<out T> : ICollectionChange<T>
    {
        IEnumerable<ISetChange<T>> Itemize();
    }
}
