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

            IdentityKvAd identityKvAd = await _dbContext.IdentityKvAds.Include(x => x.KvAd).Include(x => x.User).FirstOrDefaultAsync(y => y.Id == identityKvAdId);
            return identityKvAd;
        }

        public async Task<Guid> InsertAsync(IdentityKvAd identityKvAd)
        {
            try
            {
                identityKvAd.ThrowIfNull(nameof(identityKvAd));
                DateTime currentDate = DateTime.Now;
                DateTime nextDay6AM = currentDate.Date.AddDays(1).AddHours(6);


                var checkadsforuser = await _dbContext.IdentityKvAds.AsNoTracking()
                    .Where(x => x.CreatedAtUtc.Hour > nextDay6AM.Hour)
                    .FirstOrDefaultAsync(x => x.UserId == identityKvAd.UserId && x.KvAdId == identityKvAd.KvAdId);
                if (checkadsforuser == null)
                {
                    await _dbContext.AddAsync(identityKvAd);
                    await _dbContext.SaveChangesAsync();

                    return identityKvAd.Id;
                }
            }
            catch (Exception c)
            {
                var s = "";
            }
            return Guid.Empty;
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

        public async Task<List<IdentityKvAd>> GetListByUserId(Guid userId)
        {
            DateTime currentDate = DateTime.Now;
            DateTime today6AM = currentDate.Date.AddHours(6);
            var data = await _dbContext.IdentityKvAds.Include(x => x.KvAd)
                .Where(x => x.UserId == userId)
                 //.Where(x => x.CreatedAtUtc > today6AM)


                .ToListAsync();
             
            return data.ToList();
        }

        public async Task<IQueryable<IdentityKvAd>> GetActiveListByUserId(Guid userId)
        {
            DateTime currentDate = DateTime.Now;
            DateTime nextDay6AM = currentDate.Date.AddDays(1).AddHours(6);

            var data = _dbContext.IdentityKvAds.Include(x => x.KvAd)
            .Where(x => x.CreatedAtUtc < nextDay6AM)
            .Where(x => x.UserId == userId && x.Active == true);


            return data;
        }

        public async Task<IQueryable<IdentityKvAd>> ListActiveToday()
        {
            DateTime currentDate = DateTime.Now;
            DateTime nextDay6AM = currentDate.Date.AddDays(1).AddHours(6);

            var data = _dbContext.IdentityKvAds
                .Include(x => x.KvAd)
                .Where(x => x.CreatedAtUtc >= currentDate && x.CreatedAtUtc < nextDay6AM);

            return data;
        }

        public async Task<bool> CheckExist(Guid userId, Guid kvAdId)
        {
            var checkadsforuser = await _dbContext.IdentityKvAds.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId && x.Id == kvAdId);
            if (checkadsforuser == null)
            {
                return false;
            }

            return true;

        }

        public Task<bool> CheckUserAdvertCountToday(Guid userId)
        {
            throw new NotImplementedException();
        }
    }

}
