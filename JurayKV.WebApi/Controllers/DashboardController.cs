using JurayKV.Application.Queries.DashboardQueries;
using JurayKV.Application.Queries.IdentityKvAdQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JurayKV.WebApi.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {

        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardData()
        {
            var query = new DashboardQuery();
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("runningads/{userId}")]
        public async Task<IActionResult> GetActiveAdsByUserId(Guid userId)
        {
            var query = new GetIdentityKvAdActiveByUserIdListQuery(userId);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("adspostedhistory{userId}")]
        public async Task<IActionResult> GetAdsByUserId(Guid userId)
        {
            var query = new GetIdentityKvAdByUserIdListQuery(userId);
            var result = await _mediator.Send(query);

            return Ok(result);
        }


    }
}
