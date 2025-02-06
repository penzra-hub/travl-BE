using AspNetCoreHero.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Domain.Commons;
using Travl.Domain.Enums;

namespace Travl.Application.Authentication.Commands
{
    public record SignupCommand(
        string firstName,
         string lastName,
         string username,
         Gender gender,
         string emailAddress,
         string phoneNumber,
         string? avater,
         string password,
         UserType userType
        ) : IRequest<ApiResponse<Guid>>;
}
