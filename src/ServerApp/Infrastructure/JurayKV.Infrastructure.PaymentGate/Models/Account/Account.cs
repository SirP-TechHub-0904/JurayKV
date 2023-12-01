using System;
using System.Collections.Generic;
using System.Text;
using JurayKV.Infrastructure.PaymentGateModels.Banks;

namespace JurayKV.Infrastructure.PaymentGateModels.Account
{
    public class Account
    {
        public Account(string accountName, string accountNumber, string bankName, string bankCode)
        {
            AccountName = accountName;
            AccountNumber = accountNumber;
            BankName = bankName;
            BankCode = bankCode;
        }
        public Account(string accountName, string accountNumber, Bank bank)
        {
            AccountName = accountName;
            AccountNumber = accountNumber;
            BankName = bank.BankName;
            BankCode = bank.BankCode;
        }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
    }
}
