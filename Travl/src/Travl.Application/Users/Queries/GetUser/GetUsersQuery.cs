using AspNetCoreHero.Results;
using MediatR;

namespace Travl.Application.Users.Queries.GetUser
{
    public record GetUsersQuery(
        string UserId,
        string? SearchTerm,
        string? SortColumn,
        string? SortOrder,
        List<string>? Filter,
        int? Page = 1,
        int? PageSize = 10) : IRequest<IResult<IList<GetUsersResponse>>>;
}
