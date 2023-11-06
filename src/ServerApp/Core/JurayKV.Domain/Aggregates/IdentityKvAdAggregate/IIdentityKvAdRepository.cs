using JurayKV.Domain.Aggregates.IdentityKvAdAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JurayKV.Domain.Aggregates.IdentityKvAdAggregate
{
    public interface IIdentityKvAdRepository
    {
        Task<IdentityKvAd> GetByIdAsync(Guid identityKvAdId);

        Task InsertAsync(IdentityKvAd identityKvAd);

        Task UpdateAsync(IdentityKvAd identityKvAd);

        Task DeleteAsync(IdentityKvAd identityKvAd);

        Task<IQueryable<IdentityKvAd>> GetListByUserId(Guid userId);
        Task<IQueryable<IdentityKvAd>> GetActiveListByUserId(Guid userId);
    }
}
