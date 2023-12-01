using System;
using System.Collections.Generic; using System.Text; using System.Net.Http; using System.Threading.Tasks; using JurayKV.Infrastructure.PaymentGateapi; using JurayKV.Infrastructure.PaymentGateconfig; using Newtonsoft.Json;
using Rave;

namespace JurayKV.Infrastructure.PaymentGateModels.Charge
{
    public class ChargePayPal: Base<RaveResponse<PayPal.ResponseData>, PayPal.ResponseData>
    {
        public ChargePayPal(RaveConfig conf) : base(conf) { }

        public override async Task<RaveResponse<PayPal.ResponseData>> Charge(IParams Params, bool isRecurring = false)         {
            var encryptedKey = PayDataEncrypt.GetEncryptionKey(Config.SecretKey);             var encryptedData = PayDataEncrypt.EncryptData(encryptedKey, JsonConvert.SerializeObject(Params));             var content = new StringContent(JsonConvert.SerializeObject(new { PBFPubKey = Params.PbfPubKey, client = encryptedData, alg = "3DES-24" }), Encoding.UTF8, "application/json");              var requestMessage = new HttpRequestMessage(HttpMethod.Post, Endpoints.CardCharge) { Content = content };             var result = await RaveRequest.Request(requestMessage);              return result;         }
    }
}
