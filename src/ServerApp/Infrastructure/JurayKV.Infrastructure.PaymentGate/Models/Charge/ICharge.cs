using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JurayKV.Infrastructure.PaymentGateapi;
using JurayKV.Infrastructure.PaymentGateModels;
using Rave;

namespace JurayKV.Infrastructure.PaymentGateModels.Charge
{
    internal interface ICharge<T1, T2> where T1: RaveResponse<T2>, new() where T2 : ChargeResponse  
    {
        IDataEncryption PayDataEncrypt { get; }
        Task<T1> Charge(IParams Params, bool isRecurring = false);
    }
}
