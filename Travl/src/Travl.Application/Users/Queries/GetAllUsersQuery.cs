using AspNetCoreHero.Results;
using MediatR;
using Travl.Domain.Enums;

namespace Travl.Application.Users.Queries.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<PaginatedResult<GetUsersResponse>>
    {
        public int? PageSize { get; set; } = 10; 
        public int? PageNumber { get; set; } = 1;
        public UserType UserType { get; set; }
    }
}
