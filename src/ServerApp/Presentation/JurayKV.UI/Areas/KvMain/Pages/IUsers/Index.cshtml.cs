using JurayKV.Application;
using JurayKV.Application.Queries.UserManagerQueries;
using JurayKV.Application.Queries.IdentityQueries.PermissionQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static JurayKV.Domain.Primitives.Enum;
using Microsoft.AspNetCore.Identity;
using JurayKV.Domain.Aggregates.IdentityAggregate;
using System.Drawing.Printing;
using static WhatsAppApi.Parser.FMessage;
using JurayKV.Infrastructure.Flutterwave.Dtos;

namespace JurayKV.UI.Areas.KvMain.Pages.IUsers
{
    [Authorize(Policy = Constants.UsersManagerPolicy)]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IMediator _mediator;
        public IndexModel(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }
        public IEnumerable<UserManagerListDto> UserManagers = new List<UserManagerListDto>();

        public int All { get; set; }
        public int ActiveOnly { get; set; }
        public int Suspended { get; set; }
        public int Disabled { get; set; }


        public int RequestedTieTwo { get;set;}
        public int ApprovedTieTwo { get;set; }
        public int NotYetTieTwo { get;set; }
        public int Cancelled { get;set; }

        public async Task<IActionResult> OnGetAsync(AccountStatus status = AccountStatus.NotDefind)
        {

            ListGetUserByStatusListQuery command = new ListGetUserByStatusListQuery(status);
            UserManagers = await _mediator.Send(command);

            All = UserManagers.Count();
            ActiveOnly = UserManagers.Where(x => x.AccountStatus == Domain.Primitives.Enum.AccountStatus.Active).Count();
            Suspended = UserManagers.Where(x => x.AccountStatus == Domain.Primitives.Enum.AccountStatus.Suspended).Count();
            Disabled = UserManagers.Where(x => x.AccountStatus == Domain.Primitives.Enum.AccountStatus.Disabled).Count();

            RequestedTieTwo = UserManagers.Where(x=>x.Tie2Request == TieRequestStatus.Requested).Count();
            ApprovedTieTwo = UserManagers.Where(x=>x.Tie2Request == TieRequestStatus.Approved).Count();
            NotYetTieTwo = UserManagers.Where(x=>x.Tie2Request == TieRequestStatus.None).Count();
            Cancelled = UserManagers.Where(x=>x.Tie2Request == TieRequestStatus.Cancelled).Count();
            return Page();
        }
    }
}
