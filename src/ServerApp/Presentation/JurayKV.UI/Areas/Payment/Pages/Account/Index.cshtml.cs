using JurayKV.Application;
using JurayKV.Application.Interswitch;
using JurayKvV.Infrastructure.Interswitch.ResponseModel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JurayKV.UI.Areas.Payment.Pages.Account
{
    [Authorize(Policy = Constants.Dashboard)]

    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;

        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }
      
     public   BillerCategoryListResponse Billers { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            ListBillersCategoryQuery getcommand = new ListBillersCategoryQuery();
            Billers = await _mediator.Send(getcommand);
            return Page();
        }
    }
}
