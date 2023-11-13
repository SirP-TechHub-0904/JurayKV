using JurayKV.Domain.Aggregates.KvPointAggregate;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TanvirArjel.ArgumentChecker;
using JurayKV.Domain.Aggregates.KvAdAggregate;
using JurayKV.Persistence.RelationalDB.Repositories.GenericRepositories;

namespace JurayKV.Persistence.RelationalDB.Repositories
{
    internal sealed class KvPointRepository : GenericRepository<KvPoint>, IKvPointRepository
    {
        private readonly JurayDbContext _dbContext;

        public KvPointRepository(JurayDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<bool> ExistsAsync(Expression<Func<KvPoint, bool>> condition)
        {
            IQueryable<KvPoint> queryable = _dbContext.Set<KvPoint>();

            if (condition != null)
            {
                queryable = queryable.Where(condition);
            }

            return queryable.AnyAsync();
        }

        public async Task<KvPoint> GetByIdAsync(Guid kvPointId)
        {
            kvPointId.ThrowIfEmpty(nameof(kvPointId));

            KvPoint kvPoint = await _dbContext.Set<KvPoint>().FindAsync(kvPointId);
            return kvPoint;
        }

        public async Task InsertAsync(KvPoint kvPoint)
        {
            kvPoint.ThrowIfNull(nameof(kvPoint));

            await _dbContext.AddAsync(kvPoint);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(KvPoint kvPoint)
        {
            kvPoint.ThrowIfNull(nameof(kvPoint));

            EntityEntry<KvPoint> trackedEntity = _dbContext.ChangeTracker.Entries<KvPoint>()
                .FirstOrDefault(x => x.Entity == kvPoint);

            if (trackedEntity == null)
            {
                _dbContext.Update(kvPoint);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(KvPoint kvPoint)
        {
            kvPoint.ThrowIfNull(nameof(kvPoint));

            _dbContext.Remove(kvPoint);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<KvPoint>> LastTenByUserId(int toplistcount, Guid userId)
        {
            var list = await _dbContext.kvPoints.OrderByDescending(x=>x.CreatedAtUtc).Take(toplistcount).ToListAsync();
            return list;
        }

        public async Task<KvPoint> GetByIdentityIdByUserAsync(Guid identityKvAd, Guid userId)
        {
            var data = await _dbContext.kvPoints.FirstOrDefaultAsync(x=> x.IdentityKvAdId == identityKvAd && x.UserId == userId);
            return data;
        }

        public async Task<List<KvPoint>> LastByUserId(Guid userId)
        {
            var list = await _dbContext.kvPoints.OrderByDescending(x => x.CreatedAtUtc).ToListAsync();
            return list;
        }
    }

}
