using JurayKV.Application.Caching.Repositories;
using JurayKV.Application.Queries.IdentityKvAdQueries;
using JurayKV.Domain.Aggregates.IdentityKvAdAggregate;
using JurayKV.Persistence.Cache.Keys;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TanvirArjel.EFCore.GenericRepository;
using TanvirArjel.Extensions.Microsoft.Caching;

namespace JurayKV.Persistence.Cache.Repositories
{
    public sealed class IdentityKvAdCacheRepository : IIdentityKvAdCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IQueryRepository _repository;
        private readonly IIdentityKvAdRepository _adRepository;
        public IdentityKvAdCacheRepository(IDistributedCache distributedCache, IQueryRepository repository, IIdentityKvAdRepository adRepository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
            _adRepository = adRepository;
        }

        public async Task<List<IdentityKvAdListDto>> GetListAsync()
        {
            string cacheKey = IdentityKvAdCacheKeys.ListKey;
            List<IdentityKvAdListDto> list = await _distributedCache.GetAsync<List<IdentityKvAdListDto>>(cacheKey);

            if (list == null)
            {
                Expression<Func<IdentityKvAd, IdentityKvAdListDto>> selectExp = d => new IdentityKvAdListDto
                {
                    Id = d.Id,
                    Activity = d.Activity,
                    CreatedAtUtc = d.CreatedAtUtc,
                    KvAdId = d.KvAdId,
                    LastModifiedAtUtc = d.LastModifiedAtUtc,
                    UserId = d.UserId,
                    VideoUrl = d.VideoUrl,
                    Active = d.Active,
                    KvAdName = d.KvAd.Bucket.Name,
                    ImageUrl = d.KvAd.ImageUrl,
                    Fullname = d.User.SurName + " " +d.User.FirstName + " " + d.User.LastName
                };

                list = await _repository.GetListAsync(selectExp);

                await _distributedCache.SetAsync(cacheKey, list);
            }

            return list;
        }

        public async Task<IdentityKvAdDetailsDto> GetByIdAsync(Guid identityKvAdId)
        {
            string cacheKey = IdentityKvAdCacheKeys.GetKey(identityKvAdId);
            IdentityKvAdDetailsDto identityKvAd = await _distributedCache.GetAsync<IdentityKvAdDetailsDto>(cacheKey);

            if (identityKvAd == null)
            {
 
                var mainidentityKvAd = await _adRepository.GetByIdAsync(identityKvAdId);
                identityKvAd = new IdentityKvAdDetailsDto
                {
                    Id = mainidentityKvAd.Id,
                    Activity = mainidentityKvAd.Activity,
                    KvAdId = mainidentityKvAd.KvAdId,
                    UserId = mainidentityKvAd.UserId,
                    VideoUrl = mainidentityKvAd.VideoUrl,
                    CreatedAtUtc = mainidentityKvAd.CreatedAtUtc,
                    LastModifiedAtUtc = mainidentityKvAd.LastModifiedAtUtc,
                    Active = mainidentityKvAd.Active,
                    KvAdImage = mainidentityKvAd.KvAd.ImageUrl,
                    Fullname = mainidentityKvAd.User.SurName + " " + mainidentityKvAd.User.FirstName + " " + mainidentityKvAd.User.LastName
                };
                await _distributedCache.SetAsync(cacheKey, identityKvAd);
            }

            return identityKvAd;
        }

        public async Task<IdentityKvAdDetailsDto> GetDetailsByIdAsync(Guid identityKvAdId)
        {
            string cacheKey = IdentityKvAdCacheKeys.GetDetailsKey(identityKvAdId);
            IdentityKvAdDetailsDto identityKvAd = await _distributedCache.GetAsync<IdentityKvAdDetailsDto>(cacheKey);

            if (identityKvAd == null)
            {
                Expression<Func<IdentityKvAd, IdentityKvAdDetailsDto>> selectExp = d => new IdentityKvAdDetailsDto
                {
                    Id = d.Id,
                    Activity = d.Activity,
                    CreatedAtUtc = d.CreatedAtUtc,
                    KvAdId = d.KvAdId,
                    LastModifiedAtUtc = d.LastModifiedAtUtc,
                    UserId = d.UserId,
                    VideoUrl = d.VideoUrl,
                    Active = d.Active,
                };

                identityKvAd = await _repository.GetByIdAsync(identityKvAdId, selectExp);

                await _distributedCache.SetAsync(cacheKey, identityKvAd);
            }

            return identityKvAd;
        }

        public async Task<List<IdentityKvAdListDto>> GetByUserIdAsync(Guid userId)
        {
            string cacheKey = IdentityKvAdCacheKeys.GetByUserIdKey(userId);
            List<IdentityKvAdListDto> list = await _distributedCache.GetAsync<List<IdentityKvAdListDto>>(cacheKey);

            if (list == null)
            {
                 

              var mainlist = await _adRepository.GetListByUserId(userId);
               list = mainlist.Select(d => new IdentityKvAdListDto
{
                   Id = d.Id,
                   Activity = d.Activity,
                   CreatedAtUtc = d.CreatedAtUtc,
                   KvAdId = d.KvAdId,
                   LastModifiedAtUtc = d.LastModifiedAtUtc,
                   UserId = d.UserId,
                   VideoUrl = d.VideoUrl,
                   Active = d.Active,
                   ImageUrl = d.KvAd.ImageUrl
               }).ToList();
                await _distributedCache.SetAsync(cacheKey, list);
            }

            return list;
        }

        public async Task<List<IdentityKvAdListDto>> GetActiveByUserIdAsync(Guid userId)
        {
            string cacheKey = IdentityKvAdCacheKeys.GetActiveByUserIdKey(userId);
            List<IdentityKvAdListDto> list = await _distributedCache.GetAsync<List<IdentityKvAdListDto>>(cacheKey);

            if (list == null)
            {

                var mainlist = await _adRepository.GetActiveListByUserId(userId);
                list = mainlist.Select(d => new IdentityKvAdListDto
                {
                    Id = d.Id,
                    Activity = d.Activity,
                    CreatedAtUtc = d.CreatedAtUtc,
                    KvAdId = d.KvAdId,
                    LastModifiedAtUtc = d.LastModifiedAtUtc,
                    UserId = d.UserId,
                    VideoUrl = d.VideoUrl,
                    Active = d.Active,
                    ImageUrl = d.KvAd.ImageUrl
                }).ToList();
                await _distributedCache.SetAsync(cacheKey, list);
            }

            return list;
        }
    }

}
