using JurayKV.Application.Queries.KvAdQueries;
using JurayKV.Domain.Aggregates.KvAdAggregate;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace JurayKV.Application.Caching.Repositories
{
    [ScopedService]
    public interface IKvAdCacheRepository
    {
        Task<List<KvAdListDto>> GetListAsync();
        Task<List<KvAdListDto>> GetListByBucketIdAsync(Guid bucketId);
        Task<KvAdDetailsDto> GetByActiveAsync(Guid bucketId);
        Task<List<KvAdDetailsDto>> GetByActiveAsync();
        Task<KvAdDetailsDto> GetByIdAsync(Guid modelId);

        Task<KvAdDetailsDto> GetDetailsByIdAsync(Guid modelId);
        Task<List<KvAdListDto>> GetActiveListAllBucketAsync(DateTime date);
        Task ClearAllActive();

        Task MakeActiveAsync(Guid id, Guid bucketId, bool active);
    }
}
