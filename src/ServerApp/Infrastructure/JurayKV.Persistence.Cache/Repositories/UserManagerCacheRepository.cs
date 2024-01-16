using JurayKV.Application.Caching.Repositories;
using JurayKV.Application.Queries.ExchangeRateQueries;
using JurayKV.Application.Queries.IdentityKvAdQueries;
using JurayKV.Application.Queries.KvPointQueries;
using JurayKV.Application.Queries.TransactionQueries;
using JurayKV.Application.Queries.UserAccountQueries.DashboardQueries;
using JurayKV.Application.Queries.UserManagerQueries;
using JurayKV.Application.Queries.WalletQueries;
using JurayKV.Domain.Aggregates.EmployeeAggregate;
using JurayKV.Domain.Aggregates.IdentityAggregate;
using JurayKV.Domain.ValueObjects;
using JurayKV.Persistence.Cache.Keys;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using TanvirArjel.EFCore.GenericRepository;
using TanvirArjel.Extensions.Microsoft.Caching;
using static JurayKV.Domain.Primitives.Enum;

namespace JurayKV.Persistence.Cache.Repositories
{
    public sealed class UserManagerCacheRepository : IUserManagerCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IQueryRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserManagerCacheRepository(IDistributedCache distributedCache, IQueryRepository repository, UserManager<ApplicationUser> userManager)
        {
            _distributedCache = distributedCache;
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<List<UserManagerListDto>> GetListReferralAsync(string myphone)
        {
            string last10DigitsPhoneNumber1 = myphone.Substring(Math.Max(0, myphone.Length - 10));
            var userlist = await _userManager.Users.Where(x => x.RefferedByPhoneNumber.Contains(last10DigitsPhoneNumber1)).ToListAsync();
            // Manual mapping from entities to DTOs
            var list = userlist.Select(entity => new UserManagerListDto
            {
                Id = entity.Id,
                Date = entity.CreationUTC,
                Fullname = entity.SurName + " " + entity.FirstName + " " + entity.LastName,
                AccountStatus = entity.AccountStatus,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                LastLoggedInAtUtc = entity.LastLoggedInAtUtc,
                CreationUTC = entity.CreationUTC,
                Verified = entity.EmailConfirmed
            }).ToList();

            return list;
        }
        public async Task<List<UserManagerListDto>> GetListByStatusAsync(AccountStatus status)
        {
            var userlist = new List<ApplicationUser>();
            if (status == AccountStatus.NotDefind)
            {
                userlist = await _userManager.Users.ToListAsync();

            }
            else
            {
                userlist = await _userManager.Users.Where(x => x.AccountStatus == status).ToListAsync();

            }
            var list = userlist.Select(entity => new UserManagerListDto
            {
                Id = entity.Id,
                Date = entity.CreationUTC,
                Fullname = entity.SurName + " " + entity.FirstName + " " + entity.LastName,
                AccountStatus = entity.AccountStatus,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                LastLoggedInAtUtc = entity.LastLoggedInAtUtc,
                CreationUTC = entity.CreationUTC,
                Verified = entity.EmailConfirmed,
                VerificationCode = entity.VerificationCode,
                Role = entity.Role
            });

            return list.ToList();
        }
        public async Task<List<UserManagerListDto>> GetListAsync()
        {
            //string cacheKey = UserManagerCacheKeys.ListKey;
            //List<UserManagerListDto> list = await _distributedCache.GetAsync<List<UserManagerListDto>>(cacheKey);

            //if (list == null)
            //{
            var userlist = await _userManager.Users.ToListAsync();
            // Manual mapping from entities to DTOs
            var list = userlist.Select(entity => new UserManagerListDto
            {
                Id = entity.Id,
                Date = entity.CreationUTC,
                Fullname = entity.SurName + " " + entity.FirstName + " " + entity.LastName,
                AccountStatus = entity.AccountStatus,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                LastLoggedInAtUtc = entity.LastLoggedInAtUtc,
                CreationUTC = entity.CreationUTC,
                Verified = entity.EmailConfirmed,
                VerificationCode = entity.VerificationCode
                // Map other properties as needed
            }).ToList();
            //    await _distributedCache.SetAsync(cacheKey, list);
            //}

            return list;
        }
        public async Task<UserManagerDetailsDto> GetReferralInfoByPhoneAsync(string phone)
        {
            try
            {
                UserManagerDetailsDto userManager = new UserManagerDetailsDto();
                if (phone != null)
                {
                    string last10DigitsPhoneNumber1 = phone.Substring(Math.Max(0, phone.Length - 10));
                    var entity = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber.Contains(last10DigitsPhoneNumber1));
                    if (entity != null)
                    {
                        userManager = new UserManagerDetailsDto
                        {
                            Id = entity.Id,

                            Fullname = entity.SurName + " " + entity.FirstName + " " + entity.LastName,

                            Email = entity.Email,
                            PhoneNumber = entity.PhoneNumber,

                        };
                    }
                }
                //    await _distributedCache.SetAsync(cacheKey, userManager);
                //}

                return userManager;
            }
            catch (Exception a)
            {
                return null;
            }
        }

