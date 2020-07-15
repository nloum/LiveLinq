using System;
using LiveLinq.Dictionary;

namespace LiveLinq.Examples.EntityFrameworkRepository.Task
{
    public interface ITaskRepository : IObservableDictionary<Guid, Task>
    {
    }
}