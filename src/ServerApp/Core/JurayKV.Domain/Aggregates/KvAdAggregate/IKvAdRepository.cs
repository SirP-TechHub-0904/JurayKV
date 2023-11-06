using JurayKV.Domain.Aggregates.KvAdAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JurayKV.Domain.Aggregates.KvAdAggregate
{
    public interface IKvAdRepository
    {
        Task<KvAd> GetByIdAsync(Guid identityKvAdId);

        Task InsertAsync(KvAd identityKvAd);

        Task UpdateAsync(KvAd identityKvAd);

        Task DeleteAsync(KvAd identityKvAd);

        Task<List<KvAd>> AdsByBucketId(Guid bucketId);
    }
}
