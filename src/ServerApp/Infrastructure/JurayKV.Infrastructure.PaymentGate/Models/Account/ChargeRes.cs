using System;
using System.Collections.Generic;
using System.Text;
using JurayKV.Infrastructure.PaymentGateapi;
using JurayKV.Infrastructure.PaymentGateModels.Account;

namespace JurayKV.Infrastructure.PaymentGateModels.Account
{
    public class ChargeRes<T> : RaveResponse<T> where T : ResponseData
    {
    }
}
