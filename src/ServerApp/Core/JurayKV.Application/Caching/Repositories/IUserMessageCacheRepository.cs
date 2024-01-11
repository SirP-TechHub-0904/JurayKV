using JurayKV.Application.Queries.BucketQueries;
using JurayKV.Application.Queries.UserMessageQueries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace JurayKV.Application.Caching.Repositories
{
    [ScopedService]
    public interface IUserMessageCacheRepository
    {
        Task<List<UserMessageDto>> GetListAsync();
        Task<List<UserMessageDto>> GetListByUserIdAsync(Guid userId);
        
    }
}
