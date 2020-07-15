using System;
using LiveLinq.Dictionary;

namespace LiveLinq.Examples.EntityFrameworkRepository.Task
{
    public class InMemoryTaskRepository : ObservableDictionary<Guid, Task>, ITaskRepository
    {
    }
}