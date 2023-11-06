using JurayKV.Domain.Aggregates.KvAdAggregate;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TanvirArjel.ArgumentChecker;
using JurayKV.Domain.Aggregates.IdentityKvAdAggregate;
using JurayKV.Persistence.RelationalDB.Repositories.GenericRepositories;

namespace JurayKV.Persistence.RelationalDB.Repositories
{
    internal sealed class KvAdRepository : GenericRepository<KvAd>, IKvAdRepository
    {
        private readonly JurayDbContext _dbContext;

        public KvAdRepository(JurayDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<bool> ExistsAsync(Expression<Func<KvAd, bool>> condition)
        {
            IQueryable<KvAd> queryable = _dbContext.Set<KvAd>();

            if (condition != null)
            {
                queryable = queryable.Where(condition);
            }

            return queryable.AnyAsync();
        }

        public async Task<KvAd> GetByIdAsync(Guid kvAdId)
        {
            kvAdId.ThrowIfEmpty(nameof(kvAdId));

            KvAd kvAd = await _dbContext.Set<KvAd>().FindAsync(kvAdId);
            return kvAd;
        }

        public async Task InsertAsync(KvAd kvAd)
        {
            kvAd.ThrowIfNull(nameof(kvAd));

            await _dbContext.AddAsync(kvAd);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(KvAd kvAd)
        {
            kvAd.ThrowIfNull(nameof(kvAd));

            EntityEntry<KvAd> trackedEntity = _dbContext.ChangeTracker.Entries<KvAd>()
                .FirstOrDefault(x => x.Entity == kvAd);

            if (trackedEntity == null)
            {
                _dbContext.Update(kvAd);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(KvAd kvAd)
        {
            kvAd.ThrowIfNull(nameof(kvAd));

            _dbContext.Remove(kvAd);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<KvAd>> AdsByBucketId(Guid bucketId)
        {
            var data = await _dbContext.kvAds
                .Include(x=>x.Bucket)
                .Include(x=>x.Company)
                .Where(x=>x.BucketId == bucketId && x.Status == Domain.Primitives.Enum.DataStatus.Active)
                .ToListAsync();

            return data;
        }
    }

}
