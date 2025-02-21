using AspNetCoreHero.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travl.Application.Password.Commands
{
    public record UpdatePasswordCommand(

         string OldPassword,
         string NewPassword,
         string ConfirmPassword
        ) : IRequest<IResult>;

}
