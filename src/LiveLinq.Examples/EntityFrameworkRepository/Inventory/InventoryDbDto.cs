using System;

namespace LiveLinq.Examples.EntityFrameworkRepository.Inventory
{
    public class InventoryDbDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
    }
}