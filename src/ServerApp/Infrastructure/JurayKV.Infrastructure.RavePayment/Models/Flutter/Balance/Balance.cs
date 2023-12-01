using System;
using System.Collections.Generic;
using System.Text;

namespace JurayKV.Infrastructure.RavePayment.Models.Flutter.Balance
{
    public class Balance
    {
        public string status { get; set; }
        public string message { get; set; }
        public ICollection<Data> data { get; set; }
    }

    public class Data
    {
        public string currency { get; set; }
        public decimal available_balance { get; set; }
        public decimal ledger_balance { get; set; }

    }
}