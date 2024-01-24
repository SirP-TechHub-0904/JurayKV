using JurayKV.Application;
using JurayKV.Application.Commands.CompanyCommands;
using JurayKV.Application.Commands.NotificationCommands;
using JurayKV.Application.Commands.UserManagerCommands;
using JurayKV.Application.Infrastructures;
using JurayKV.Application.Queries.CompanyQueries;
using JurayKV.Application.Queries.IdentityQueries.UserQueries;
using JurayKV.Application.Services;
using JurayKV.Application.Validation;
using JurayKV.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using static JurayKV.Domain.Primitives.Enum;

namespace JurayKV.UI.Areas.KvMain.Pages.ICompanys
{

    [Authorize(Policy = Constants.CompanyPolicy)]
    public class AddModel : PageModel
    {
        private readonly IExceptionLogger _exceptionLogger;

        private readonly IMediator _mediator;
        public AddModel(IMediator mediator, IExceptionLogger exceptionLogger)
        {
            _mediator = mediator;
            _exceptionLogger = exceptionLogger;
        }

        [BindProperty]
        public string CompanyName { get; set; }

        [BindProperty]
        public decimal AmountPerPoint { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {

            return Page();
        }
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {

            [Required]
            [MaxLength(100, ErrorMessage = "The {1} can't be more than {1} characters long.")]
            public string Surname { get; set; }

            [Required]
            [MaxLength(100, ErrorMessage = "The {1} can't be more than {1} characters long.")]
            public string FirstName { get; set; }
            [Required]
            [ValidatePhoneNumber]
            [MinLength(8, ErrorMessage = "The {1} can't be less than {1} characters long.")]
            public string PhoneNumber { get; set; }

            [Required]
            [MinLength(4, ErrorMessage = "The {0} must be at least {1} characters long.")]
            [MaxLength(50, ErrorMessage = "The {1} can't be more than {1} characters long.")]
            [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
            public string Email { get; set; }

            [Required]
            [MinLength(6, ErrorMessage = "The {0} must be at least {1} characters long.")]
            [MaxLength(20, ErrorMessage = "The {1} can't be more than {1} characters long.")]
            public string Password { get; set; }

            [Required]
            [Compare(nameof(Password), ErrorMessage = "Confirm password does match with password.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Guid UserId = Guid.Empty;
            try
            {
                CreateUserDto create = new CreateUserDto();
                create.Surname = Input.Surname;
                create.Firstname = Input.FirstName;
                create.Email = Input.Email;
                create.Password = Input.Password;
                create.PhoneNumber = Input.PhoneNumber;
               create.Comfirm = true;
                create.Role = "Company";
                CreateUserManagerCommand command = new CreateUserManagerCommand(create);
                ResponseCreateUserDto Result = await _mediator.Send(command);
                if (Result.Succeeded == false)
                {
                    Input.Password = null;
                    Input.ConfirmPassword = null;
                    TempData["error"] = Result.Message; 
                    ModelState.AddModelError(string.Empty, Result.Message);

                    return Page();
                }
                UserId = Result.Id;
                //return RedirectToPage("./Verify", new { xmal = Result.Mxemail, txtd = Result.Id });
                 
            }
            catch (Exception exception)
            {
                Input.Password = null;
                Input.ConfirmPassword = null;
                await _exceptionLogger.LogAsync(exception, Input);
                //return StatusCode(StatusCodes.Status500InternalServerError);
                TempData["error"] = "unable to create account";
                ModelState.AddModelError(string.Empty, "unable to create account");

                return Page();
            }
            try
            {
                CreateCompanyCommand command = new CreateCompanyCommand(CompanyName, UserId, AmountPerPoint);
                Guid Result = await _mediator.Send(command);
                TempData["success"] = "Added Successfuly";
            }
            catch (Exception ex)
            {
                TempData["error"] = "error. adding new bucket";
            }
            return RedirectToPage("./Index");
        }
    }
}
