using JurayKV.Application.Commands.IdentityCommands.UserCommands;
using JurayKV.Application.Commands.NotificationCommands;
using JurayKV.Application.Queries.IdentityQueries.UserQueries;
using JurayKV.Application.Services;
using JurayKV.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static JurayKV.Domain.Primitives.Enum;

namespace JurayKV.UI.Areas.Auth.Pages.Account
{

    [AllowAnonymous]
    public class ComfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMediator _mediator;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ComfirmationModel(UserManager<ApplicationUser> userManager, IMediator mediator, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _mediator = mediator;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// [bind
        /// 
        [BindProperty]
        public Guid Xtxnt { get; set; }
        [BindProperty]
        public string One { get; set; }
        
        [BindProperty]
        public int NotificationNumber { get; set; }
        public async Task<IActionResult> OnGetAsync(string xcode, string xmal, string txtd)
        {
            if (xmal == null)
            {
                return RedirectToPage("/Index");
            }
            if (txtd == null)
            {
                return RedirectToPage("/Index");

            }
            var data = await _userManager.FindByIdAsync(txtd);

            if (data == null)
            {
                return NotFound($"Unable to load user with email '{xmal}'.");
            }

            Xtxnt = data.Id;
            TempData["codetype"] = xcode;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var identityResult = await _userManager.FindByIdAsync(Xtxnt.ToString());
                    GetEmailVerificationCodeQuery command = new GetEmailVerificationCodeQuery(identityResult.Id.ToString());

                    EmailVerificationCode result = await _mediator.Send(command);
                    //
                    string VerificationCode = One;
                     
                    VerificationCode = VerificationCode.Replace(" ", "");
                    result.Code = result.Code.Replace(" ", "");
                    if (result.Code == VerificationCode)
                    {
                        identityResult.EmailConfirmed = true;
                        await _userManager.UpdateAsync(identityResult);
                        //if (DateTime.UtcNow > result.SentAtUtc.AddMinutes(5))
                        //{
                        //    TempData["error"] = "The code is expired.";
                        //    return Page();
                        //}
                        //UpdateEmailVerificationCodeCommand verificationcommand = new UpdateEmailVerificationCodeCommand(result.Email, result.PhoneNumber, result.Id.ToString(), result.Code, result.Id, DateTime.UtcNow);
                        //bool verificationresult = await _mediator.Send(verificationcommand);
                        await _signInManager.SignInAsync(identityResult, true);
                        return RedirectToPage("/Account/Index", new { area = "User" });
                    }
                    else
                    {
                        TempData["error"] = "Invalid Code";
                        return Page();
                    }

                }
                catch (Exception exception)
                {

                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

    }
}
