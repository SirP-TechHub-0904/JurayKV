using System;
using System.Collections.Generic;
using System.Text;

namespace JurayKV.Infrastructure.RavePayment.Models.Flutter.BankInfo
{
    public class BankInformation
    {
        public string status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {

        public string account_number { get; set; }
        public string account_name { get; set; }

    }



}
