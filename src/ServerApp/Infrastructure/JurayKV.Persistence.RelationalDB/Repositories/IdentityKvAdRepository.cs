using JurayKV.Domain.Aggregates.IdentityKvAdAggregate;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TanvirArjel.ArgumentChecker;
using JurayKV.Domain.Aggregates.IdentityActivityAggregate;
using JurayKV.Persistence.RelationalDB.Repositories.GenericRepositories;

namespace JurayKV.Persistence.RelationalDB.Repositories
{
    internal sealed class IdentityKvAdRepository : GenericRepository<IdentityKvAd>, IIdentityKvAdRepository
    {
        private readonly JurayDbContext _dbContext;

        public IdentityKvAdRepository(JurayDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<bool> ExistsAsync(Expression<Func<IdentityKvAd, bool>> condition)
        {
            IQueryable<IdentityKvAd> queryable = _dbContext.Set<IdentityKvAd>();

            if (condition != null)
            {
                queryable = queryable.Where(condition);
            }

            return queryable.AnyAsync();
        }

        public async Task<IdentityKvAd> GetByIdAsync(Guid identityKvAdId)
        {
            identityKvAdId.ThrowIfEmpty(nameof(identityKvAdId));

            IdentityKvAd identityKvAd = await _dbContext.IdentityKvAds.Include(x => x.KvAd).Include(x=>x.User).FirstOrDefaultAsync(y => y.Id == identityKvAdId);
            return identityKvAd;
        }

        public async Task InsertAsync(IdentityKvAd identityKvAd)
        {
            identityKvAd.ThrowIfNull(nameof(identityKvAd));
            var checkadsforuser = await _dbContext.IdentityKvAds.FirstOrDefaultAsync(x => x.UserId == identityKvAd.UserId && x.Id == identityKvAd.Id);
            if (checkadsforuser == null)
            {

                await _dbContext.AddAsync(identityKvAd);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(IdentityKvAd identityKvAd)
        {
            identityKvAd.ThrowIfNull(nameof(identityKvAd));

            EntityEntry<IdentityKvAd> trackedEntity = _dbContext.ChangeTracker.Entries<IdentityKvAd>()
                .FirstOrDefault(x => x.Entity == identityKvAd);

            if (trackedEntity == null)
            {
                _dbContext.Update(identityKvAd);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(IdentityKvAd identityKvAd)
        {
            identityKvAd.ThrowIfNull(nameof(identityKvAd));

            _dbContext.Remove(identityKvAd);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IQueryable<IdentityKvAd>> GetListByUserId(Guid userId)
        {
            var data = _dbContext.IdentityKvAds.Include(x => x.KvAd).Where(x => x.UserId == userId);
            return data;
        }

        public async Task<IQueryable<IdentityKvAd>> GetActiveListByUserId(Guid userId)
        {
            var data = _dbContext.IdentityKvAds.Include(x => x.KvAd).Where(x => x.UserId == userId && x.Active == true);
            return data;
        }
    }

}
