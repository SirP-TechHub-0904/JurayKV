﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JurayKV.Application.Caching.Handlers
{
    public interface IBucketCacheHandler
    {
        Task RemoveDetailsByIdAsync(Guid bucketId);
        Task RemoveGetByIdAsync(Guid bucketId);
        Task RemoveListAsync();
        Task RemoveDropdownListAsync();
    }
}
