﻿using JurayKV.Domain.Aggregates.TransactionAggregate;
using JurayKV.Domain.Aggregates.NotificationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JurayKV.Domain.Aggregates.KvPointAggregate;

namespace JurayKV.Domain.Aggregates.TransactionAggregate
{
    public interface ITransactionRepository
    {
        Task<Transaction> GetByIdAsync(Guid transactionId);

        Task InsertAsync(Transaction transaction);
        Task<Guid> InsertReturnIdAsync(Transaction transaction);
        Task UpdateAsync(Transaction transaction);

        Task DeleteAsync(Transaction transaction);

        Task<List<Transaction>> LastListByCountByUserId(int toplistcount, Guid userId);
        Task<List<Transaction>> GetListByUserId(Guid userId);
        Task<List<Transaction>> GetReferralListByUserId(Guid userId);
        Task<int> TransactionCount(Guid userId);

    }
}
