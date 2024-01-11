using JurayKV.Application;
using JurayKV.Application.Commands.UserManagerCommands;
using JurayKV.Application.Queries.UserManagerQueries;
using JurayKV.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JurayKV.UI.Areas.User.Pages.Account
{
    [Authorize(Policy = Constants.Dashboard)]

    public class CSAModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public CSAModel(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }
        public List<UserManagerListDto> UserData { get; set; }
        public string Phonenumber { get; set; }

        [BindProperty]
        public UserUpgradeDto UserUpgradeDto { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            var userdata = await _userManager.FindByIdAsync(userId); 
             
            return Page();
        }
    }
}
