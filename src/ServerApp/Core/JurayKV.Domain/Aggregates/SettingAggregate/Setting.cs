using JurayKV.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JurayKV.Domain.Aggregates.SettingAggregate
{
      public sealed class Setting : AggregateRoot
    {
        public Setting(Guid id)
        {
            Id = id;
            
        }

        // This is needed for EF Core query mapping or deserialization.
        public Setting()
        {
        }

        public decimal DefaultAmountPerView { get; set; }
        public decimal MinimumAmountBudget { get; set; }
         
    }
}
