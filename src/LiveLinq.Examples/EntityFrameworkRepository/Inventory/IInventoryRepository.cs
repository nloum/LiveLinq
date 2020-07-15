using System;
using LiveLinq.Dictionary;

namespace LiveLinq.Examples.EntityFrameworkRepository.Inventory
{
    public interface IInventoryRepository : IObservableDictionary<Guid, Inventory>
    {
    }
}