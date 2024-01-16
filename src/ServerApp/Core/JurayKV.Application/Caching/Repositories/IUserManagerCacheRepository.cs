using JurayKV.Application.Queries.BucketQueries;
using JurayKV.Application.Queries.UserAccountQueries.DashboardQueries;
using JurayKV.Application.Queries.UserManagerQueries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;
using static JurayKV.Domain.Primitives.Enum;

namespace JurayKV.Application.Caching.Repositories
{
    [ScopedService]
    public interface IUserManagerCacheRepository
    {
        Task<List<UserManagerListDto>> GetListAsync();
        Task<UserManagerDetailsDto> GetReferralInfoByPhoneAsync(string phone);
        Task<List<UserManagerListDto>> GetListByStatusAsync(AccountStatus status);
        Task<UserManagerDetailsDto> GetByIdAsync(Guid modelId);
        Task<UserDashboardDto> GetUserDashboardDto(Guid userId, CancellationToken cancellationToken);
        Task<List<UserManagerListDto>> GetListReferralAsync(string myphone);
    }
}
