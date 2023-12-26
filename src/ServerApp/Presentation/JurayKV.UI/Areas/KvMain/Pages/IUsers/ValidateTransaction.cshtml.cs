using JurayKV.Application.Commands.TransactionCommands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static JurayKV.Domain.Primitives.Enum;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using JurayKV.Application;
using JurayKV.Application.Queries.WalletQueries;

namespace JurayKV.UI.Areas.KvMain.Pages.IUsers
{
     [Authorize(Policy = Constants.SuperAdminPolicy)]
    public class ValidateTransactionModel : PageModel
    {

        private readonly IMediator _mediator;
        public ValidateTransactionModel(IMediator mediator)
        {
            _mediator = mediator;
        }



        [BindProperty]
        public CommandDto Command { get; set; } = new CommandDto();
        public class CommandDto
        {
            public Guid WalletId { get; set; }

            public Guid UserId { get; set; }

            public string Note { get; set; }
            public decimal Amount { get; set; }

            public TransactionTypeEnum TransactionType { get; set; }
            public EntityStatus Status { get; set; }
            public string TransactionReference { get; set; }
            public string Description { get; set; }
            public string TrackCode { get; set; }
        }

        public WalletDetailsDto walet {  get; set; }
        public async Task<IActionResult> OnGetAsync(Guid userId)
        {
            GetWalletUserByIdQuery commandwallet = new GetWalletUserByIdQuery(userId);
             walet = await _mediator.Send(commandwallet);
            if (walet == null)
            {
                return RedirectToPage("./Index");
            }

              
                Command.UserId = walet.UserId;
                Command.WalletId = walet.Id;
                Command.TrackCode = Guid.NewGuid().ToString();
                Command.TransactionReference = Guid.NewGuid().ToString();
             
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            try
            {
                CreateReconsileTransactionCommand command = new CreateReconsileTransactionCommand(Command.WalletId, Command.UserId,
                    Command.Note, Command.Amount, Command.TransactionType, Command.Status, Command.TransactionReference, Command.Description,
                    Command.TrackCode);
                bool Result = await _mediator.Send(command);
                TempData["success"] = "Added Successfully";
            }
            catch (Exception ex)
            {
                TempData["error"] = "error. adding new Transaction";
            }
            return RedirectToPage("./Index");
        }
    }
}
