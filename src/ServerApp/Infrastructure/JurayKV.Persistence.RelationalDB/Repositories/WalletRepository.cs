﻿using JurayKV.Domain.Aggregates.WalletAggregate;
using JurayKV.Persistence.RelationalDB.Repositories.GenericRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Threading.Tasks;
using TanvirArjel.ArgumentChecker;

namespace JurayKV.Persistence.RelationalDB.Repositories
{
    internal sealed class WalletRepository : GenericRepository<Wallet>, IWalletRepository
    {
        private readonly JurayDbContext _dbContext;

        public WalletRepository(JurayDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<bool> ExistsAsync(Expression<Func<Wallet, bool>> condition)
        {
            IQueryable<Wallet> queryable = _dbContext.Set<Wallet>();

            if (condition != null)
            {
                queryable = queryable.Where(condition);
            }

            return queryable.AnyAsync();
        }

        public async Task<Wallet> GetByIdAsync(Guid walletId)
        {
            walletId.ThrowIfEmpty(nameof(walletId));

            Wallet wallet = await _dbContext.Set<Wallet>().FindAsync(walletId);
            return wallet;
        }

        public async Task InsertAsync(Wallet wallet)
        {
            wallet.ThrowIfNull(nameof(wallet));

            await _dbContext.AddAsync(wallet);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Wallet wallet)
        {
            wallet.ThrowIfNull(nameof(wallet));

            EntityEntry<Wallet> trackedEntity = _dbContext.ChangeTracker.Entries<Wallet>()
                .FirstOrDefault(x => x.Entity == wallet);

            if (trackedEntity == null)
            {
                _dbContext.Update(wallet);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Wallet wallet)
        {
            wallet.ThrowIfNull(nameof(wallet));

            _dbContext.Remove(wallet);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Wallet> GetByUserIdAsync(Guid userId)
        {
            userId.ThrowIfEmpty(nameof(userId));

            Wallet wallet = await _dbContext.Set<Wallet>().FirstOrDefaultAsync(x => x.UserId == userId);
            return wallet;
        }

        public async Task<bool> CheckPhoneUnique(string phone)
        {
            var checkphone = await _dbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phone);
            if (checkphone == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
 

        public async Task<bool> CheckEmailUnique(string email)
        {
            var checkphone = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (checkphone == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

}
