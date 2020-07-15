using Microsoft.EntityFrameworkCore;

namespace LiveLinq.Examples.EntityFrameworkRepository.Task
{
    public class TaskDbContext : DbContext
    {
        public DbSet<TaskDbDto> Task { get; set; }
        
        protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder ) {
            optionsBuilder.UseNpgsql( "host=localhost;database=postgres;user id=livelinq;password=supersecret;" );
        }
    }
}