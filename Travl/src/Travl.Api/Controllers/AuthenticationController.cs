using AspNetCoreHero.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Travl.Application.Authentication.Commands;

namespace Travl.Api.Controllers
{
    public class AuthenticationController : ApiController
    {
        private readonly IMediator _mediator;

        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [AllowAnonymous]
        [ProducesResponseType(typeof(Result), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Result), (int)HttpStatusCode.BadRequest)]
        [HttpPost("signup")]
        public async Task<ActionResult<Guid>> Register(SignupCommand command)
        {
            var signUp = await _mediator.Send(command);
            return Ok(signUp);
        }

        [AllowAnonymous]
        [ProducesResponseType(typeof(Result), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Result), (int)HttpStatusCode.BadRequest)]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            return await Initiate(() => Mediator.Send(command));
        }

        [Authorize]
        [ProducesResponseType(typeof(Result), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Result), (int)HttpStatusCode.BadRequest)]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> GenerateRefreshToken(LoginCommand command)
        {
            return await Initiate(() => Mediator.Send(command));
        }

        [Authorize]
        [ProducesResponseType(typeof(Result), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Result), (int)HttpStatusCode.BadRequest)]
        [HttpPost("logout")]
        public async Task<IActionResult> LoutUser(LoginCommand command)
        {
            return await Initiate(() => Mediator.Send(command));
        }
    }
}
