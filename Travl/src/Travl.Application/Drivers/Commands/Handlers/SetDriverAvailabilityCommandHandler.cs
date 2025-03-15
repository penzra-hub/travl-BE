using AspNetCoreHero.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Application.Interfaces;
using Travl.Application.IRepositories;

namespace Travl.Application.Drivers.Commands.Handlers
{
    public class SetDriverAvailabilityCommandHandler : IRequestHandler<SetDriverAvailablityCommand, IResult>
    {
        //private readonly ICacheService _cacheService;
        private readonly IDriverRepository _driverRepository;
        private readonly ICurrentUserService _currentUserService;

        public SetDriverAvailabilityCommandHandler(
            IDriverRepository driverRepository,
            ICurrentUserService currentUserService)
        {
          
            _driverRepository = driverRepository;
            _currentUserService = currentUserService;
        }

        public async Task<IResult> Handle(SetDriverAvailablityCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result.Fail("Unauthorized. User ID not found.");
            }

            var driver = await _driverRepository.GetDriverByAppUserId(userId);
            if (!driver.Succeeded)
            {
                return Result.Fail("Driver not found.");
            }

            //driver.Data.IsAvailable = request.IsAvailable;
            await _driverRepository.UpdateAsync(driver.Data);

            // Cache the availability status in Redis
            string cacheKey = $"driver:{driver.Data.Id}:availability";
            //await _cacheService.SetAsync(cacheKey, request.IsAvailable, TimeSpan.FromMinutes(30));

            return Result.Success("Driver availability updated successfully.");
        }

    }
}
