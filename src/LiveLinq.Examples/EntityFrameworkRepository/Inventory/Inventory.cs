using System;

namespace LiveLinq.Examples.EntityFrameworkRepository.Inventory
{
    public class Inventory
    {
        public Inventory(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
    }
}