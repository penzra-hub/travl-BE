using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Travl.Application.Interfaces;
using Travl.Application.Users.Queries.GetAllUsers;
using Travl.Domain.Entities;
using Travl.Domain.Enums;

namespace Travl.Application.Users.Queries.Handlers
{
    internal sealed class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PaginatedResult<GetUsersResponse>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllUsersQueryHandler> _logger;
        private readonly IPaginationHelper _pagination;

        public GetAllUsersQueryHandler(UserManager<AppUser> userManager, IMapper mapper, ILogger<GetAllUsersQueryHandler> logger, IPaginationHelper pagination)
        {
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
            _pagination = pagination;
        }

        public async Task<PaginatedResult<GetUsersResponse>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all users with parameters: {@Request}", request);

                if (!Enum.IsDefined(typeof(UserType), request.UserType))
                {
                    _logger.LogInformation("Invalid User Type provided.");
                    return PaginatedResult<GetUsersResponse>.Failure(new List<string>
                    {
                        "Invalid User Type provided."
                    });
                }

                var query = _userManager.Users
                    .Where(x => (x.IsActive || !x.IsDeleted || x.Status != Status.Active) &&
                                x.UserType == request.UserType);

                var mappedQuery = query.Select(user => _mapper.Map<GetUsersResponse>(user));

                var paginatedResult = await _pagination.ApplyPaginationAsync(mappedQuery, request.PageNumber, request.PageSize, cancellationToken);

                _logger.LogInformation("Successfully fetched paginated users.");
                return paginatedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users: {Message}", ex.Message);
                return PaginatedResult<GetUsersResponse>.Failure(new List<string>
                {
                    "An error occurred while fetching users. Please try again later."
                });
            }
        }
    }
}
