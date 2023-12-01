using System;
using JurayKV.Infrastructure.PaymentGateapi;

namespace JurayKV.Infrastructure.PaymentGateModels.PayPal
{
    public class ChargeRes<T> : RaveResponse<T> where T : ResponseData
    {
        public override T Data { get ;  set ; }
    }
}
