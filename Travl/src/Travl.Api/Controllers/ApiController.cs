using AspNetCoreHero.Results;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Travl.Application.Common;
using Travl.Application.Common.Exceptions;
using Travl.Application.Common.Extensions;
using IResult = AspNetCoreHero.Results.IResult;

namespace Travl.Api.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public abstract class ApiController : ControllerBase
    {
        private IMediator _mediator;
        private static readonly ILogger Logger = AppLoggerFactory.CreateLogger<ApiController>();
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        protected JwtSecurityToken accessToken;

        protected IActionResult BadRequestResponse(string message)
        {
            return BadRequest(Result.Fail(message));
        }

        protected IActionResult ForbiddenResponse(string message)
        {
            return BadRequest(Result.Fail(message));
        }

        protected IActionResult ServerErrorResponse(string message)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, Result.Fail(message));
        }

        protected async Task<IActionResult> Initiate<TOut>(Func<Task<IResult<TOut>>> action)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(GetErrorsAsList(ModelState));

                var result = await action.Invoke();
                if (result.Succeeded)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (ArgumentException ex)
            {
                Logger.LogError(ex.Message, ex); ;
                return BadRequestResponse(ex.Message);
            }
            catch (ValidationException ex)
            {
                Logger.LogError(ex.Message, ex);
                return BadRequestResponse(string.Join('\n', ex.Errors));
            }
            catch (ForbiddenAccessException ex)
            {
                Logger.LogError(ex.Message, ex);
                return ForbiddenResponse("Forbidden");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
                return ServerErrorResponse(Constants.InternalServerErrorMessage);
            }
        }

        protected async Task<IActionResult> Initiate<TOut>(Func<Task<PaginatedResult<TOut>>> action)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(GetErrorsAsList(ModelState));

                var result = await action.Invoke();
                if (result.Succeeded)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (ArgumentException ex)
            {
                Logger.LogError(ex.Message, ex);
                return BadRequestResponse(ex.Message);
            }
            catch (ValidationException ex)
            {
                Logger.LogError(ex.Message, ex);
                return BadRequestResponse(string.Join('\n', ex.Errors));
            }
            catch (ForbiddenAccessException ex)
            {
                Logger.LogError(ex.Message, ex);
                return ForbiddenResponse("Forbidden");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
                return ServerErrorResponse(Constants.InternalServerErrorMessage);
            }
        }

        protected async Task<IActionResult> Initiate(Func<Task<IResult>> action)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(GetErrorsAsList(ModelState));

                var result = await action.Invoke();
                if (result.Succeeded)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (ArgumentException ex)
            {
                Logger.LogError(ex.Message, ex);
                return BadRequestResponse(ex.Message);
            }
            catch (ValidationException ex)
            {
                Logger.LogError(ex.Message, ex);
                return BadRequestResponse(string.Join('\n', ex.Errors));
            }
            catch (ForbiddenAccessException ex)
            {
                Logger.LogError(ex.Message, ex);
                return ForbiddenResponse("Forbidden");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
                return ServerErrorResponse(Constants.InternalServerErrorMessage);
            }
        }

        public static List<string> GetErrorsAsList(ModelStateDictionary modelState)
        {
            if (modelState == null || !modelState.Values.Any())
                return new List<string>();

            IList<string> allErrors = modelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList();

            var err = allErrors.Where(error => !string.IsNullOrEmpty(error)).ToList();

            if (err.Count == 0)
                err = modelState.Values.SelectMany(v => v.Errors.Select(b => b.Exception.Message)).ToList();

            return err;
        }
    }
}
