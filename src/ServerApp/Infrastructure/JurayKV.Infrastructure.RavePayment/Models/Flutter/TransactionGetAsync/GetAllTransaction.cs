using System;
using System.Collections.Generic;
using System.Text;

namespace JurayKV.Infrastructure.RavePayment.Models.Flutter.TransactionGetAsync
{

    //  public class GetAllTransaction

    public class GetAllTransaction
    {
        public string status { get; set; }
        public string message { get; set; }
        public Meta meta { get; set; }
        public Datum[] data { get; set; }
    }

    public class Meta
    {
        public Page_Info page_info { get; set; }
    }

    public class Page_Info
    {
        public int total { get; set; }
        public int current_page { get; set; }
        public int total_pages { get; set; }
    }

    public class Datum
    {
        public int id { get; set; }
        public string tx_ref { get; set; }
        public string flw_ref { get; set; }
        public string device_fingerprint { get; set; }
        public int amount { get; set; }
        public string currency { get; set; }
        public int charged_amount { get; set; }
        public int app_fee { get; set; }
        public int merchant_fee { get; set; }
        public string processor_response { get; set; }
        public string auth_model { get; set; }
        public string ip { get; set; }
        public string narration { get; set; }
        public string status { get; set; }
        public string payment_type { get; set; }
        public DateTime created_at { get; set; }
        public int? amount_settled { get; set; }
        public Card card { get; set; }
        public Customer customer { get; set; }
        public int account_id { get; set; }
        public Meta1 meta { get; set; }
    }

    public class Card
    {
        public string type { get; set; }
        public string country { get; set; }
        public string issuer { get; set; }
        public string first_6digits { get; set; }
        public string last_4digits { get; set; }
        public string expiry { get; set; }
    }

    public class Customer
    {
        public int id { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string name { get; set; }
        public DateTime created_at { get; set; }
    }

    public class Meta1
    {
        public string __CheckoutInitAddress { get; set; }
        public string consumer_id { get; set; }
        public string consumer_mac { get; set; }
        public string originatoraccountnumber { get; set; }
        public string originatorname { get; set; }
        public string bankname { get; set; }
        public string originatoramount { get; set; }
        public string sdk { get; set; }
    }


}
