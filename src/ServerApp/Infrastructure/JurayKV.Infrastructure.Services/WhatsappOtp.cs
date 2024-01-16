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
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using RestSharp;
using System.Text.RegularExpressions;

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
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return false;
                }

                string phoneNumberWithPlus = user.PhoneNumber; // Replace with the actual phone number

                string formattedPhoneNumber = FormatToNigeria(phoneNumberWithPlus);
                if(!String.IsNullOrEmpty(formattedPhoneNumber)) { 
                string instanceId = "instance74902"; // your instanceId
                string token = "k5a6qlezjsff9zmt";  
                string message = smsMessage;
                var url = "https://api.ultramsg.com/" + instanceId + "/messages/chat";
                var client = new RestClient(url);
                var request = new RestRequest(url, Method.POST);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("token", token);
                request.AddParameter("to", formattedPhoneNumber);
                request.AddParameter("body", message);


                var response = await client.ExecuteAsync(request);
                var output = response.Content;
                }
                return true;
            }
            catch (Exception c)
            {

            }

            return false;
        }

        static string FormatToNigeria(string phoneNumber)
        {
            try { 
            // Remove any non-numeric characters from the phone number
            string numericPhoneNumber = Regex.Replace(phoneNumber, @"[^\d]", "");

            // Check if the phone number starts with "+234" and return as is
            if (numericPhoneNumber.StartsWith("234"))
            {
                return "+" + numericPhoneNumber;
            }

            // Check if the phone number starts with "0" and replace it with "+234"
            if (numericPhoneNumber.StartsWith("0"))
            {
                numericPhoneNumber = "+234" + numericPhoneNumber.Substring(1);
            }

            return numericPhoneNumber;
            }catch(Exception c) { 
                
                return "";
                }
        }
    }

}
