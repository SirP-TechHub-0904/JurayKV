using JurayKvV.Infrastructure.Interswitch.RequestModel;
using JurayKvV.Infrastructure.Interswitch.ResponseModel;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JurayKvV.Infrastructure.Interswitch.Repositories
{
    public class SwitchRepository : ISwitchRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public SwitchRepository(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
        }
      
        public async Task<string> Payment(PaymentRequest model)
        {
            var clientId = _configuration["Interswitch:ClientId"];
            var clientSecret = _configuration["Interswitch:ClientSecret"];
            //access bearer
            Authentication authentication= new Authentication(_configuration);
            string accessToken = await authentication.GetAccessTokenAsync(clientId, clientSecret);


            var apiUrl = Endpoints.Pay;

            var requestData = new PaymentRequest
            {
               merchant_code = model.merchant_code,
               pay_item_id = model.pay_item_id,
               txn_ref = model.txn_ref,
               amount = model.amount,
               currency = model.currency,
               cust_email = model.cust_email,
               cust_id = model.cust_id,
               cust_name = model.cust_name,
               pay_item_name = model.pay_item_name,
               site_redirect_url = model.site_redirect_url,
            };

            var jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

             _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");


            var response = await _httpClient.PostAsync(apiUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Process the response content
            var paymentResponse = JsonConvert.DeserializeObject<PaymentResponse>(responseContent);

            // Now you can access the properties of paymentResponse
            return responseContent;
        }

        public async Task<ComfirmationResponse> PaymentComfirmation(string merchantcode, string reference, string amount)
        {
            var clientId = _configuration["Interswitch:ClientId"];
            var clientSecret = _configuration["Interswitch:ClientSecret"];

            // Access bearer token
            Authentication authentication = new Authentication(_configuration);
            string accessToken = await authentication.GetAccessTokenAsync(clientId, clientSecret);

            var apiUrl = $"https://qa.interswitchng.com/collections/api/v1/gettransaction.json?merchantcode={merchantcode}&transactionreference={reference}&amount={amount}";

            // Make the GET request
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");

            var response = await _httpClient.GetAsync(apiUrl);

            response.EnsureSuccessStatusCode(); // Ensure the request was successful (status code 2xx)

            var responseContent = await response.Content.ReadAsStringAsync();

            // Process the response content
            var paymentResponse = JsonConvert.DeserializeObject<ComfirmationResponse>(responseContent);

            // Now you can access the properties of paymentResponse
            return paymentResponse;
        }

        public async Task<UssdIssuersResponse> GetUssdIssuers()
        {
            var apiUrl = "https://qa.interswitchng.com/collections/api/v1/ussd/issuers/NG";

            // Make the GET request
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = await _httpClient.GetAsync(apiUrl);

            response.EnsureSuccessStatusCode(); // Ensure the request was successful (status code 2xx)

            var responseContent = await response.Content.ReadAsStringAsync();

            // Process the response content
            var ussdIssuersResponse = JsonConvert.DeserializeObject<UssdIssuersResponse>(responseContent);

            // Now you can access the properties of ussdIssuersResponse
            return ussdIssuersResponse;
        }
        public async Task<GenerateUssdResponse> GenerateUssdTransaction(string accessToken, UssdTransactionRequest requestData)
        {
            var apiUrl = "https://qa.interswitchng.com/collections/api/v1/sdk/ussd/generate";

            // Serialize the request data to JSON
            var jsonRequest = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            // Make the POST request
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            response.EnsureSuccessStatusCode(); // Ensure the request was successful (status code 2xx)

            var responseContent = await response.Content.ReadAsStringAsync();

            // Process the response content
            var generateUssdResponse = JsonConvert.DeserializeObject<GenerateUssdResponse>(responseContent);

            // Now you can access the properties of generateUssdResponse
            return generateUssdResponse;
        }
        public async Task<VirtualAccountTransactionResponse> CreateVirtualAccountTransaction(string accessToken, VirtualAccountTransactionRequest requestData)
        {
            var apiUrl = "https://qa.interswitchng.com/paymentgateway/api/v1/virtualaccounts/transaction";

            // Serialize the request data to JSON
            var jsonRequest = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            // Make the POST request
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            response.EnsureSuccessStatusCode(); // Ensure the request was successful (status code 2xx)

            var responseContent = await response.Content.ReadAsStringAsync();

            // Process the response content
            var virtualAccountTransactionResponse = JsonConvert.DeserializeObject<VirtualAccountTransactionResponse>(responseContent);

            // Now you can access the properties of virtualAccountTransactionResponse
            return virtualAccountTransactionResponse;
        }
        public async Task<VirtualAccountTransactionResponse> GetVirtualAccountTransaction(string accessToken, string merchantCode, string transactionReference)
        {
            var apiUrl = $"https://qa.interswitchng.com/paymentgateway/api/v1/virtualaccounts/transaction?merchantCode={merchantCode}&transactionReference={transactionReference}";

            // Make the GET request
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = await _httpClient.GetAsync(apiUrl);

            response.EnsureSuccessStatusCode(); // Ensure the request was successful (status code 2xx)

            var responseContent = await response.Content.ReadAsStringAsync();

            // Process the response content
            var virtualAccountTransactionResponse = JsonConvert.DeserializeObject<VirtualAccountTransactionResponse>(responseContent);

            // Now you can access the properties of virtualAccountTransactionResponse
            return virtualAccountTransactionResponse;
        }
        public async Task<PayableVirtualAccountResponse> CreatePayableVirtualAccount(string accessToken, PayableVirtualAccountRequest requestData)
        {
            var apiUrl = "https://qa.interswitchng.com/paymentgateway/api/v1/payable/virtualaccount";

            // Serialize the request data to JSON
            var jsonRequest = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            // Make the POST request
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            response.EnsureSuccessStatusCode(); // Ensure the request was successful (status code 2xx)

            var responseContent = await response.Content.ReadAsStringAsync();

            // Process the response content
            var payableVirtualAccountResponse = JsonConvert.DeserializeObject<PayableVirtualAccountResponse>(responseContent);

            // Now you can access the properties of payableVirtualAccountResponse
            return payableVirtualAccountResponse;
        }
        public async Task<GenerateQrResponse> GenerateQrCode(string accessToken, QrCodeRequest requestData)
        {
            var apiUrl = "https://qa.interswitchng.com/collections/api/v1/sdk/qr/generate";

            // Serialize the request data to JSON
            var jsonRequest = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            // Make the POST request
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            response.EnsureSuccessStatusCode(); // Ensure the request was successful (status code 2xx)

            var responseContent = await response.Content.ReadAsStringAsync();

            // Process the response content
            var generateQrResponse = JsonConvert.DeserializeObject<GenerateQrResponse>(responseContent);

            // Now you can access the properties of generateQrResponse
            return generateQrResponse;
        }
        public async Task<QuickTellerServicesResponse> GetQuickTellerBillServices(string accessToken, string terminalId)
        {
            var apiUrl = "http://172.25.20.59/quicktellerservice/api/v5/services";

            // Make the GET request
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            _httpClient.DefaultRequestHeaders.Add("TerminalID", terminalId);

            var response = await _httpClient.GetAsync(apiUrl);

            response.EnsureSuccessStatusCode(); // Ensure the request was successful (status code 2xx)

            var responseContent = await response.Content.ReadAsStringAsync();

            // Process the response content
            var quickTellerServicesResponse = JsonConvert.DeserializeObject<QuickTellerServicesResponse>(responseContent);

            // Now you can access the properties of quickTellerServicesResponse
            return quickTellerServicesResponse;
        }
        public async Task<QuickTellerTransactionsResponse> QueryQuickTellerTransactions(string token, string requestRef, string terminalId)
        {
            var apiUrl = $"http://172.25.20.59/quicktellerservice/api/v5/Transactions?requestRef={requestRef}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            _httpClient.DefaultRequestHeaders.Add("TerminalID", terminalId);

            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                // Handle non-success status codes
                // You might want to log or throw a specific exception
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            var quickTellerTransactionsResponse = JsonConvert.DeserializeObject<QuickTellerTransactionsResponse>(responseContent);

            return quickTellerTransactionsResponse;
        }
        public async Task<QuickTellerServicesResponse> GetBillersByCategoryQuickTellerServices(string token, string categoryId, string terminalId)
        {
            var apiUrl = $"http://172.25.20.59/quicktellerservice/api/v5/services?categoryId={categoryId}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            _httpClient.DefaultRequestHeaders.Add("TerminalID", terminalId);

            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                // Handle non-success status codes
                // You might want to log or throw a specific exception
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            var quickTellerServicesResponse = JsonConvert.DeserializeObject<QuickTellerServicesResponse>(responseContent);

            return quickTellerServicesResponse;
        }
        public async Task<QuickTellerServiceOptionsResponse> GetBillerPaymentItemQuickTellerServiceOptions(string token, string serviceId, string terminalId)
        {
            var apiUrl = $"http://172.25.20.59/quicktellerservice/api/v5/services/options?serviceid={serviceId}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            _httpClient.DefaultRequestHeaders.Add("TerminalID", terminalId);

            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                // Handle non-success status codes
                // You might want to log or throw a specific exception
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            var quickTellerServiceOptionsResponse = JsonConvert.DeserializeObject<QuickTellerServiceOptionsResponse>(responseContent);

            return quickTellerServiceOptionsResponse;
        }

        public async Task<TransactionResponse> ProcessTransaction(string token, string terminalId, SendBillPaymentRequest transactionRequest)
        {
            var apiUrl = "http://172.25.20.59/quicktellerservice/api/v5/Transactions";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            _httpClient.DefaultRequestHeaders.Add("TerminalID", terminalId);

            var jsonRequest = JsonConvert.SerializeObject(transactionRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                // Handle non-success status codes
                // You might want to log or throw a specific exception
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            var transactionResponse = JsonConvert.DeserializeObject<TransactionResponse>(responseContent);

            return transactionResponse;
        }

        public async Task<ValidationResponse> ValidateCustomers(string token, string terminalId, ValidateCustomersRequest validateCustomersRequest)
        {
            var apiUrl = "http://172.25.20.59/quicktellerservice/api/v5/Transactions/validatecustomers";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            _httpClient.DefaultRequestHeaders.Add("TerminalID", terminalId);

            var jsonRequest = JsonConvert.SerializeObject(validateCustomersRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                // Handle non-success status codes
                // You might want to log or throw a specific exception
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            var validationResponse = JsonConvert.DeserializeObject<ValidationResponse>(responseContent);

            return validationResponse;
        }
        public async Task<BillersCategoriesResponse> GetBillersCategories(string token, string terminalId)
        {
            var apiUrl = "http://172.25.20.59/quicktellerservice/api/v5/services/categories";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            _httpClient.DefaultRequestHeaders.Add("TerminalID", terminalId);
              

            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                // Handle non-success status codes
                // You might want to log or throw a specific exception
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            var serviceCategoriesResponse = JsonConvert.DeserializeObject<BillersCategoriesResponse>(responseContent);

            return serviceCategoriesResponse;
        }
    }
}
