using JurayKV.Application;
using JurayKV.Application.Queries.BucketQueries;
using JurayKV.Application.Queries.IdentityQueries.PermissionQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JurayKV.UI.Areas.KvMain.Pages.IBuckets
{
    [Authorize(Policy = Constants.BucketPolicy)]
    public class IndexModel : PageModel
    {

        private readonly IMediator _mediator;
        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public List<BucketListDto> Buckets = new List<BucketListDto>();
        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            GetBucketListQuery command = new GetBucketListQuery();
            Buckets = await _mediator.Send(command);

            return Page();
        }
    }
}
