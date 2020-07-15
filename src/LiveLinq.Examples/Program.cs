using System;
using System.Linq;
using AutoMapper;
using LiveLinq.Dictionary;
using LiveLinq.Examples.EntityFrameworkRepository;
using LiveLinq.Examples.EntityFrameworkRepository.Inventory;
using LiveLinq.Examples.EntityFrameworkRepository.Task;

namespace LiveLinq.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            // var config = new MapperConfiguration(cfg =>
            // {
            //     cfg.CreateMap<InventoryDbDto, Inventory>();
            //     cfg.CreateMap<Inventory, InventoryDbDto>();
            // });
            //
            // var mapper = new Mapper(config);
            
            var inventoryRepo = new InMemoryInventoryRepository();
            
            var inventory = new Inventory(Guid.NewGuid())
            {
                Description = "Beans",
                Quantity = 12,
                Unit = "half-gallon jars"
            };
            
            
            inventoryRepo.Add(inventory.Id, inventory);
            var inventoryTasksRepo = new InventoryTasksRepository(inventoryRepo);
            
            var tasksRepo = new InMemoryTaskRepository();
            
            var allTasks = new AllTasksRepository(tasksRepo, new[]{inventoryTasksRepo});

            foreach (var item in allTasks.Values)
            {
                Console.WriteLine($"Task: {item.Description}");
            }
        }
    }
}