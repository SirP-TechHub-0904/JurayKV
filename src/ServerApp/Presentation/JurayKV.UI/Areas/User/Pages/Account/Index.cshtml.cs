using JurayKV.Application;
using JurayKV.Application.Queries.DashboardQueries;
using JurayKV.Application.Queries.UserAccountQueries.DashboardQueries;
using JurayKV.Domain.Aggregates.IdentityAggregate;
using JurayKV.UI.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace JurayKV.UI.Areas.User.Pages.Account
{
    [Authorize(Policy = Constants.Dashboard)]

    
    public class IndexModel : BasePageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        public IndexModel(ILogger<IndexModel> logger, IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _mediator = mediator;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            GetUserDashboardQuery command = new GetUserDashboardQuery(Guid.Parse(userId));

            DashboardData = await _mediator.Send(command);
             
            return Page();
        }
        public UserDashboardDto DashboardData { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {

            return Page();
        }
    }
}
