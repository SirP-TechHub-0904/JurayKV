using JurayKV.Application;
using JurayKV.Application.Commands.UserManagerCommands;
using JurayKV.Application.Queries.UserManagerQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JurayKV.UI.Areas.KvMain.Pages.IUsers
{
    [Authorize(Policy = Constants.UsersManagerPolicy)]
    public class UpdateModel : PageModel
    {

        private readonly IMediator _mediator;
        public UpdateModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty]
        public UserManagerDetailsDto UpdateUserManager { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                GetUserManagerByIdQuery command = new GetUserManagerByIdQuery(id);
                UpdateUserManager = await _mediator.Send(command);

                return Page();
            }
            catch (Exception ex)
            {
                TempData["error"] = "unable to fetch bucket";
                return RedirectToPage("/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                UpdateUserDto update = new UpdateUserDto();
                update.AccountStatus = UpdateUserManager.AccountStatus;
                update.Email = UpdateUserManager.Email;
                update.PhoneNumber = UpdateUserManager.PhoneNumber;
                update.SurName = UpdateUserManager.Surname;
                update.FirstName = UpdateUserManager.Firstname;
                update.LastName = UpdateUserManager.Lastname;
                update.DisableEmailNotification = UpdateUserManager.DisableEmailNotification;
                update.Tier = UpdateUserManager.Tier;
                update.DateUpgraded = DateTime.UtcNow.AddHours(1);
                update.Tie2Request = UpdateUserManager.Tie2Request; 
                update.EmailComfirmed = UpdateUserManager.EmailComfirmed;
                update.TwoFactorEnable = UpdateUserManager.TwoFactorEnable;
                UpdateUserManagerCommand command = new UpdateUserManagerCommand(UpdateUserManager.Id, update);
                await _mediator.Send(command);
                TempData["success"] = "Updated Successfuly";
            }
            catch (Exception ex)
            {
                TempData["error"] = "error. adding updating";
            }
            return RedirectToPage("./Info", new {id = UpdateUserManager.Id});
        }
    }
}
