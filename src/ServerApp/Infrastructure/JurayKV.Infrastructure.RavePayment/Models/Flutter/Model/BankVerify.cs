using System;
using System.Collections.Generic;
using System.Text;

namespace JurayKV.Infrastructure.RavePayment.Models.Flutter.Model
{
    public class BankVerify
    {
        public string status { get; set; }
        public string message { get; set; }
        public ICollection<Data> data { get; set; }
    }

    public class Data
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }

    }



}
