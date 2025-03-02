using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Travl.Domain.Entities;
using MediatR;
using Travl.Application.Password.Commands;


namespace Travl.Api.Controllers
{
   

    [Route("api/v1/[controller]")]
    [ApiController]
    public class PasswordController : ApiController
    {
        
        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordCommand command)
        {
            return await Initiate(() => Mediator.Send(command));
            
        }

        [AllowAnonymous]
        [HttpPost("request-password-reset")]
        public async Task<IActionResult> ResetPassword([FromBody] RequestPasswordResetCommand command)
        {
            return await Initiate(() => Mediator.Send(command));
        }
    }
}

