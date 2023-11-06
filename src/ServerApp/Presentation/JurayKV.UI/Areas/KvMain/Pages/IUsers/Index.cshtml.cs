using JurayKV.Application;
using JurayKV.Application.Queries.UserManagerQueries;
using JurayKV.Application.Queries.IdentityQueries.PermissionQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JurayKV.UI.Areas.KvMain.Pages.IUsers
{
    [Authorize(Policy = Constants.AdminPolicy)]
    public class IndexModel : PageModel
    {

        private readonly IMediator _mediator;
        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public List<UserManagerListDto> UserManagers = new List<UserManagerListDto>();
        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            GetUserManagerListQuery command = new GetUserManagerListQuery();
            UserManagers = await _mediator.Send(command);

            return Page();
        }
    }
}
