﻿using System;
using System.Collections.Generic;
using System.Text;
using JurayKV.Infrastructure.PaymentGateModels.Charge;
using Newtonsoft.Json;

namespace JurayKV.Infrastructure.PaymentGateModels.Ebills
{
    public class EbillsUpdateRequestParams
    {
        public EbillsUpdateRequestParams(string currency, string seckey,  int amount, string reference)
        {

            Currency = currency;
            SECKEY = seckey;
            Amount = amount;
            Reference = reference;

        }


        [JsonProperty("currency")]
        public string Currency { get; set; }


        [JsonProperty("SECKEY")]
        public string SECKEY { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

     
        [JsonProperty("reference")]
        public string Reference { get; set; }
    }
}