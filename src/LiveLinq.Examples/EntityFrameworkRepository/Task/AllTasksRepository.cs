using System;
using System.Collections.Generic;
using System.Linq;
using LiveLinq.Dictionary;

namespace LiveLinq.Examples.EntityFrameworkRepository.Task
{
    public class AllTasksRepository : AggregateObservableDictionaryBase<Guid, Task>
    {
        private readonly ITaskRepository _taskRepo;
        public AllTasksRepository(ITaskRepository taskRepo, IEnumerable<IAutoGeneratedTaskRepository> autoGeneratedTaskRepos) : base(autoGeneratedTaskRepos.Cast<IReadOnlyObservableDictionary<Guid, Task>>().Concat(new[]{taskRepo}))
        {
            _taskRepo = taskRepo;
        }

        protected override void AddInternal(Guid key, Task value)
        {
            _taskRepo.Add(key, value);
        }

        protected override void RemoveInternal(Guid key)
        {
            _taskRepo.Remove(key);
        }
    }
}