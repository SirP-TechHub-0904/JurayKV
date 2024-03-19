using JurayKV.Application.Commands.IdentityCommands.UserCommands;
using JurayKV.WebApi.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace JurayKV.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LoginController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Post(LoginDto loginDTO)
        {
            try
            {
                var command = new LoginCommand { Email = loginDTO.Email, Password = loginDTO.Password };
                var token = await _mediator.Send(command);

                if (token != null)
                {
                    return Ok(token);
                }
                else
                {
                    return Unauthorized("Invalid credentials");
                }
            }
            catch
            {
                return BadRequest("An error occurred in generating the token");
            }
           
        }
    }
}
