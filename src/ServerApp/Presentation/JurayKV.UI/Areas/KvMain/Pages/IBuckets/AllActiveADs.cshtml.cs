using JurayKV.Application;
using JurayKV.Application.Queries.BucketQueries;
using JurayKV.Application.Queries.KvAdQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JurayKV.UI.Areas.KvMain.Pages.IBuckets
{
        [Authorize(Policy = Constants.AdvertPolicy)]
    public class AllActiveADsModel : PageModel
    {

        private readonly IMediator _mediator;
        public AllActiveADsModel(IMediator mediator)
        {
            _mediator = mediator;
        }
        public BucketDetailsDto UpdateBucket { get; set; }
        public KvAdDetailsDto AdsDto { get; set; }

        public List<KvAdListDto> KvAds = new List<KvAdListDto>();
        public List<KvAdListDto> KvAdsUpcoming = new List<KvAdListDto>();
        public List<KvAdListDto> KvAdsFinished = new List<KvAdListDto>();
        public async Task<IActionResult> OnGetAsync(DateTime? date)
        {
            DateTime mdate = DateTime.Now;
            if(date != null)
            {
                mdate = date.Value;
            }
            TempData["date"] = mdate.ToString("ddd dd MMM, yyyy");
            GetKvAdActiveListAllBucketQuery command = new GetKvAdActiveListAllBucketQuery(mdate);
            KvAds = await _mediator.Send(command);
            
            return Page();
        }
    }
}
