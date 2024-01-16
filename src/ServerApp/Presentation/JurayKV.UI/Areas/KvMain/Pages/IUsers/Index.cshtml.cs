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

namespace JurayKV.UI.Areas.KvMain.Pages.IUsers
{
    [Authorize(Policy = Constants.AdminPolicy)]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IMediator _mediator;
        public IndexModel(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }
        public string Title { get; set; }
        public List<UserManagerListDto> UserManagers = new List<UserManagerListDto>();

        public int All { get;set;}
        public int ActiveOnly { get;set;}
        public int ActiveWhatsapp { get;set;}
        public int Suspended { get;set;}
        public int Disabled { get;set; }


        public async Task<IActionResult> OnGetAsync(AccountStatus status = AccountStatus.NotDefind, bool all = false)
        {
            if (all == true)
            {
                GetUserByStatusListQuery command = new GetUserByStatusListQuery(Domain.Primitives.Enum.AccountStatus.NotDefind);
                UserManagers = await _mediator.Send(command);
                Title = "ALL USERS";
            }
            else
            {
                if (status == AccountStatus.NotDefind)
                {
                    GetUserByStatusListQuery command = new GetUserByStatusListQuery(Domain.Primitives.Enum.AccountStatus.Active);
                    UserManagers = await _mediator.Send(command);

                    GetUserByStatusListQuery xccommand = new GetUserByStatusListQuery(Domain.Primitives.Enum.AccountStatus.WhatsappActive);
                    var XUserManagers = await _mediator.Send(xccommand);
                    UserManagers.AddRange(XUserManagers);
                    Title = "ACTIVE/WHATSAPP ACTIVE USERS";
                }
                else
                {
                    GetUserByStatusListQuery command = new GetUserByStatusListQuery(status);
                    UserManagers = await _mediator.Send(command);

                    Title = status.ToString().ToUpper() + "USERS";
                }
            }

            var entity = _userManager.Users.AsQueryable();

            All = entity.Count();
            ActiveOnly = entity.Where(x => x.AccountStatus == Domain.Primitives.Enum.AccountStatus.Active).Count();
            ActiveWhatsapp = entity.Where(x => x.AccountStatus == Domain.Primitives.Enum.AccountStatus.WhatsappActive).Count();
            Suspended = entity.Where(x => x.AccountStatus == Domain.Primitives.Enum.AccountStatus.Suspended).Count();
            Disabled = entity.Where(x => x.AccountStatus == Domain.Primitives.Enum.AccountStatus.Disabled).Count();
            return Page();
        }
    }
}
