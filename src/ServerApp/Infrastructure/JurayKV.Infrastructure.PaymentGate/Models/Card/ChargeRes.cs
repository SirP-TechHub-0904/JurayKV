using System;
using System.Collections.Generic;
using System.Text;
using JurayKV.Infrastructure.PaymentGateapi;

namespace JurayKV.Infrastructure.PaymentGateModels.Card
{
    public class ChargeRes<T> : RaveResponse<T> where T : ResponseData
    {
        public override T Data { get ;  set ; }
    }
}
