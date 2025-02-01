using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travl.Application.Users.Queries;
using Travl.Application.Users.Queries.GetAllUsers;
using Travl.Domain.Enums;
using Travl.Infrastructure.Utility;

namespace Travl.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ApiController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsersController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            // Extract the access token from the Authorization header
            var authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader))
            {
                throw new UnauthorizedAccessException("Authorization header is missing or invalid.");
            }

            accessToken = authHeader.ExtractToken();
            if (accessToken == null)
            {
                throw new UnauthorizedAccessException("Authorization token could not be extracted.");
            }
        }

        /// <summary>
        /// Get details of a specific user by ID.
        /// </summary>
        [HttpGet("get/{userId}")]
        public async Task<IActionResult> Get(
            [FromRoute] string userId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortColumn = null,
            [FromQuery] string? sortOrder = null,
            [FromQuery] List<string>? filter = null,
            [FromQuery] int? page = 1,
            [FromQuery] int? pageSize = 10)
        {
            accessToken.ValidateToken(userId);
            var query = new GetUsersQuery(userId, searchTerm, sortColumn, sortOrder, filter, page, pageSize);

            return await Initiate(() => Mediator.Send(query));
        }

        /// <summary>
        /// Get all users with optional pagination and filtering by user type.
        /// </summary>
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? pageSize = 10,
            [FromQuery] int? pageNumber = 1,
            [FromQuery] UserType userType = UserType.Passenger)
        {
            var query = new GetAllUsersQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                UserType = userType
            };
            return await Initiate(() => Mediator.Send(query));
        }
    }
}
