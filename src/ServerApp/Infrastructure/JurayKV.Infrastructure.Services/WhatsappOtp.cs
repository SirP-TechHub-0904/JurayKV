using JurayKV.Application.Infrastructures;
using JurayKV.Application.Services;
using JurayKV.Domain.Aggregates.IdentityAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Verify.V2.Service;
using Twilio;

namespace JurayKV.Infrastructure.Services
{
         public sealed class WhatsappOtp : IWhatsappOtp
    {
        private readonly IConfiguration _configManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public WhatsappOtp(IConfiguration configManager, UserManager<ApplicationUser> userManager)
        {
            _configManager = configManager;
            _userManager = userManager;
        }

        public async Task<bool> SendAsync(string smsMessage, string id)
        {
            string accountSid = _configManager.GetValue<string>("TWILIO_ACCOUNT_SID");
            string authToken = _configManager.GetValue<string>("TWILIO_AUTH_TOKEN");
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return false;
                }
                //var client = new HttpClient();
                //var request = new HttpRequestMessage(HttpMethod.Post, "https://my.kudisms.net/api/corporate");
                //var content = new MultipartFormDataContent();
                ////content.Add(new StringContent(apiToken), "token");
                ////content.Add(new StringContent("KoboView"), "senderID");
                ////content.Add(new StringContent(user.PhoneNumber), "recipient");
                ////content.Add(new StringContent(smsMessage), "message");
                //request.Content = content;

                //var response = await client.SendAsync(request);
                //response.EnsureSuccessStatusCode();
                //string responseBody = await response.Content.ReadAsStringAsync();
                //var result = JsonConvert.DeserializeObject<SmsResponse>(responseBody);
                //if (result.status == "success")
                //{
                //    return true;
                //}

                // Find your Account SID and Auth Token at twilio.com/console
                // and set the environment variables. See http://twil.io/secure
               
                TwilioClient.Init(accountSid, authToken);

                var verification = VerificationResource.Create(
                    to: "08165680904",
                    channel: "whatsapp",
                    pathServiceSid: "Your otp is 87388738"
                );

                Console.WriteLine(verification.AccountSid);

                return false;
            }
            catch (Exception c)
            {

            }

            return false;
        }
    }

}
