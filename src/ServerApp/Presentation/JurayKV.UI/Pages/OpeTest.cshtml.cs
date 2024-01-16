using JurayKV.Application.Infrastructures;
using JurayKV.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Twilio.Rest.Verify.V2.Service;
using Twilio;
using Twilio.TwiML.Messaging;
using Twilio.Rest.Api.V2010.Account;
using WhatsAppApi.Account;
using WhatsAppApi.Helper;
using WhatsAppApi.Register;
using WhatsAppApi;
using RestSharp;

namespace JurayKV.UI.Pages
{
    [AllowAnonymous]
    public class OpeTestModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IEmailSender _sender;
        public OpeTestModel(IEmailSender sender, UserManager<ApplicationUser> userManager)
        {
            _sender = sender;
            _userManager = userManager;
        }
        public bool Result = false;
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.FindByEmailAsync("onwukaemeka41@gmail.com");
            //Result = await _sender.SendAsync("Test Mail Message", user.Id.ToString(), "Email Test");

            try
            {
                string instanceId = "instance74902"; // your instanceId
                string token = "k5a6qlezjsff9zmt";         //instance Token
                string mobile = "2347067504100";
                string message = "Your KoboView OTP is 9 4 7 9 2 3";
                var url = "https://api.ultramsg.com/" + instanceId + "/messages/chat";
                var client = new RestClient(url);
                var request = new RestRequest(url, Method.POST);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("token", token);
                request.AddParameter("to", mobile);
                request.AddParameter("body", message);


                var response = await client.ExecuteAsync(request);
                var output = response.Content;
                Console.WriteLine(output);

            }
            catch (Exception ex)
            {

            }
            return Page();
        }
    }
}