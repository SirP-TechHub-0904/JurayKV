using JurayKV.Application;
using JurayKV.Application.Commands.ExchangeRateCommands;
using JurayKV.Application.Queries.ExchangeRateQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JurayKV.UI.Areas.KvMain.Pages.IRates
{
    [Authorize(Policy = Constants.Transaction)]
    public class UpdateModel : PageModel
    {

        private readonly IMediator _mediator;
        public UpdateModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty]
        public ExchangeRateDetailsDto Command { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                GetExchangeRateByIdQuery command = new GetExchangeRateByIdQuery(id);
                Command = await _mediator.Send(command);

                //
                return Page();
            }
            catch (Exception ex)
            {
                TempData["error"] = "unable to fetch Ads";
                return RedirectToPage("/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                UpdateExchangeRateCommand command = new UpdateExchangeRateCommand(Command.Id, Command.Amount);

                await _mediator.Send(command);
                TempData["success"] = "Updated Successfuly";

            }
            catch (Exception ex)
            {
                TempData["error"] = "error. adding new bucket";
            }
            return RedirectToPage("./Index");

        }
    }
}
