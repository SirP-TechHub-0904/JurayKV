﻿using JurayKV.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JurayKV.Domain.Aggregates.CompanyAggregate
{
    public sealed class Company :AggregateRoot
    {
        public Company(Guid id)
        {
            Id = id;
           
            CreatedAtUtc = DateTime.UtcNow;
        }

        // This is needed for EF Core query mapping or deserialization.
        public Company()
        {
        }

        public string Name { get; set; }

       
        public DateTime CreatedAtUtc { get; set; }

    }
}
