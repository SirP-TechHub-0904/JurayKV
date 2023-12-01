using JurayKV.Infrastructure.RavePayment.Models.Flutter.Balance;
using JurayKV.Infrastructure.RavePayment.Models.Flutter.BankInfo;
using JurayKV.Infrastructure.RavePayment.Models.Flutter.Bill;
using JurayKV.Infrastructure.RavePayment.Models.Flutter.Model;
using JurayKV.Infrastructure.RavePayment.Models.Flutter.Transfer;
using JurayKV.Infrastructure.RavePayment.Models.Flutter.Verify;
using JurayKV.Infrastructure.RavePayment.Models.Response;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace JurayKV.Infrastructure.RavePayment.Repository.FlutterRave
{
    public class FlutterTransactionService : IFlutterTransactionService
    {

        public FlutterTransactionService()
        {

        }

        public static string GenerateToken()
        {

            // Token will be good for 20 minutes
            DateTime Expiry = DateTime.UtcNow.AddMinutes(20);

            string ApiKey = "FLWPUBK_TEST-f4bb871959b328ba5c04417f81263902-X";
            string ApiSecret = "FLWSECK_TEST-2e538587a52c4836a1e5860199817f71-X";

            int ts = (int)(Expiry - new DateTime(1970, 1, 1)).TotalSeconds;

            // Create Security key  using private key above:
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiSecret));

            // length should be >256b
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Finally create a Token
            var header = new JwtHeader(credentials);

            //Zoom Required Payload
            var payload = new JwtPayload
        {
            { "iss", ApiKey},
            { "exp", ts },
        };

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            // Token to String so you can use it in your client
            var tokenString = handler.WriteToken(secToken);

            return tokenString;
        }

        public async Task<BankInformation> AccountInfomation(string account_number, string account_bank)
        {
            try
            {
                string apiurl = $"https://api.flutterwave.com/v3/accounts/resolve";
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");

                var lad = new RequestAccountInfo
                {
                    account_bank = account_bank,
                    account_number = account_number
                };

                request.AddJsonBody(lad);
                IRestResponse response = client.Execute(request);
                var contents = response.Content.ToString();
                //var json = await response.Content.ReadAsStringAsync();

                var mainresponse = JsonConvert.DeserializeObject<BankInformation>(contents);
                return mainresponse;
            }
            catch (Exception g)
            {
                return null;
            }
        }

        public async Task<Models.Flutter.TransactionGetAsync.GetAllTransaction> GetAllTransactions(string from, string to, int page, string customerEmail, string status, string tx_ref, string customerName)
        {
            try
            {
                string apiurl = $"https://api.flutterwave.com/v3/transactions";
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");

                request.AddParameter("from", from);
                request.AddParameter("to", to);
                request.AddParameter("page", page);
                request.AddParameter("customer_email", customerEmail);
                request.AddParameter("status", status);
                request.AddParameter("tx_ref", tx_ref);
                request.AddParameter("customer_fullname", customerName);
                request.AddParameter("currency", "NGN");


                IRestResponse response = client.Execute(request);
                var contents = response.Content.ToString();
                //var json = await response.Content.ReadAsStringAsync();

                var mainresponse = JsonConvert.DeserializeObject<Models.Flutter.TransactionGetAsync.GetAllTransaction>(contents);
                return mainresponse;
            }
            catch (Exception g)
            {
                return null;
            }
        }

        public async Task<Models.Flutter.TransferGetAsync.FetchTransfer> GetAllTransfer(string page, string status)
        {
            try
            {
                string apiurl = $"https://api.flutterwave.com/v3/transfers";
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");
                request.AddParameter("page", page);
                request.AddParameter("status", status);

                //var lad = new Models.Flutter.TransferGetAsync.TransferValue
                //{
                //    page = page,
                //    status = status
                //};

                //request.AddJsonBody(lad);

                IRestResponse response = client.Execute(request);
                var contents = response.Content.ToString();
                //var json = await response.Content.ReadAsStringAsync();

                var mainresponse = JsonConvert.DeserializeObject<Models.Flutter.TransferGetAsync.FetchTransfer>(contents);
                return mainresponse;
            }
            catch (Exception g)
            {
                return null;
            }
        }

        public async Task<Balance> GetBalance()
        {
            try
            {
                string apiurl = $"https://api.flutterwave.com/v3/balances";
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");


                IRestResponse response = client.Execute(request);
                var contents = response.Content.ToString();
                //var json = await response.Content.ReadAsStringAsync();

                var mainresponse = JsonConvert.DeserializeObject<Balance>(contents);
                return mainresponse;
            }
            catch (Exception g)
            {
                return null;
            }
        }

        public async Task<BankVerify> GetBanks()
        {
            try
            {
                string apiurl = $"https://api.flutterwave.com/v3/banks/NG";
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");


                IRestResponse response = client.Execute(request);
                var contents = response.Content.ToString();
                //var json = await response.Content.ReadAsStringAsync();

                var mainresponse = JsonConvert.DeserializeObject<BankVerify>(contents);
                return mainresponse;
            }
            catch (Exception g)
            {
                return null;
            }
        }

        public async Task<FlutterResponse> InitializeTransaction(string tx_ref, string amount, string currency, string redirect_url, string payment_options, int consumer_id, string consumer_mac, string email, string phonenumber, string name, string title, string description, string logo, string from)
        {
            var token = GenerateToken();
            ///v2/accounts/128273138/users/{userId}/meetings
            /////https://api.zoom.us/v2/users
            ///
            string apiurl = "https://api.flutterwave.com/v3/payments";
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
            var client = new RestClient(apiurl);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");

            var lad = new TransactionResponseModel
            {
                tx_ref = tx_ref,
                amount = amount,
                currency = currency,
                redirect_url = redirect_url,
                payment_options = payment_options,
                meta = new Meta
                {
                    consumer_id = consumer_id,
                    consumer_mac = consumer_mac
                },
                customer = new Models.Flutter.Model.Customer
                {
                    email = email,
                    phonenumber = phonenumber,
                    name = name
                },
                customizations = new Customizations
                {
                    title = title,
                    description = description,
                    logo = logo
                }

            };

            request.AddJsonBody(lad);

            IRestResponse response = client.Execute(request);
            var contents = response.Content.ToString();
            //var json = await response.Content.ReadAsStringAsync();

            var mainresponse = JsonConvert.DeserializeObject<FlutterResponse>(contents);
            return mainresponse;
            //if (mainresponse.status == true)
            //{
            //    return Redirect(response.data.authorization_url);
            //}

        }

        public async Task<string> Transfer(string account_bank, string account_number, int amount, string narration, string currency, string reference, string callback_url, string debit_currency, string uid, string from)
        {
            string outputstring = "";
            try
            {
                string apiurl = $"https://api.flutterwave.com/v3/transfers";
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");

                var lad = new TModel
                {
                    account_bank = account_bank,
                    account_number = account_number,
                    amount = amount,
                    narration = narration,
                    currency = currency,
                    reference = reference,
                    callback_url = callback_url,
                    debit_currency = debit_currency
                };

                request.AddJsonBody(lad);

                Thread.Sleep(2000);
                var Walletd = 0;
                if (amount < 500)
                {
                    return "Minimum Amount is N500.";

                }
                if (amount > Walletd)
                {
                    return "Insufficient Amount";

                }
            }
            catch (Exception c)
            {

            }
            return "";
        }


        public async Task<string> MajorTransfer(long id)
        {
            string outputstring = "";
           return "";
        }

        public async Task<FlutterTransactionVerify> VerifyTransaction(string tx_ref)
        {
            //          try{

            try
            {
                string apiurl = $"https://api.flutterwave.com/v3/transactions/" + tx_ref + "/verify";
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");


                IRestResponse response = client.Execute(request);
                var contents = response.Content.ToString();
                //var json = await response.Content.ReadAsStringAsync();

                var mainresponse = JsonConvert.DeserializeObject<FlutterTransactionVerify>(contents);
                return mainresponse;
            }
            catch (Exception g)
            {
                return null;
            }

        }


        public async Task<FlutterTransactionVerify> GetTransfer(string tx_ref)
        {
            //          try{

            try
            {
                string apiurl = $"https://api.flutterwave.com/v3/transactions/" + tx_ref + "/verify";
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");


                IRestResponse response = client.Execute(request);
                var contents = response.Content.ToString();
                //var json = await response.Content.ReadAsStringAsync();

                var mainresponse = JsonConvert.DeserializeObject<FlutterTransactionVerify>(contents);
                return mainresponse;
            }
            catch (Exception g)
            {
                return null;
            }

        }

        public async Task<string> GetBills()
        {
            try
            {
                string apiurl = $"https://api.flutterwave.com/v3/bill-categories";
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");


                IRestResponse response = client.Execute(request);
                var contents = response.Content.ToString();
                //var json = await response.Content.ReadAsStringAsync();

                //var mainresponse = JsonConvert.DeserializeObject<FlutterTransactionVerify>(contents);
                //return mainresponse;
                return "";
            }
            catch (Exception g)
            {
                return null;
            }
        }

        public async Task<ResponseRequestBill> CreateBill(RequestBill model)
        {
            try
            {
                string apiurl = $"https://api.flutterwave.com/v3/accounts/resolve";
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");

                var lad = new RequestBill
                {
                    country = "NG",
                    customer = model.customer,
                    amount = model.amount,
                    recurrence = "ONCE",
                    type = model.type,
                    reference = model.reference
                };

                request.AddJsonBody(lad);
                IRestResponse response = client.Execute(request);
                var contents = response.Content.ToString();
                //var json = await response.Content.ReadAsStringAsync();

                var mainresponse = JsonConvert.DeserializeObject<ResponseRequestBill>(contents);
                return mainresponse;
            }
            catch (Exception g)
            {
                return null;
            }
        }
    }
}
