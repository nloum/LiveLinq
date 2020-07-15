using System;

namespace LiveLinq.Examples.EntityFrameworkRepository.Task
{
    public class Task
    {
        public Task(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
        public string Description { get; set; }
    }
}