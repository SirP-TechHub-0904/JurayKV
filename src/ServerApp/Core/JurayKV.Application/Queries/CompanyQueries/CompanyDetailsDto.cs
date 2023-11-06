using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JurayKV.Application.Queries.CompanyQueries
{
    public class CompanyDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
