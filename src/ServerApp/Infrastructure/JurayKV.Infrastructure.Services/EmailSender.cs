using System;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using JurayKV.Application.Infrastructures;
using JurayKV.Application.Services;
using JurayKV.Domain.Aggregates.IdentityAggregate;
using JurayKV.Infrastructure.Services.Configs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace JurayKV.Infrastructure.Services;

public sealed class EmailSender : IEmailSender
{
    private readonly IConfiguration _configManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IExceptionLogger _exceptionLogger;
     public EmailSender(IExceptionLogger exceptionLogger, UserManager<ApplicationUser> userManager, IConfiguration configManager)
    {
        _exceptionLogger = exceptionLogger;
         _userManager = userManager;
        _configManager = configManager;
    }

    public async Task<bool> SendAsync(string smsMessage, string id, string subject)
    {
        try
        {

            var request = await _userManager.FindByIdAsync(id);

            (string Email, string VerificationCode, string Subject) model = (request.Email, smsMessage, subject);
            // string emailBody = await _viewRenderService.RenderViewToStringAsync("/Pages/EmailTemplate/KvMail", model);
            string emailbody = "welcome {mmm}";
            emailbody = emailbody.Replace("{mmm}", model.VerificationCode);
            try
            {


                ////create the mail message 
                MailMessage mail = new MailMessage();


                mail.Body = emailbody;
                //set the addresses 



                mail.From = new MailAddress("help@koboview.com", "KoboView"); //IMPORTANT: This must be same as your smtp authentication address.
                mail.To.Add(model.Email);

                //set the content 
                mail.Subject = model.Subject.Replace("\r\n", "");

                mail.IsBodyHtml = true;
                //send the message 
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

                //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
                NetworkCredential Credentials = new NetworkCredential("help@koboview.com", "xuoyokecrbnihahi");
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = Credentials; smtp.Timeout = 20000;
                //alternative port number is 8889
                smtp.EnableSsl = true; smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(mail);


                return true;
            }
            catch (Exception ex)
            {

                return false;
            } 
        }
        catch (Exception exception)
        {
            await _exceptionLogger.LogAsync(exception, "");
            return false;
        }
    }
}
