using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Rave
{
    public class Meta
    {
        [JsonProperty("ChargeResponse")]
        public string ChargeResponse { get; set; }

        [JsonProperty("ChargeResponseMessage")]
        public string ChargeResponseMessage { get; set; }

        [JsonProperty("VBVRESPONSEMESSAGE")]
        public string Vbvresponsemessage { get; set; }

        [JsonProperty("VBVRESPONSECODE")]
        public string Vbvresponsecode { get; set; }

        [JsonProperty("ACCOUNTVALIDATIONRESPMESSAGE")]
        public string Accountvalidationrespmessage { get; set; }

        [JsonProperty("ACCOUNTVALIDATIONRESPONSECODE")]
        public string Accountvalidationresponsecode { get; set; }
    }
}
