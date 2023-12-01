using JurayKV.Application.Caching.Repositories;
using JurayKV.Domain.Aggregates.IdentityAggregate;
using JurayKV.Domain.Aggregates.IdentityKvAdAggregate;
using JurayKV.Domain.Aggregates.KvAdAggregate;
using JurayKV.Domain.Aggregates.KvPointAggregate;
using JurayKV.Persistence.Cache.Keys;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TanvirArjel.EFCore.GenericRepository;
using TanvirArjel.Extensions.Microsoft.Caching;

namespace JurayKV.Persistence.Cache.Repositories
{
    public sealed class DashboardCacheRepository : IDashboardCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IQueryRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        public DashboardCacheRepository(IDistributedCache distributedCache, IQueryRepository repository, UserManager<ApplicationUser> userManager)
        {
            _distributedCache = distributedCache;
            _repository = repository;
            _userManager = userManager;
        }
        public async Task<int> ActiveAdsCount()
        {
            //string cacheKey = DashboardCacheKeys.ActiveAdsCountKey;
            //int count = await _distributedCache.GetAsync<int>(cacheKey);

            //if (count == 0)
            //{
            // Define the start time (6:00 AM) and end time (5:59 AM the next day)
            var startTime = new TimeSpan(6, 0, 0); // 6:00 AM
            var endTime = new TimeSpan(5, 59, 59); // 5:59 AM the next day

            // Define the current date and the date for the next day
            var currentDate = DateTime.UtcNow.Date;
            var nextDayDate = DateTime.UtcNow.Date.AddDays(1);

            // Define the condition to check if CreatedAtUtc is within the specified time frame
            Expression<Func<IdentityKvAd, bool>> withinTimeFrameCondition = data =>
                data.Active == true;
            //

            int count = await _repository.GetCountAsync<IdentityKvAd>(withinTimeFrameCondition); // Assuming there's a CountAsync method in your repository.

            //    await _distributedCache.SetAsync(cacheKey, count);
            //}

            return count;
        }

        

        public async Task<int> TotalActiveUsersCount()
        {
            //string cacheKey = DashboardCacheKeys.TotalActiveUsersCountKey;
            //int count = await _distributedCache.GetAsync<int>(cacheKey);

            //if (count == 0)
            //{
            DateTime fiveDaysAgo = DateTime.UtcNow.AddDays(-5);

            Expression<Func<IdentityKvAd, bool>> kvAdCondition = kvAd =>
                kvAd.CreatedAtUtc >= fiveDaysAgo &&
                kvAd.CreatedAtUtc <= DateTime.UtcNow;

            // Get the Ids of users who meet the kvAdCondition criteria.
            var user = await _repository.GetListAsync<IdentityKvAd>(kvAdCondition);
            var userIds = user.Select(x => x.UserId);


            // Check if there are users with these Ids.
            int count = await _userManager.Users
                 .Where(user => userIds.Contains(user.Id))
                 .CountAsync();

            //    await _distributedCache.SetAsync(cacheKey, count);
            //}

            return count;
        }

        public async Task<decimal> TotalPointsEarnTheseMonthCount()
        {
            //string cacheKey = DashboardCacheKeys.TotalPointsEarnTheseMonthCountKey;
            //decimal sum = await _distributedCache.GetAsync<decimal>(cacheKey);

            //if (sum == 0)
            //{
            // Calculate the start and end dates for the current month.
            DateTime startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddSeconds(-1);

            Expression<Func<KvPoint, bool>> pointsCondition = data =>
                data.CreatedAtUtc >= startDate && data.CreatedAtUtc <= endDate;

            // Get the sum of points earned this month.
            var outcome = await _repository
                .GetListAsync<KvPoint>(pointsCondition);

            int sum = outcome.Sum(kvPoint => kvPoint.Point);


            //    await _distributedCache.SetAsync(cacheKey, sum);
            //}

            return sum;
        }

        public async Task<decimal> TotalPointsEarnTheseWeekCount()
        {
            //string cacheKey = DashboardCacheKeys.TotalPointsEarnTheseWeekCountKey;
            //decimal sum = await _distributedCache.GetAsync<decimal>(cacheKey);

            //if (sum == 0)
            //{
            DateTime currentDate = DateTime.UtcNow;
            DateTime startOfWeek = currentDate.Date.AddDays(-(int)currentDate.DayOfWeek);
            DateTime endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);

            Expression<Func<KvPoint, bool>> pointsCondition = data =>
                data.CreatedAtUtc >= startOfWeek && data.CreatedAtUtc <= endOfWeek;


            // Get the sum of points earned this month.
            var outcome = await _repository
                .GetListAsync<KvPoint>(pointsCondition);

            int sum = outcome.Sum(kvPoint => kvPoint.Point);


            //    await _distributedCache.SetAsync(cacheKey, sum);
            //}

            return sum;
        }

        public async Task<decimal> TotalPointsEarnTodayCount()
        {
            //string cacheKey = DashboardCacheKeys.TotalPointsEarnTodayCountKey;
            //decimal sum = await _distributedCache.GetAsync<decimal>(cacheKey);

            //if (sum == 0)
            //{
            // Calculate the start and end dates for the current day.
            DateTime currentDate = DateTime.UtcNow;
            DateTime startOfDay = currentDate.Date;
            DateTime endOfDay = startOfDay.AddDays(1).AddSeconds(-1);

            Expression<Func<KvPoint, bool>> pointsCondition = data =>
                data.CreatedAtUtc >= startOfDay && data.CreatedAtUtc <= endOfDay;


            // Get the sum of points earned this month.
            var outcome = await _repository
                .GetListAsync<KvPoint>(pointsCondition);

            int sum = outcome.Sum(kvPoint => kvPoint.Point);


            //    await _distributedCache.SetAsync(cacheKey, sum);
            //}

            return sum;
        }

        public async Task<int> TotalUsersCount()
        {
            //string cacheKey = DashboardCacheKeys.TotalUsersCountKey;
            //int sum = await _distributedCache.GetAsync<int>(cacheKey);
            //int 
            //if (sum == 0)
            //{
            int sum = await _userManager.Users.CountAsync();

            //    await _distributedCache.SetAsync(cacheKey, sum);
            //}

            return sum;
        }

        public async Task<int> TotalUsersPostTodayCount()
        {
            //string cacheKey = DashboardCacheKeys.TotalActiveUsersCountKey;
            //int count = await _distributedCache.GetAsync<int>(cacheKey);

            //if (count == 0)
            //{
            // Calculate the start and end dates for the current day.
            DateTime currentDate = DateTime.UtcNow;
            DateTime startOfDay = currentDate.Date;
            DateTime endOfDay = startOfDay.AddDays(1).AddSeconds(-1);

            Expression<Func<IdentityKvAd, bool>> pointsCondition = data =>
                data.Active == true;

            // Get the Ids of users who meet the kvAdCondition criteria.
            var user = await _repository.GetListAsync<IdentityKvAd>(pointsCondition);
            var userIds = user.Select(x => x.UserId);


            // Check if there are users with these Ids.
            int count = await _userManager.Users
                 .Where(user => userIds.Contains(user.Id))
                 .CountAsync();

            //    await _distributedCache.SetAsync(cacheKey, count);
            //}

            return count;
        }
    }
}
