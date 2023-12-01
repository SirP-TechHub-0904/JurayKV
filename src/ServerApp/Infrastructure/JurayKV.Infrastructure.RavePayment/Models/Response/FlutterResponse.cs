using System;
using System.Collections.Generic;
using System.Text;
namespace JurayKV.Infrastructure.RavePayment.Models.Response
{

    public class FlutterResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string link { get; set; }
    }
}
