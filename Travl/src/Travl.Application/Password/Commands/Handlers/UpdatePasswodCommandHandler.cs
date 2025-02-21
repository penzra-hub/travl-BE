using IResult = AspNetCoreHero.Results.IResult;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Travl.Domain.Entities;
using AspNetCoreHero.Results;
using Microsoft.AspNetCore.Authorization;
using Travl.Application.Interfaces;




namespace Travl.Application.Password.Commands.UpdatePassword
{

    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, IResult>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public UpdatePasswordCommandHandler(UserManager<AppUser> userManager, ICurrentUserService currentUserService)
        {
            _userManager = userManager;        
            _currentUserService = currentUserService;
        }

        public async Task<IResult> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
        {

            var userId = _currentUserService.UserId;

            

            if (string.IsNullOrEmpty(userId))
                return Result.Fail("User is not authenticated."); 

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Fail("User not found.");

            if (request.NewPassword != request.ConfirmPassword)
                return Result.Fail("Passwords do not match.");

            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

            return result.Succeeded
                ? Result.Success("Password updated successfully.")
                : Result.Fail("Password update failed");
        }
    }
}
