using System;
using System.Collections.Generic;
using System.Text;

namespace JurayKV.Infrastructure.RavePayment.Models.Flutter.Bill
{

    public class RequestBill
    {
        public string country { get; set; }
        public string customer { get; set; }
        public int amount { get; set; }
        public string recurrence { get; set; }
        public string type { get; set; }
        public string reference { get; set; }
    }

}
