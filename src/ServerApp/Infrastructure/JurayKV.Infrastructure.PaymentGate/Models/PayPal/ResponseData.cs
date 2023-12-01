using System;
using Newtonsoft.Json;
using JurayKV.Infrastructure.PaymentGateModels.Charge;

namespace JurayKV.Infrastructure.PaymentGateModels.PayPal
{
    public class ResponseData : Charge.ChargeResponse
    {
        [JsonProperty("appfee")]
        public string AppFee { get; set; }

        [JsonProperty("merchantfee")]
        public string MerchantFee { get; set; }

        [JsonProperty("merchantbearsfee")]
        public string Merchantbearsfee { get; set; }

        [JsonProperty("authModelUsed")]
        public string AuthModelUsed { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("IP")]
        public string Ip { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("vbvrespmessage")]
        public string Vbvrespmessage { get; set; }

        [JsonProperty("vbvrespcode")]
        public string Vbvrespcode { get; set; }


        [JsonProperty("acctvalrespmsg")]
        public string AcctValRespMsg { get; set; }

        [JsonProperty("acctvalrespcode")]
        public string AcctvalRespcode { get; set; }

        [JsonProperty("paymentType")]
        public string PaymentType { get; set; }
    }
}
