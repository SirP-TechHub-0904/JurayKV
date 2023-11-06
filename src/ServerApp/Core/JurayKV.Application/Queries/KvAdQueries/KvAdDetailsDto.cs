﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JurayKV.Domain.Primitives.Enum;

namespace JurayKV.Application.Queries.KvAdQueries
{
    public class KvAdDetailsDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Fullname { get; set; }
        public Guid BucketId { get; set; }
        public string BucketName { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public string ImageUrl { get; set; }
        public DataStatus Status { get; set; }

    }
}
