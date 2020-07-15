using System;
using LiveLinq.Dictionary;

namespace LiveLinq.Examples.EntityFrameworkRepository.Inventory
{
    public class InMemoryInventoryRepository : ObservableDictionary<Guid, Inventory>, IInventoryRepository
    {
    }
}