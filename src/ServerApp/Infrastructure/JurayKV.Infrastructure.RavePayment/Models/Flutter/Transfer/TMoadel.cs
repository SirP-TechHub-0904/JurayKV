using System;
using System.Collections.Generic;
using System.Text;

namespace JurayKV.Infrastructure.RavePayment.Models.Flutter.Transfer
{

    public class TModel
    {
        public string account_bank { get; set; }
        public string account_number { get; set; }
        public int amount { get; set; }
        public string narration { get; set; }
        public string currency { get; set; }
        public string reference { get; set; }
        public string callback_url { get; set; }
        public string debit_currency { get; set; }
    }

}
