using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JurayKV.Domain.Primitives.Enum;

namespace JurayKV.Application.Queries.SettingQueries
{
    public class SettingListDto
    {
        public Guid Id { get; set; }
        [Display(Name = "Default Amount Per View")]
        public decimal DefaultAmountPerView { get; set; }

        [Display(Name = "Minimum Amount Budget")]
        public decimal MinimumAmountBudget { get; set; }

        public PaymentGateway PaymentGateway { get; set; }
        public BillGateway BillGateway { get; set; }

    }
}
