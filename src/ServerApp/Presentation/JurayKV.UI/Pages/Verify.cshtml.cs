using JurayKV.Application.Flutterwaves;
using JurayKV.Application.Queries.SliderQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JurayKV.UI.Pages
{
    public class VerifyModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IMediator _mediator;

        public VerifyModel(ILogger<IndexModel> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var tranxRef = HttpContext.Request.Query["tx_ref"].ToString();
            var transaction_id = HttpContext.Request.Query["transaction_id"].ToString();
            var status = HttpContext.Request.Query["status"].ToString();
             VerifyTransactionQuery command = new VerifyTransactionQuery(transaction_id);
            var response = await _mediator.Send(command);
            
            return Page();
        }
    }


}
