using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace LiveLinq.Examples.EntityFrameworkRepository.Task
{
    public class EfCoreTaskRepository : EntityFrameworkAutoMapperRepositoryBase<Guid, Task, TaskDbContext, TaskDbDto>, ITaskRepository
    {
        public EfCoreTaskRepository(IMapper mapper) : base(mapper, true)
        {
        }

        protected override TaskDbContext CreateDbContextInternal() => new TaskDbContext();
        protected override DbSet<TaskDbDto> GetDbSet(TaskDbContext dbContext) => dbContext.Task;
        protected override Guid GetId(TaskDbDto dbDto) => dbDto.Id;
    }
}