using Azure.Core;
using JurayKV.Application.Flutterwaves;
using JurayKV.Application.Interswitch;
using JurayKV.Application.Queries.SliderQueries;
using JurayKV.Domain.Aggregates.SliderAggregate;
using JurayKV.Infrastructure.Flutterwave.Models;
using JurayKvV.Infrastructure.Interswitch.RequestModel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JurayKV.UI.Pages
{
    public class PayModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IMediator _mediator;

        public PayModel(ILogger<IndexModel> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
        public string Outcome { get; set; } 
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                //    //CreateTransactionTransferQuery command = new CreateTransactionTransferQuery();
                //    BillPaymentModel model = new BillPaymentModel();
                //    model.country = "NG";
                //    model.customer = "+2348165680904";
                //    model.amount = "100";
                //    //model.recurrence = "ONCE";
                //    model.type = "MTN 50 MB";
                //    model.reference = Guid.NewGuid().ToString();
                //    //model.biller_name = "MTN VTU";

                PaymentRequest model = new PaymentRequest();
                CreateInterswitchTransactionQuery command = new CreateInterswitchTransactionQuery();
                Outcome = await _mediator.Send(command);
                 

                return Page();

            }
            catch (Exception c)
            {
                return Page();
            }
        }
    }
}
