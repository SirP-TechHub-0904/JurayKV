using JurayKV.Domain.Aggregates.TransactionAggregate;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TanvirArjel.ArgumentChecker;
using JurayKV.Domain.Aggregates.NotificationAggregate;
using JurayKV.Persistence.RelationalDB.Repositories.GenericRepositories;

namespace JurayKV.Persistence.RelationalDB.Repositories
{
    internal sealed class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        private readonly JurayDbContext _dbContext;

        public TransactionRepository(JurayDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<bool> ExistsAsync(Expression<Func<Transaction, bool>> condition)
        {
            IQueryable<Transaction> queryable = _dbContext.Set<Transaction>();

            if (condition != null)
            {
                queryable = queryable.Where(condition);
            }

            return queryable.AnyAsync();
        }

        public async Task<Transaction> GetByIdAsync(Guid transactionId)
        {
            transactionId.ThrowIfEmpty(nameof(transactionId));

            Transaction transaction = await _dbContext.Set<Transaction>().FindAsync(transactionId);
            return transaction;
        }

        public async Task InsertAsync(Transaction transaction)
        {
            transaction.ThrowIfNull(nameof(transaction));

            await _dbContext.AddAsync(transaction);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Guid> InsertReturnIdAsync(Transaction transaction)
        {
            transaction.ThrowIfNull(nameof(transaction));

            await _dbContext.AddAsync(transaction);
            await _dbContext.SaveChangesAsync();
            return transaction.Id;
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            transaction.ThrowIfNull(nameof(transaction));

            EntityEntry<Transaction> trackedEntity = _dbContext.ChangeTracker.Entries<Transaction>()
                .FirstOrDefault(x => x.Entity == transaction);

            if (trackedEntity == null)
            {
                _dbContext.Update(transaction);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Transaction transaction)
        {
            transaction.ThrowIfNull(nameof(transaction));

            _dbContext.Remove(transaction);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<bool> CheckTransactionAboveTieOne(string uniqueId, Guid userId)
        {
            var setting = await _dbContext.Settings.FirstOrDefaultAsync();
            var checktransaction = await _dbContext.Transactions.Where(x=>x.UniqueReference.Contains(uniqueId) && x.UserId == userId).SumAsync(x=>x.Amount);
            if(checktransaction > setting.AirtimeMaxRechargeTieOne)
            {
                return true;
            }
            return false;
        }
        public async Task<List<Transaction>> LastListByCountByUserId(int toplistcount, Guid userId)
        {
            var list = await _dbContext.Transactions
                .Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedAtUtc)
                .Take(toplistcount).ToListAsync();
            return list;
        }

        public async Task<int> TransactionCount(Guid userId)
        {
           return await _dbContext.Transactions.Where(x=>x.UserId == userId).CountAsync();
        }

        public async Task<List<Transaction>> GetListByUserId(Guid userId)
        {
            var list = await _dbContext.Transactions
                .Include(x=>x.User)
                .Include(x=>x.Wallet)
                 .Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedAtUtc)
                  .ToListAsync();
            return list;
        }

        public async Task<List<Transaction>> GetReferralListByUserId(Guid userId)
        {
            var list = await _dbContext.Transactions
                .Include(x => x.User)
                .Include(x => x.Wallet)
                 .Where(x => x.UserId == userId && x.Description.ToLower().Contains("Referral Bonus")).OrderByDescending(x => x.CreatedAtUtc)
                  .ToListAsync();
            return list;
        }
    }

}
