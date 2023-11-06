using JurayKV.Application;
using JurayKV.Application.Queries.BucketQueries;
using JurayKV.Application.Queries.IdentityKvAdQueries;
using JurayKV.Application.Queries.KvAdQueries;
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


    public class AdsModel : BasePageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdsModel(ILogger<IndexModel> logger, IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _mediator = mediator;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        { 
            GetKvAdListByBucketIdQuery command = new GetKvAdListByBucketIdQuery(id);

            Ads = await _mediator.Send(command);

            //

            GetBucketByIdQuery cmd = new GetBucketByIdQuery(id);
            Bucket = await _mediator.Send(cmd);
            return Page();
        }
        public List<KvAdListDto> Ads { get; set; }
        public BucketDetailsDto Bucket { get;set;}

      
    }
     
}
