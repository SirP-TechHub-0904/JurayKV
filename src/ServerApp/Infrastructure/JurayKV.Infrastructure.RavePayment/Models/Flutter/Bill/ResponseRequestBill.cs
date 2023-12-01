using System;
using System.Collections.Generic;
using System.Text;

namespace JurayKV.Infrastructure.RavePayment.Models.Flutter.Bill
{


    public class ResponseRequestBill
    {
        public string status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string phone_number { get; set; }
        public int amount { get; set; }
        public string network { get; set; }
        public string flw_ref { get; set; }
        public string reference { get; set; }
    }

}
