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
using JurayKV.Domain.ValueObjects;

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

            IdentityKvAd identityKvAd = await _dbContext.IdentityKvAds.Include(x => x.KvAd).Include(x => x.KvAd.Company).Include(x => x.KvAd.Bucket).Include(x => x.KvAd.ImageFile).Include(x => x.User).FirstOrDefaultAsync(y => y.Id == identityKvAdId);
            return identityKvAd;
        }

        public async Task<Guid> InsertAsync(IdentityKvAd identityKvAd)
        {
            try
            {
                identityKvAd.ThrowIfNull(nameof(identityKvAd));
                
                var checkadsforuser = await _dbContext.IdentityKvAds.AsNoTracking()
                    //.Where(x => x.CreatedAtUtc.Hour > nextDay6AM.Hour)
                    .FirstOrDefaultAsync(x => x.UserId == identityKvAd.UserId && x.KvAdId == identityKvAd.KvAdId && x.KvAdHash == identityKvAd.KvAdHash &&
                    x.Active == true);
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
            DateTime currentDate = DateTime.UtcNow.AddHours(1);
            DateTime today6AM = currentDate.Date.AddHours(6);
            var data = await _dbContext.IdentityKvAds.Include(x => x.KvAd).Include(x => x.KvAd.ImageFile)
                .Where(x => x.UserId == userId && x.Active == false)


                .ToListAsync();


            var xxdata = await _dbContext.IdentityKvAds.Include(x => x.KvAd).Include(x => x.KvAd.ImageFile)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.CreatedAtUtc.Hour < today6AM.Hour);


            return data.ToList();
        }
        public async Task<IQueryable<IdentityKvAd>> GetActiveListByCompanyId(Guid companyId)
        {
            //DateTime currentDate = DateTime.UtcNow.AddHours(1);
            //DateTime nextDay6AM = currentDate.Date.AddDays(1).AddHours(6);

            DateTime mdate = DateForSix.GetTheDateBySix(DateTime.UtcNow.AddHours(1));


            var data = _dbContext.IdentityKvAds.Include(x => x.KvAd).Include(x => x.KvAd.ImageFile)
            .Where(x => x.KvAdHash == mdate.Date.ToString("ddMMyyyy"))
            .Where(x => x.KvAd.CompanyId == companyId && x.Active == true);


            return data;
        }
        public async Task<IQueryable<IdentityKvAd>> GetActiveListByUserId(Guid userId)
        {
            //DateTime currentDate = DateTime.UtcNow.AddHours(1);
            //DateTime nextDay6AM = currentDate.Date.AddDays(1).AddHours(6);

            DateTime mdate = DateForSix.GetTheDateBySix(DateTime.UtcNow.AddHours(1));


            var data = _dbContext.IdentityKvAds.Include(x => x.KvAd).Include(x => x.KvAd.ImageFile)
            .Where(x => x.KvAdHash == mdate.Date.ToString("ddMMyyyy"))
            .Where(x => x.UserId == userId && x.Active == true); 


            return data;
        }
        public async Task<List<IdentityKvAd>> ListNonActive()
        {
            DateTime currentDate = DateForSix.GetTheDateBySix(DateTime.UtcNow.AddHours(1));

            var data = await _dbContext.IdentityKvAds
                .Include(x => x.KvAd)
                .ThenInclude(x => x.Company).Include(x => x.KvAd.ImageFile)
                .Include(x => x.KvAd.Bucket)
                .Include(x => x.KvAd.ImageFile)
                 .Include(x => x.User)
                .Where(x => x.KvAdHash != currentDate.ToString("ddMMyyyy")).ToListAsync();

            return data.ToList();
        }

        public async Task<IQueryable<IdentityKvAd>> ListActiveToday()
        {
            DateTime currentDate = DateForSix.GetTheDateBySix(DateTime.UtcNow.AddHours(1));

            var data = _dbContext.IdentityKvAds
                .Include(x => x.KvAd)
                .ThenInclude(x => x.Company).Include(x => x.KvAd.ImageFile)
                .Include(x=>x.KvAd.Bucket) 

                .Include(x => x.User)
                .Where(x => x.Active == true && x.KvAdHash == currentDate.ToString("ddMMyyyy"));

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

        public async Task<int> AdsCount(Guid userId)
        {
           return await  _dbContext.IdentityKvAds.Where(x=>x.UserId ==  userId).CountAsync();
        }
    }

}
