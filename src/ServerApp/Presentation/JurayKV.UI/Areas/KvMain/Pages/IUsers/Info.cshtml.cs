using JurayKV.Application;
using JurayKV.Application.Queries.CompanyQueries;
using JurayKV.Application.Queries.UserManagerQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JurayKV.UI.Areas.KvMain.Pages.IUsers
{
    [Authorize(Policy = Constants.AdminPolicy)]
    public class InfoModel : PageModel
    {

        private readonly IMediator _mediator;
        public InfoModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty]
        public FullUserManagerDetailsDto UpdateUserManager { get; set; }
        public CompanyDetailsDto UpdateCompany { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                GetFullUserManagerByIdQuery command = new GetFullUserManagerByIdQuery(id);
                UpdateUserManager = await _mediator.Send(command);


                //if (UpdateUserManager.IsCompany == true)
                //{
                //    GetCompanyByUserIdQuery xcommand = new GetCompanyByUserIdQuery(id);
                //    UpdateCompany = await _mediator.Send(xcommand);
                //}
                return Page();
            }
            catch (Exception ex)
            {
                TempData["error"] = "unable to fetch bucket";
                return RedirectToPage("./Index");
            }
        }

     }
}
