using JurayKV.Application;
using JurayKV.Application.Queries.SettingQueries;
using JurayKV.Application.Queries.IdentityQueries.PermissionQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JurayKV.UI.Areas.KvMain.Pages.ISetting
{
    [Authorize(Policy = Constants.AdvertPolicy)]
    public class IndexModel : PageModel
    {

        private readonly IMediator _mediator;
        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public List<SettingDetailsDto> Setting = new List<SettingDetailsDto>();
        public SettingDetailsDto SettingDetails { get; set; }
        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            GetSettingListQuery command = new GetSettingListQuery();
            Setting = await _mediator.Send(command);


            GetSettingDefaultQuery settingcommand = new GetSettingDefaultQuery();
            SettingDetails = await _mediator.Send(settingcommand);

            return Page();
        }
    }
}
