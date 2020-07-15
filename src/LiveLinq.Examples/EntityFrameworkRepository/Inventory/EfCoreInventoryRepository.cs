using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace LiveLinq.Examples.EntityFrameworkRepository.Inventory
{
    public class EfCoreInventoryRepository : EntityFrameworkAutoMapperRepositoryBase<Guid, Inventory, InventoryDbContext, InventoryDbDto>, IInventoryRepository
    {
        public EfCoreInventoryRepository(IMapper mapper) : base(mapper, true)
        {
        }

        protected override InventoryDbContext CreateDbContextInternal() => new InventoryDbContext();

        protected override DbSet<InventoryDbDto> GetDbSet(InventoryDbContext dbContext) => dbContext.Inventory;

        protected override Guid GetId(InventoryDbDto dbDto) => dbDto.Id;
    }
}