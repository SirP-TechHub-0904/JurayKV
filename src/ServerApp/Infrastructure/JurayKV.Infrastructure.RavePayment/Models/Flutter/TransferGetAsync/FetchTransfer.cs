using System;
using System.Collections.Generic;
using System.Text;

namespace JurayKV.Infrastructure.RavePayment.Models.Flutter.TransferGetAsync
{


    public class FetchTransfer
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
        public string account_number { get; set; }
        public string bank_code { get; set; }
        public string full_name { get; set; }
        public DateTime created_at { get; set; }
        public string currency { get; set; }
        public string debit_currency { get; set; }
        public string amount { get; set; }
        public string fee { get; set; }
        public string status { get; set; }
        public string reference { get; set; }
        public object meta { get; set; }
        public string narration { get; set; }
        public object approver { get; set; }
        public string complete_message { get; set; }
        public int requires_approval { get; set; }
        public int is_approved { get; set; }
        public string bank_name { get; set; }
    }

    public class TransferValue
    {
        public string page { get; set; }
        public string status { get; set; }
    }
}
