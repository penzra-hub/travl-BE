using IResult = AspNetCoreHero.Results.IResult;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Application.Interfaces;
using AspNetCoreHero.Results;
using MediatR;
using AutoMapper;
using Travl.Application.IRepositories;
using Travl.Domain.Context;
using CloudinaryDotNet.Actions;

namespace Travl.Application.Drivers.Commands.Handlers
{
    public class UpdateDriverBasicProfileCommandHandler : IRequestHandler<UpdateDriverBasicDetailsCommand, IResult>
    {       
        private readonly ICurrentUserService _currentUserService;
        private readonly IDriverRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationContext _context;
        private readonly ICloudinaryService _cloudinaryService;


        public UpdateDriverBasicProfileCommandHandler(IDriverRepository repository, IHttpContextAccessor httpContextAccessor, ICloudinaryService cloudinaryService, ApplicationContext context, ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _cloudinaryService = cloudinaryService;
            _context = context;

        }

        public async Task<IResult> Handle(UpdateDriverBasicDetailsCommand command, CancellationToken cancellationToken)
        {
            var userId = _currentUserService?.UserId;

            if (userId == null || command == null)
            {
                return Result.Fail("Neither the user Id nor the dto object should be null");

            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return Result.Fail("User not found");
            }

            var result = await _repository.GetDriverByAppUserId(user.Id);
            if (!result.Succeeded)
            {
                return Result.Fail(result.Message);
            }

            var driver = result.Data;
            driver.UpdatedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;

            // track if any changes are made
            bool isUpdated = false;

            // Update user details if provided
            if (!string.IsNullOrEmpty(command.FirstName)
                && command.FirstName != user.FirstName)
            {
                user.FirstName = command.FirstName;
                isUpdated = true;
            }
            if (!string.IsNullOrEmpty(command.LastName)
                && command.LastName != user.LastName)
            {
                user.LastName = command.LastName;
                isUpdated = true;
            }
            if (!string.IsNullOrEmpty(command.Email)
                && command.Email != user.Email)
            {
                user.Email = command.Email;
                isUpdated = true;
            }
            if (!string.IsNullOrEmpty(command.PhoneNumber)
                && command.PhoneNumber != user.PhoneNumber)
            {
                user.PhoneNumber = command.PhoneNumber;
                isUpdated = true;
            }

            // Update avatar if it is populated
            if (command.Avatar != null && command.Avatar.Length > 0)
            {
                ImageUploadResult? uploadResult = null;
                var existingAvatarPublicId = user.PublicId;

                if (existingAvatarPublicId != null)
                {
                    uploadResult = await _cloudinaryService
                        .UpdatePhotoAsync(command.Avatar, existingAvatarPublicId);
                }
                else
                {
                    uploadResult = await _cloudinaryService
                        .AddPhotoAsync(command.Avatar);
                }

                // Validate upload result
                if (uploadResult == null || string.IsNullOrEmpty(uploadResult.Url.ToString()))
                {
                    return Result.Fail("Failed to upload avatar.");
                }

                user.Avatar = uploadResult.Url.ToString();
                user.PublicId = uploadResult.PublicId;
                isUpdated = true;
            }

            if (!isUpdated)
                return Result.Fail("No changes detected");


            // Save both records in a transaction for consistency
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Users.Update(user);
                    await _repository.UpdateAsync(driver); // Saves changes internally
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return Result.Fail("Update failed: " + ex.Message);
                }
            }

            return Result.Success("Successfully updated driver information");
        }
    }
}
