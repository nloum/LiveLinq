using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace LiveLinq.Examples.EntityFrameworkRepository
{
    public abstract class EntityFrameworkAutoMapperRepositoryBase<TId, TAggregateRoot, TDbContext, TDbDto> : EntityFrameworkRepositoryBase<TId, TAggregateRoot, TDbContext, TDbDto> where TDbContext : DbContext where TDbDto : class
    {
        private readonly IMapper _mapper;

        public EntityFrameworkAutoMapperRepositoryBase(IMapper mapper, bool migrateOnDemand) : base(migrateOnDemand)
        {
            _mapper = mapper;
        }

        protected override TDbDto Convert(TAggregateRoot aggregateRoot) =>
            _mapper.Map<TAggregateRoot, TDbDto>(aggregateRoot);

        protected override TAggregateRoot Convert(TDbDto dbDto)
        {
            return _mapper.Map<TDbDto, TAggregateRoot>(dbDto);
        }

        protected override void Update(TAggregateRoot source, TDbDto destination)
        {
            _mapper.Map(source, destination);
        }
    }
}