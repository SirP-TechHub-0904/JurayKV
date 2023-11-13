﻿using JurayKV.Domain.Aggregates.IdentityKvAdAggregate;
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

        Task<Guid> InsertAsync(IdentityKvAd identityKvAd);

        Task UpdateAsync(IdentityKvAd identityKvAd);

        Task DeleteAsync(IdentityKvAd identityKvAd);

        Task<List<IdentityKvAd>> GetListByUserId(Guid userId);
        Task<IQueryable<IdentityKvAd>> GetActiveListByUserId(Guid userId);
        Task<IQueryable<IdentityKvAd>> ListActiveToday();

        Task<bool> CheckExist(Guid userId, Guid kvAdId);
        Task<bool> CheckUserAdvertCountToday(Guid userId);
    }
}
