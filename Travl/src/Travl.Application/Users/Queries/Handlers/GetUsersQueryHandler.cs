using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Text.Json;
using Travl.Domain.Context;
using Travl.Domain.Entities;
using Travl.Domain.Enums;

namespace Travl.Application.Users.Queries.Handlers
{
    internal sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IResult<IList<GetUsersResponse>>>
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<GetUsersQueryHandler> _logger;

        public GetUsersQueryHandler(ApplicationContext context, ILogger<GetUsersQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IResult<IList<GetUsersResponse>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Backend Service - Fetching user details - Request parameters: " +
                                       JsonSerializer.Serialize(request));

                IQueryable<AppUser> usersQuery = _context.Users;

                // Fetch a specific user by ID if provided
                if (!string.IsNullOrWhiteSpace(request.UserId))
                {
                    var user = await usersQuery
                        .Where(u => u.Id == request.UserId && !u.IsDeleted)
                        .Select(u => new GetUsersResponse
                        {
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Email = u.Email,
                            Id = u.Id,
                            UserName = u.UserName,
                            CreatedAt = u.CreatedAt,
                            AccessLevel = u.AccessLevel,
                            Gender = u.Gender,
                            Status = u.Status,
                            PhoneNumber = u.PhoneNumber
                        })
                        .FirstOrDefaultAsync(cancellationToken);

                    if (user == null)
                    {
                        _logger.LogWarning($"User with ID {request.UserId} not found.");
                        return await Result<IList<GetUsersResponse>>.FailAsync("User not found.");
                    }

                    return await Result<IList<GetUsersResponse>>.SuccessAsync(new List<GetUsersResponse> { user });
                }

                // Apply filters, search, and pagination for multiple users
                usersQuery = usersQuery.Where(x => x.IsActive || !x.IsDeleted || x.Status != Status.Active);

                if (!string.IsNullOrWhiteSpace(request.SearchTerm) && request.Filter != null && request.Filter.Any())
                {
                    usersQuery = usersQuery.Where(p =>
                        request.Filter.Contains("FirstName") &&
                         EF.Functions.Like(p.FirstName, $"%{request.SearchTerm}%")
                        || request.Filter.Contains("Email") && p.Email != null &&
                            EF.Functions.Like(p.Email, $"%{request.SearchTerm}%")
                        || request.Filter.Contains("LastName") &&
                            EF.Functions.Like(p.LastName, $"%{request.SearchTerm}%")
                        || request.Filter.Contains("accessLevel") &&
                            EF.Functions.Like(p.AccessLevelDesc, $"%{request.SearchTerm}%")
                    );
                }
                else if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    usersQuery = usersQuery.Where(p =>
                        EF.Functions.Like(p.FirstName, $"%{request.SearchTerm}%") ||
                        EF.Functions.Like(p.LastName, $"%{request.SearchTerm}%") ||
                        p.Email != null && EF.Functions.Like(p.Email, $"%{request.SearchTerm}%")
                    );
                }

                if (request.SortOrder?.ToLower() == "desc")
                {
                    usersQuery = usersQuery.OrderByDescending(GetSortProperty(request));
                }
                else
                {
                    usersQuery = usersQuery.OrderBy(GetSortProperty(request));
                }

                var userResponsesQuery = await usersQuery
                    .Select(p => new GetUsersResponse
                    {
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        Email = p.Email,
                        Id = p.Id,
                        UserName = p.UserName,
                        CreatedAt = p.CreatedAt,
                        AccessLevel = p.AccessLevel,
                        Gender = p.Gender,
                        Status = p.Status,
                        PhoneNumber = p.PhoneNumber
                    })
                    .Skip((int)((request.Page - 1) * request.PageSize))
                    .Take((int)request.PageSize)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Backend Service - Completed retrieving user details.");

                return await Result<IList<GetUsersResponse>>.SuccessAsync(userResponsesQuery);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"Backend Service at {DateTime.UtcNow} - Error retrieving user: {ex?.Message ?? ex?.InnerException?.Message}",
                    ex.StackTrace);
                return await Result<IList<GetUsersResponse>>.FailAsync(
                    $"Error retrieving user: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        private static Expression<Func<AppUser, object>> GetSortProperty(GetUsersQuery request) =>
            request.SortColumn?.ToLower() switch
            {
                "firstname" => user => user.FirstName,
                "lastname" => user => user.LastName,
                "email" => user => user.Email,
                _ => user => user.CreatedAt,
            };
    }
}
