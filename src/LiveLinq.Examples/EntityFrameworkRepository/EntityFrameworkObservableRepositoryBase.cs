using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using LiveLinq.Dictionary;
using Microsoft.EntityFrameworkCore;
using MoreCollections;

namespace LiveLinq.Examples.EntityFrameworkRepository
{
    public abstract class EntityFrameworkRepositoryBase<TId, TAggregateRoot, TDbContext, TDbDto> : ObservableDictionaryBase<TId, TAggregateRoot> where TDbDto : class where TDbContext : DbContext
    {
        private bool _hasMigratedYet = false;
        private readonly bool _migrateOnDemand;

        protected EntityFrameworkRepositoryBase(bool migrateOnDemand)
        {
            _migrateOnDemand = migrateOnDemand;
        }

        protected abstract TDbContext CreateDbContextInternal();
        
        protected virtual TDbContext CreateDbContext()
        {
            if (!_hasMigratedYet && _migrateOnDemand)
            {
                using (var context = CreateDbContextInternal())
                {
                    context.Database.Migrate();
                }
                _hasMigratedYet = true;
            }

            return CreateDbContextInternal();
        }
        
        protected abstract DbSet<TDbDto> GetDbSet(TDbContext dbContext);
        protected abstract TId GetId(TDbDto dbDto);
        protected abstract TDbDto Convert(TAggregateRoot aggregateRoot);
        protected abstract TAggregateRoot Convert(TDbDto dbDto);
        protected abstract void Update(TAggregateRoot source, TDbDto destination);

        protected override IEnumerator<KeyValuePair<TId, TAggregateRoot>> GetKeyValuePairEnumeratorInternal()
        {
            using (var dbContext = CreateDbContext())
            {
                return GetDbSet(dbContext).Select(x => new KeyValuePair<TId, TAggregateRoot>(GetId(x), Convert((TDbDto) x))).ToImmutableList().GetEnumerator();
            }
        }

        protected override IEnumerator<IKeyValuePair<TId, TAggregateRoot>> GetIKeyValuePairEnumeratorInternal()
        {
            using (var dbContext = CreateDbContext())
            {
                return GetDbSet(dbContext).Select(x => MoreCollections.Utility.KeyValuePair(GetId(x), Convert((TDbDto) x))).ToImmutableList().GetEnumerator();
            }
        }

        public override bool ContainsKey(TId id)
        {
            using (var dbContext = CreateDbContext())
            {
                return GetDbSet(dbContext).Find(id) != null;
            }
        }

        public override bool TryGetValue(TId id, out TAggregateRoot value)
        {
            using (var dbContext = CreateDbContext())
            {
                var item = GetDbSet(dbContext).Find(id);
                if (item != null)
                {
                    value = Convert(item);
                    return true;
                }
                else
                {
                    value = default;
                    return false;
                }
            }
        }

        protected override void AddInternal(TId id, TAggregateRoot value)
        {
            using (var dbContext = CreateDbContext())
            {
                GetDbSet(dbContext).Add(Convert(value));
                dbContext.SaveChanges();
            }
        }

        protected override void RemoveInternal(TId id)
        {
            using (var dbContext = CreateDbContext())
            {
                var item = GetDbSet(dbContext).Find(id);
                if (item == null)
                {
                    throw new KeyNotFoundException();
                }
                GetDbSet(dbContext).Remove(item);
                dbContext.SaveChanges();
            }
        }

        protected override void AddRangeInternal(ImmutableList<KeyValuePair<TId, TAggregateRoot>> pairs)
        {
            using (var dbContext = CreateDbContext())
            {
                GetDbSet(dbContext).AddRange(pairs.Select(pair => Convert(pair.Value)));
                dbContext.SaveChanges();
            }
        }

        protected override AddOrUpdateResult AddOrUpdateInternal(TId id, TAggregateRoot value, out TAggregateRoot preExistingValue)
        {
            using (var dbContext = CreateDbContext())
            {
                var item = GetDbSet(dbContext).Find(id);
                if (item == null)
                {
                    preExistingValue = default;
                    GetDbSet(dbContext).Add(Convert(value));
                    dbContext.SaveChanges();
                    return AddOrUpdateResult.Add;
                }
                else
                {
                    preExistingValue = Convert(item);
                    Update(value, item);
                    GetDbSet(dbContext).Update(item);
                    dbContext.SaveChanges();
                    return AddOrUpdateResult.Update;
                }
            }
        }

        protected override void RemoveRangeInternal(IEnumerable<TId> keys)
        {
            using (var dbContext = CreateDbContext())
            {
                GetDbSet(dbContext).RemoveRange(keys.Select(x => GetDbSet(dbContext).Find(x)));
                dbContext.SaveChanges();
            }
        }

        public override int Count
        {
            get
            {
                using (var dbContext = CreateDbContext())
                {
                    return GetDbSet(dbContext).Count();
                }
            }
        }
    }
}