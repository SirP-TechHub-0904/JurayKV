using JurayKV.Application.Caching.Repositories;
using JurayKV.Application.Queries.KvAdQueries;
using JurayKV.Domain.Aggregates.KvAdAggregate;
using JurayKV.Persistence.Cache.Keys;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TanvirArjel.EFCore.GenericRepository;
using TanvirArjel.Extensions.Microsoft.Caching;

namespace JurayKV.Persistence.Cache.Repositories
{
        public sealed class KvAdCacheRepository : IKvAdCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IQueryRepository _repository;
        private readonly IKvAdRepository _kvAdRepository;
        public KvAdCacheRepository(IDistributedCache distributedCache, IQueryRepository repository, IKvAdRepository kvAdRepository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
            _kvAdRepository = kvAdRepository;
        }

        public async Task<List<KvAdListDto>> GetListAsync()
        {
            string cacheKey = KvAdCacheKeys.ListKey;
            List<KvAdListDto> list = await _distributedCache.GetAsync<List<KvAdListDto>>(cacheKey);

            if (list == null)
            {
                Expression<Func<KvAd, KvAdListDto>> selectExp = d => new KvAdListDto
                {
                    Id = d.Id, 
                    CreatedAtUtc = d.CreatedAtUtc,
                    BucketId = d.BucketId,
                    BucketName = d.Bucket.Name,
                    CompanyName = d.Company.Name,
                    CompanyId = d.CompanyId,
                    ImageUrl = d.ImageUrl,
                    UserId = d.UserId,
                    Status = d.Status,
                };

                list = await _repository.GetListAsync(selectExp);

                await _distributedCache.SetAsync(cacheKey, list);
            }

            return list;
        }

        public async Task<KvAdDetailsDto> GetByIdAsync(Guid bucketId)
        {
            string cacheKey = KvAdCacheKeys.GetKey(bucketId);
            KvAdDetailsDto bucket = await _distributedCache.GetAsync<KvAdDetailsDto>(cacheKey);

            if (bucket == null)
            {
                Expression<Func<KvAd, KvAdDetailsDto>> selectExp = d => new KvAdDetailsDto
                {
                    Id = d.Id,
                    CreatedAtUtc = d.CreatedAtUtc,
                    BucketId = d.BucketId,
                    CompanyId = d.CompanyId,
                    ImageUrl = d.ImageUrl,
                    UserId = d.UserId,
                    Status = d.Status,
                };

                bucket = await _repository.GetByIdAsync(bucketId, selectExp);

                await _distributedCache.SetAsync(cacheKey, bucket);
            }

            return bucket;
        }

        public async Task<KvAdDetailsDto> GetDetailsByIdAsync(Guid bucketId)
        {
            string cacheKey = KvAdCacheKeys.GetDetailsKey(bucketId);
            KvAdDetailsDto bucket = await _distributedCache.GetAsync<KvAdDetailsDto>(cacheKey);

            if (bucket == null)
            {
                Expression<Func<KvAd, KvAdDetailsDto>> selectExp = d => new KvAdDetailsDto
                {
                    Id = d.Id, 
                    CreatedAtUtc = d.CreatedAtUtc,
                    BucketId = d.BucketId,
                    CompanyId = d.CompanyId,
                    ImageUrl = d.ImageUrl,
                    UserId = d.UserId,
                    Status = d.Status,
                };

                bucket = await _repository.GetByIdAsync(bucketId, selectExp);

                await _distributedCache.SetAsync(cacheKey, bucket);
            }

            return bucket;
        }

        public async Task<List<KvAdListDto>> GetListByBucketIdAsync(Guid bucketId)
        {

            string cacheKey = KvAdCacheKeys.ListByBucketIdKey(bucketId);
            List<KvAdListDto> list = await _distributedCache.GetAsync<List<KvAdListDto>>(cacheKey);

            if (list == null)
            { 
               var mainlist = await _kvAdRepository.AdsByBucketId(bucketId);
                list = mainlist.Select(d => new KvAdListDto
                {
                    Id = d.Id,
                    CreatedAtUtc = d.CreatedAtUtc,
                    BucketId = d.BucketId,
                    BucketName = d.Bucket.Name,
                    CompanyName = d.Company.Name,
                    CompanyId = d.CompanyId,
                    ImageUrl = d.ImageUrl,
                    UserId = d.UserId,
                    Status = d.Status,
                }).ToList();

                await _distributedCache.SetAsync(cacheKey, list);
            }

            return list;
        }
    }

}
