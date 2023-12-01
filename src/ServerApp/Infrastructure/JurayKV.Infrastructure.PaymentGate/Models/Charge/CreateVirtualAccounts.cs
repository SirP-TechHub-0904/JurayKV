using JurayKV.Infrastructure.PaymentGateapi; using JurayKV.Infrastructure.PaymentGateModels.VirtualAccount; using Newtonsoft.Json;
using Rave;
using System.Text; 
namespace JurayKV.Infrastructure.PaymentGateModels.Charge
{
    public class CreateVirtualAccounts : Base<RaveResponse<Card.ResponseData>, Card.ResponseData>
	{
		public CreateVirtualAccounts(RaveConfig conf) : base(conf) { }


		//public override async Task<RaveResponse<Card.ResponseData>> Create(VirtualAccountParams virtualaccountparams)
		//{

		//	var content = new StringContent(JsonConvert.SerializeObject(new { virtualaccountparams }), Encoding.UTF8, "application/json");
		//	var requestMessage = new HttpRequestMessage(HttpMethod.Post, Endpoints.CreateaccountNumber) { Content = content };

		//	var result = await RaveRequest.Request(requestMessage);
		//	return result;
		//}

        public override async Task<RaveResponse<Card.ResponseData>> Charge(IParams parameters, bool isRecurring)
        {

            var content = new StringContent(JsonConvert.SerializeObject(new { parameters }), Encoding.UTF8, "application/json");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, Endpoints.CreateaccountNumber) { Content = content };

            var result = await RaveRequest.Request(requestMessage);
            return result;
        }

        //public override async Task<RaveResponse<Card.ResponseData>> Create(VirtualAccountParams virtualaccountparams)
        //{
        //    var content = new StringContent(JsonConvert.SerializeObject(new { virtualaccountparams }), Encoding.UTF8, "application/json");
        //    var requestMessage = new HttpRequestMessage(HttpMethod.Post, Endpoints.CreateaccountNumber) { Content = content };

        //    var result = await RaveRequest.Request(requestMessage);
        //    return result;
        //}
    }
}
