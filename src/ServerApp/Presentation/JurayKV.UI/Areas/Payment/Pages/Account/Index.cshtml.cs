using JurayKV.Application;
using JurayKV.Application.Interswitch;
using JurayKV.Application.Queries.SettingQueries;
using JurayKV.Application.VtuServices;
using JurayKV.Domain.Aggregates.CategoryVariationAggregate;
using JurayKvV.Infrastructure.Interswitch.ResponseModel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JurayKV.UI.Areas.Payment.Pages.Account
{
    [Authorize(Policy = Constants.Dashboard)]

    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;

        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public BillerCategoryListResponse Billers { get; set; }
        public SettingDetailsDto SettingDetails { get; set; }

        public List<CategoryVariation> CategoryVariations { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            GetSettingDefaultQuery settingcommand = new GetSettingDefaultQuery();
            SettingDetails = await _mediator.Send(settingcommand);

            if (SettingDetails == null)
            {
                return RedirectToPage("Index");
            }

            if (SettingDetails.BillGateway == Domain.Primitives.Enum.BillGateway.VTU)
            {
                GetVariationCategoryCommand categorycommand = new GetVariationCategoryCommand();
                CategoryVariations = await _mediator.Send(categorycommand);
            }
            else
            {
                ListBillersCategoryQuery getcommand = new ListBillersCategoryQuery();
                Billers = await _mediator.Send(getcommand);
            }
            return Page();
        }
    }
}
