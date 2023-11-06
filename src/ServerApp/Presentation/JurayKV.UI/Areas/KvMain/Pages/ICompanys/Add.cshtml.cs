using JurayKV.Application;
using JurayKV.Application.Commands.CompanyCommands;
using JurayKV.Application.Queries.CompanyQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JurayKV.UI.Areas.KvMain.Pages.ICompanys
{

    [Authorize(Policy = Constants.CompanyPolicy)]
    public class AddModel : PageModel
    {

        private readonly IMediator _mediator;
        public AddModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty]
        public string CompanyName { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            
            try
            {
                CreateCompanyCommand command = new CreateCompanyCommand(CompanyName);
                Guid Result = await _mediator.Send(command);
                TempData["success"] = "Added Successfully";
            }
            catch (Exception ex)
            {
                TempData["error"] = "error. adding new bucket";
            }
            return RedirectToPage("./Index");
        }
    }
}
