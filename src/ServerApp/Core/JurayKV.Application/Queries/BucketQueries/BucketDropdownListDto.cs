﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JurayKV.Application.Queries.BucketQueries
{
    public class BucketDropdownListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Disable {  get; set; }
    }
}
