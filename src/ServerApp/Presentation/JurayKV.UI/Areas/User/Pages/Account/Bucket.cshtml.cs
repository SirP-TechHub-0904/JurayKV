using JurayKV.Application;
using JurayKV.Application.Queries.BucketQueries;
using JurayKV.Application.Queries.IdentityKvAdQueries;
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


    public class BucketModel : BasePageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        public BucketModel(ILogger<IndexModel> logger, IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _mediator = mediator;
            _userManager = userManager;
        }
        [BindProperty]
        public Guid kId { get; set; }
        public async Task<IActionResult> OnGetAsync(string? error, string? success)
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            GetBucketListQuery command = new GetBucketListQuery();

            Bucket = await _mediator.Send(command);

            TempData["error"] = error;
            TempData["success"] = success;
            return Page();
        }
        public List<BucketListDto> Bucket { get; set; }


    } 
    
}
