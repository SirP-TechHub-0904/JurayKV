using System;
using System.Collections.Generic; using System.Text; using JurayKV.Infrastructure.PaymentGateModels.Charge; using Newtonsoft.Json;  namespace JurayKV.Infrastructure.PaymentGateModels.PayPal {     public class PayPalParams : ParamsBase
    {

        public PayPalParams(string PbfPubKey, string secretKey, string firstName, string lastName, string email, decimal amount, string currency, string country, string paymentType, string txRef, string redirect, Address address) : base(PbfPubKey, secretKey, firstName, lastName, email, currency, country)         {             TxRef = txRef;             Amount = amount;             PaymentType = paymentType;             RedirectUrl = redirect;             BillingAddress = address.BillingAddress;             BillingCity = address.BillingCity;             BillingState = address.BillingState;             BillingZip = address.BillingZip;             BillingCountry = address.BillingCountry;          }
        [JsonProperty("payment_type")]         public string PaymentType { get; set; }

        [JsonProperty("txRef")]
        public string TxRef { get; set; }

        [JsonProperty("redirect_url")]
        public string RedirectUrl { get; set; }

    }
}