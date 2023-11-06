using JurayKV.Application.Queries.SliderQueries;
using JurayKV.Domain.Aggregates.SliderAggregate;
using JurayKV.UI.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JurayKV.UI.Pages
{
    public class IndexModel : BasePageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IMediator _mediator;

        public IndexModel(ILogger<IndexModel> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            GetSliderListQuery command = new GetSliderListQuery();
            Slider = await _mediator.Send(command);
            return Page();
        }
        public List<SliderDetailsDto> Slider = new List<SliderDetailsDto>();

        public async Task<IActionResult> OnPostAsync()
        {
            //CreateDepartmentCommand command = new CreateDepartmentCommand("sdhjys", "sdiusuhiu jkli kjjkjkj ukj iuiusd");

            //Guid departmentId = await Mediator.Send(command);
            return Page();
         }
    }
}
 