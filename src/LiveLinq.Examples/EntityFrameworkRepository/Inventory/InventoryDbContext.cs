using Microsoft.EntityFrameworkCore;

namespace LiveLinq.Examples.EntityFrameworkRepository.Inventory
{
    public class InventoryDbContext : DbContext
    {
        public DbSet<InventoryDbDto> Inventory { get; set; }
        
        protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder ) {
            optionsBuilder.UseNpgsql( "host=localhost;database=postgres;user id=livelinq;password=supersecret;" );
        }
    }
}