using AspNetCoreHero.Results;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Domain.Context;
using Travl.Domain.Entities;
using Travl.Application.IRepositories;
using Travl.Infrastructure.Implementations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Travl.Application.Dtos.DriverDto;
using Travl.Domain.Enums;

namespace Travl.Infrastructure.Repositories
{
    public class DriverRepository : RepositoryBase<Driver>, IDriverRepository
    {
        private readonly ApplicationContext _context;

        public DriverRepository(ApplicationContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<IResult<Driver>> GetDriverByAppUserId(string appUserId)
        {
            var driver = await _context.Drivers
                .Include(d => d.Vehicles)
                .Include(d => d.UserVerification)
                .FirstOrDefaultAsync(d => d.AppUserId == appUserId);

            if (driver == null)
            {
                return await Result<Driver>.FailAsync("Driver is not found");
            }

            return await Result<Driver>.SuccessAsync(driver, $"Successfully retrieved driver with id: {appUserId}");
        }

        public async Task<IResult<GetDriverDto>> GetDriverForPassenger(string driverId)
        {
            var driver = await GetAllAsync().Result.Data
                .Where(d => d.Id == driverId && !d.IsDeleted)
                .Include(d => d.Vehicles)
                .Include(d => d.UserVerification)
                .Select(d => new GetDriverDto()
                {
                    Id = driverId,
                    FullName = d.AppUser.Name,
                    Email = d.AppUser.Email,
                    PhoneNumber = d.AppUser.PhoneNumber,
                    VerificationStatus = d.VerificationStatus.ToString(),
                    Status = d.Status.ToString(),
                    Model = d.Vehicles.FirstOrDefault().Model,
                    Color = d.Vehicles.FirstOrDefault().Color,
                    PlateNumber = d.Vehicles.FirstOrDefault().LicensePlateNo ?? "No vehicle listed"
                })
                .FirstOrDefaultAsync();

            if (driver == null) 
                return await Result<GetDriverDto>.FailAsync("Driver not found");

            return await Result<GetDriverDto>.SuccessAsync(driver);
        }

        public async Task<IResult> CreateDriverProfile(AppUser user)
        {

            if (user.UserType != UserType.Driver)
            {
                return Result.Fail("User is not a driver"); ;
            }

            // Create Driver Profile
            Driver driver = new Driver()
            {
                Id = Guid.NewGuid().ToString(),
                AppUserId = user.Id,
                Status = Status.Active,
                VerificationStatus = VerificationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
            };

            var result = await AddAsync(driver);

            if (!result.Succeeded)
                return Result.Fail(result.Message);

            return Result.Success("Driver profile created successfully"); 
        }

    }
}
