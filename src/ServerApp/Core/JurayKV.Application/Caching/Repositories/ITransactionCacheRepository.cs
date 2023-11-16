using JurayKV.Application.Queries.KvPointQueries;
using JurayKV.Application.Queries.TransactionQueries;
using JurayKV.Domain.Aggregates.TransactionAggregate;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace JurayKV.Application.Caching.Repositories
{
    [ScopedService]
    public interface ITransactionCacheRepository
    {
        Task<List<TransactionListDto>> GetListAsync();
        Task<List<TransactionListDto>> GetListByCountAsync(int latestcount);

        Task<TransactionDetailsDto> GetByIdAsync(Guid modelId);

        Task<TransactionDetailsDto> GetDetailsByIdAsync(Guid modelId);

        Task<List<TransactionListDto>> GetListByCountAsync(int toplistcount, Guid userId);

        Task<int> TransactionCount(Guid userId);

    }
}
