using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Travl.Application.Interfaces;
using Travl.Domain.Entities;
using Travl.Domain.Enums;

namespace Travl.Application.Users.Queries.GetAllUsers
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
            var result = new List<GetUsersResponse>();
            var query = _userManager.Users
                .Where(x => (x.IsActive || !x.IsDeleted || x.Status != Status.Active) &&
                            x.UserType == request.UserType);

            // Apply pagination only if PageNumber and PageSize are greater than 0
            var pageNumber = request.PageNumber ?? 1; // Default to first page
            var pageSize = request.PageSize ?? 10;   // Default to 10 items per page

            query = query
                .Skip((int)((pageNumber - 1) * pageSize))
                .Take((int)pageSize);

            foreach (var user in query)
            {
                var userResponse = _mapper.Map<GetUsersResponse>(user);
                var roles = await _userManager.GetRolesAsync(user);
                userResponse.Role = roles.FirstOrDefault();
                result.Add(userResponse);
            }

            return await _pagination.ApplyPaginationAsync(result.AsQueryable(), pageNumber, pageSize, cancellationToken);
        }
    }
}