        public async Task<UserManagerDetailsDto> GetByIdAsync(Guid userManagerId)
        {
            //string cacheKey = UserManagerCacheKeys.GetDetailsKey(userManagerId);
            //UserManagerDetailsDto userManager = await _distributedCache.GetAsync<UserManagerDetailsDto>(cacheKey);
            UserManagerDetailsDto userManager = new UserManagerDetailsDto();

            //if (userManager == null)
            //{
            var entity = await _userManager.FindByIdAsync(userManagerId.ToString());
            if (entity != null)
            {
                userManager = new UserManagerDetailsDto
                {
                    Id = entity.Id,
                    CreationUTC = entity.CreationUTC,
                    Fullname = entity.SurName + " " + entity.FirstName + " " + entity.LastName,
                    AccountStatus = entity.AccountStatus,
                    Email = entity.Email,
                    PhoneNumber = entity.PhoneNumber,
                    LastLoggedInAtUtc = entity.LastLoggedInAtUtc,
                    Surname = entity.SurName,
                    Firstname = entity.FirstName,
                    Lastname = entity.LastName,
                    RefferedBy = entity.RefferedByPhoneNumber,
                    IsCompany = await _userManager.IsInRoleAsync(entity, "Company"),
                    IsCSARole = await _userManager.IsInRoleAsync(entity, "CSA"),

                    Tier = entity.Tier,
                    DateTie2Upgraded = entity.DateTie2Upgraded,
                    About = entity.About,
                    AlternativePhone = entity.AlternativePhone,
                    Address = entity.Address,
                    State = entity.State,
                    LGA = entity.LGA,
                    Occupation = entity.Occupation,
                    FbHandle = entity.FbHandle,
                    InstagramHandle = entity.InstagramHandle,
                    TwitterHandle = entity.TwitterHandle,
                    TiktokHandle = entity.TiktokHandle,
                    IDCardKey = entity.IDCardKey,
                    IDCardUrl = entity.IDCardUrl,
                    PassportUrl = entity.PassportUrl,
                    PassportKey = entity.PassportKey,
                    AccountName = entity.AccountName,
                    AccountNumber = entity.AccountNumber,
                    BankName = entity.BankName,
                    BVN = entity.BVN,
                    DateUpgraded = entity.DateUpgraded,
                    ResponseOnCsaRequest = entity.ResponseOnCsaRequest,
                    CsaRequest = entity.CsaRequest,
                };
            }

            //    await _distributedCache.SetAsync(cacheKey, userManager);
            //}

            return userManager;
        }

        public async Task<UserManagerDetailsDto> GetDetailsByIdAsync(Guid userManagerId)
        {
            //string cacheKey = UserManagerCacheKeys.GetDetailsKey(userManagerId);
            //UserManagerDetailsDto userManager = await _distributedCache.GetAsync<UserManagerDetailsDto>(cacheKey);
            UserManagerDetailsDto userManager = new UserManagerDetailsDto();
            //if (userManager == null)
            //{
            var entity = await _userManager.FindByIdAsync(userManagerId.ToString());
            if (entity != null)
            {
                userManager = new UserManagerDetailsDto
                {
                    Id = entity.Id,
                    CreationUTC = entity.CreationUTC,
                    Fullname = entity.SurName + " " + entity.FirstName + " " + entity.LastName,
                    AccountStatus = entity.AccountStatus,
                    Email = entity.Email,
                    PhoneNumber = entity.PhoneNumber,
                    LastLoggedInAtUtc = entity.LastLoggedInAtUtc,
                };
            }

            //    await _distributedCache.SetAsync(cacheKey, userManager);
            //}

            return userManager;
        }

        public async Task<UserDashboardDto> GetUserDashboardDto(Guid userId, CancellationToken cancellationToken)
        {
            var getuser = await _userManager.FindByIdAsync(userId.ToString());

            //
            Specification<KvPointListDto> kvpointspec = new Specification<KvPointListDto>();
            kvpointspec.Conditions.Add(x => x.UserId == userId);
            kvpointspec.Take = 10;
            List<KvPointListDto> pointlist = await _repository.GetListAsync<KvPointListDto>(kvpointspec, cancellationToken);

            //
            Specification<TransactionListDto> transspec = new Specification<TransactionListDto>();
            transspec.Conditions.Add(x => x.UserId == userId);
            transspec.Take = 10;
            List<TransactionListDto> translist = await _repository.GetListAsync<TransactionListDto>(transspec, cancellationToken);

            //

            Specification<IdentityKvAdListDto> useradsspec = new Specification<IdentityKvAdListDto>();
            useradsspec.Conditions.Add(x => x.UserId == userId && x.Active == true);

            int count = await _repository.GetCountAsync<IdentityKvAdListDto>(useradsspec.Conditions, cancellationToken);
            //

            Specification<WalletDetailsDto> walletspec = new Specification<WalletDetailsDto>();
            walletspec.Conditions.Add(x => x.UserId == userId);
            WalletDetailsDto UserWallet = await _repository.GetAsync<WalletDetailsDto>(walletspec, cancellationToken);
            //
            //Specification<ExchangeRateDetailsDto> ratespec = new Specification<ExchangeRateDetailsDto>();
            //ratespec.Conditions.OrderByDescending).;
            //WalletDetailsDto UserWallet = await _repository.GetAsync<WalletDetailsDto>(walletspec, cancellationToken);

            return null;
        }
    }

}
