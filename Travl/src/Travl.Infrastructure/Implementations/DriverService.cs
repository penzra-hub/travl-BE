using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Application;
using Travl.Application.Interfaces;
using Travl.Domain.Context;
using Travl.Domain.Entities;
using Travl.Application.Dtos.DriverDto;
using Travl.Domain.Enums;
using AspNetCoreHero.Results;
using Travl.Application.IRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Travl.Application.Implementation;
using CloudinaryDotNet.Actions;

namespace Travl.Infrastructure.Implementations
{
    public class DriverService : IDriverService
    {
        private readonly IDriverRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationContext _context;
        private readonly ICloudinaryService _cloudinaryService;


        public DriverService(IDriverRepository repository, 
            IMapper mapper, 
            IHttpContextAccessor httpContextAccessor, 
            ICloudinaryService cloudinaryService)
        {
            _repository = repository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<AspNetCoreHero.Results.IResult> CreateDriverProfile(AppUser user)
        {
            if (user == null) return Result.Fail("User cannot be null");

            // Check if user already has a driver profile

            if (await _repository.GetDriverByAppUserId(user.Id) != null)
            {
                return Result.Fail("Driver profile already exist"); 
            }

            if (user.UserType != UserType.Driver)
            {
                return Result.Fail("User is not a driver"); ;
            }

            // Create Driver Profile

            Driver driver = new Driver()
            {
                Id = Guid.NewGuid().ToString(),
                AppUserId = user.Id,
                Status = Status.Inactive,
                VerificationStatus = VerificationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
            };

            var result = await _repository.AddAsync(driver);

            if (!result.Succeeded) 
                return Result.Fail(result.Message);

            return Result.Success("Driver profile created successfully"); ;
        }

        public async Task<GetDriverDto> GetDriverForPassengerAsync(string driverId)
        {
            var result = await _repository.GetAllAsync();

            if (!result.Succeeded || result.Data == null) return null;

            var drivers = result.Data; // Extract IQueryable<T> from the result

            var driver = await drivers
                .Where(d => d.Id == driverId && !d.IsDeleted)
                .Include(d => d.Vehicles)
                .Include(d => d.UserVerification)
                .Select(d => new GetDriverDto()
                {
                    FullName = d.AppUser.Name,
                    Email = d.AppUser.Email,
                    PhoneNumber = d.AppUser.PhoneNumber,
                    VerificationStatus = d.VerificationStatus.ToString(),
                    VehicleInfo = d.Vehicles.FirstOrDefault().LicensePlateNo ?? "No vehicle listed"
                })
                .FirstOrDefaultAsync();

            return driver;

        }

        public async Task<AspNetCoreHero.Results.IResult> UpdateDriverAsync(string userId, UpdateDriverDto updateDto)
        {
            if (userId == null || updateDto == null) 
                return Result.Fail("Neither the user Id nor the dto object should be null");

            var user = await _context.Users.FindAsync(userId);

            var result = await _repository.GetDriverByAppUserId(userId);

            if (!result.Succeeded) return Result.Fail(result.Message);

            var driver = result.Data;

            // Map DTO to driver Entity

            driver.UpdatedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
            driver.UpdatedAt = DateTime.UtcNow;

            // Map DTO to user Entity

            user.FirstName = updateDto.FirstName ?? user.FirstName;
            user.LastName = updateDto.LastName ?? user.LastName;
            user.Email = updateDto.Email;
            user.PhoneNumber = updateDto.PhoneNumber;
            user.Avatar = updateDto.Avatar.ToString();
            user.UpdatedAt = DateTime.UtcNow;

            // Update avatar if it is populated

            if (updateDto.Avatar != null)
            {
                ImageUploadResult uploadResult = null;
                var existingAvatarPublicId = user.PublicId;

                if (existingAvatarPublicId != null)
                {
                    uploadResult = await _cloudinaryService.UpdatePhotoAsync(updateDto.Avatar, existingAvatarPublicId);
                }
                else
                {
                    uploadResult = await _cloudinaryService.AddPhotoAsync(updateDto.Avatar);
                }

                user.Avatar = uploadResult.Url.ToString();
                user.PublicId = uploadResult.PublicId;
            }

            // Save both records in a transaction for consistency

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Users.Update(user);
                    await _repository.UpdateAsync(driver);
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return Result.Fail("Update failed: " + ex.Message);
                }
            }

            if (!result.Succeeded) return Result.Fail("Driver Update failed");

            return Result.Success("Successfully updated driver information");
        }

    }
}
