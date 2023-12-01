using JurayKV.Infrastructure.PaymentGateModels.Account;
using JurayKV.Infrastructure.PaymentGateModels.Charge;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Rave;

namespace JurayKV.UI.Pages
{
    public class PayModel : PageModel
    {
        private readonly RaveConfig _raveConfig;

        public PayModel(RaveConfig raveConfig)
        {
            _raveConfig = raveConfig;
        }

        public void OnGet()
        {
        }

        [HttpPost]
        public IActionResult OnPost([FromBody] PaymentDetails paymentDetails)
        {
            // Now you can access _raveConfig in your code
            var accountc = new ChargeAccount(_raveConfig);

            var payload = new AccountParams(_raveConfig.PbfPubKey, _raveConfig.SecretKey, "customer", "customer", "user@example.com", "0690000031", 1000, "044", "NGN", "MC-0292920");
            var chargeResponse = accountc.Charge(payload).Result;

            // Process the chargeResponse as needed

            return Page();
        }



        // You can create a class to represent the payment details if needed
        public class PaymentDetails
        {
            // Add properties as needed for your payment details
        }
    }
}
