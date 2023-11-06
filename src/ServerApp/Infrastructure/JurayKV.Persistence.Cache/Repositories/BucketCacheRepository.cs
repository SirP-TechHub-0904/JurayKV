using JurayKV.Application.Caching.Repositories;
using JurayKV.Application.Queries.BucketQueries;
using JurayKV.Domain.Aggregates.BucketAggregate;
using JurayKV.Persistence.Cache.Keys;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TanvirArjel.EFCore.GenericRepository;
using TanvirArjel.Extensions.Microsoft.Caching;

namespace JurayKV.Persistence.Cache.Repositories
{
    public sealed class BucketCacheRepository : IBucketCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IQueryRepository _repository;

        public BucketCacheRepository(IDistributedCache distributedCache, IQueryRepository repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public async Task<List<BucketListDto>> GetListAsync()
        {
            string cacheKey = BucketCacheKeys.ListKey;
            List<BucketListDto> list = await _distributedCache.GetAsync<List<BucketListDto>>(cacheKey);

            if (list == null)
            {
                Expression<Func<Bucket, BucketListDto>> selectExp = d => new BucketListDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Date = d.CreatedAtUtc,
                    AdminActive = d.AdminActive,
                    Disable = d.Disable,
                    UserActive = d.UserActive,
                };

                list = await _repository.GetListAsync(selectExp);

                await _distributedCache.SetAsync(cacheKey, list);
            }

            return list;
        }

        public async Task<BucketDetailsDto> GetByIdAsync(Guid bucketId)
        {
            string cacheKey = BucketCacheKeys.GetKey(bucketId);
            BucketDetailsDto bucket = await _distributedCache.GetAsync<BucketDetailsDto>(cacheKey);
            if (bucket == null)
            {
                Expression<Func<Bucket, BucketDetailsDto>> selectExp = d => new BucketDetailsDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Date = d.CreatedAtUtc,
                    AdminActive = d.AdminActive,
                    Disable = d.Disable,
                    UserActive = d.UserActive
                };

                bucket = await _repository.GetByIdAsync(bucketId, selectExp);

                await _distributedCache.SetAsync(cacheKey, bucket);
            }

            return bucket;
             
        }

        public async Task<BucketDetailsDto> GetDetailsByIdAsync(Guid bucketId)
        {
            string cacheKey = BucketCacheKeys.GetDetailsKey(bucketId);
            BucketDetailsDto bucket = await _distributedCache.GetAsync<BucketDetailsDto>(cacheKey);

            if (bucket == null)
            {
                Expression<Func<Bucket, BucketDetailsDto>> selectExp = d => new BucketDetailsDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Date = d.CreatedAtUtc,
                    AdminActive = d.AdminActive,
                    Disable = d.Disable,
                    UserActive = d.UserActive
                };

                bucket = await _repository.GetByIdAsync(bucketId, selectExp);

                await _distributedCache.SetAsync(cacheKey, bucket);
            }

            return bucket;
        }

        public async Task<List<BucketDropdownListDto>> GetDropdownListAsync()
        {
            string cacheKey = BucketCacheKeys.SelectListKey;
            List<BucketDropdownListDto> list = await _distributedCache.GetAsync<List<BucketDropdownListDto>>(cacheKey);

            if (list == null)
            {
                Expression<Func<Bucket, BucketDropdownListDto>> selectExp = d => new BucketDropdownListDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    
                };

                list = await _repository.GetListAsync(selectExp);

                await _distributedCache.SetAsync(cacheKey, list);
            }

            return list;
        }
    }

}
