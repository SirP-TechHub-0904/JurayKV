using System;
using System.Collections.Generic;
using System.Text;
using JurayKV.Infrastructure.PaymentGateModels.Charge;
using Newtonsoft.Json;
namespace JurayKV.Infrastructure.PaymentGateModels.Subscriptions
{
    public class SubscriptionRequestParams
    {
        public SubscriptionRequestParams(string id, string seckey, string fetch_by_tx)
        {

            Id = id;
            Seckey = seckey;
            fetch_by_tx = "1";

        }


        [JsonProperty("id")]
        public string Id { get; set; }


        [JsonProperty("seckey")]
        public string Seckey { get; set; }

        [JsonProperty("fetch_by_tx")]
        public string fetch_by_tx { get; set; }


    }
}