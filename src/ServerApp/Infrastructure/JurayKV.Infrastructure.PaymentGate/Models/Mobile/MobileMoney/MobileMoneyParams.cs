﻿using System; using System.Collections.Generic; using System.Text; using JurayKV.Infrastructure.PaymentGateModels.Charge; using Newtonsoft.Json;  namespace JurayKV.Infrastructure.PaymentGateModels.MobileMoney {     public class MobileMoneyParams : ParamsBase     {          public MobileMoneyParams(string PbfPubKey, string secretKey, string firstName, string lastName, string email, decimal amount, string currency, string phonenumber, string network, string country, string paymentType, string txRef) : base(PbfPubKey, secretKey, firstName, lastName, email, currency, country)         {             Amount = amount;             Email = email;             Phonenumber = phonenumber;             Network = network;
            TxRef = txRef;              PaymentType = paymentType;
            switch (PaymentType)             {                  case "mobilemoneygh":                     IsMobileMoneyGh = 1;                     break;                 case "mpesa":                     IsMpesa = "1";                     IsMpesaLipa = 1;                     break;                 case "mobilemoneyuganda":                     IsMobileMoneyUg = 1;                     break;                 case "mobilemoneyzambia":                     IsMobileMoneyUg = 1;                     break;                 default:                     break;             }         }            [JsonProperty("payment_type")]         public string PaymentType { get; set; }

        //[JsonProperty("country")]
        //public string Country { get; set; }

        [JsonProperty("phonenumber")]         public string Phonenumber { get; set; }          [JsonProperty("network")]         public string Network { get; set; }

        //[JsonProperty("voucher")]
        //public string Voucher { get; set; }

        [JsonProperty("IP")]         public string IP { get; set; }          [JsonProperty("txRef")]         public string TxRef { get; set; }          [JsonProperty("orderRef")]         public string OrderRef { get; set; }          [JsonProperty("is_mobile_money_gh")]         public int IsMobileMoneyGh { get; set; }

        //[JsonProperty("device_fingerprint")]
        //public string DeviceFingerprint { get; set; }

        [JsonProperty("is_mobile_money_ug")]         public int IsMobileMoneyUg { get; set; }          [JsonProperty("is_mpesa")]         public string IsMpesa { get; set; }          [JsonProperty("is_mpesa_lipa")]         public int IsMpesaLipa { get; set; }



    } }  