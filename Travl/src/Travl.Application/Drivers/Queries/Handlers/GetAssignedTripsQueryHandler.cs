using AspNetCoreHero.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Application.Interfaces;
using Travl.Application.IRepositories;
using Travl.Domain.Entities;

namespace Travl.Application.Drivers.Queries.Handlers
{
    public class GetAssignedTripsQueryHandler : IRequestHandler<GetAssignedTripsQuery, IResult<List<Ride>>>
    {
        //private readonly ICacheService _cacheService;
        private readonly IRepositoryBase<Ride> _tripRepository;
        private readonly IDriverRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public GetAssignedTripsQueryHandler(
            IRepositoryBase<Ride> tripRepository,
            ICurrentUserService currentUserService)
        {
           
            _tripRepository = tripRepository;
            _currentUserService = currentUserService;
        }

        public async Task<IResult<List<Ride>>> Handle(GetAssignedTripsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result<List<Ride>>.Fail("Unauthorized. User ID not found.");
            }

            var driverQuery = await _repository.GetDriverByAppUserId(userId);

            if (!driverQuery.Succeeded)
            {
                return Result<List<Ride>>.Fail("Driver not found.");
            }

            var driver = driverQuery.Data;

            // Check Redis first before querying the database
            string cacheKey = $"driver:{driver.Id}:trips";
            //var cachedTrips = await _cacheService.GetAsync<List<Ride>>(cacheKey);

            //if (cachedTrips != null)
            //{
            //    return Result<List<Ride>>.Success(cachedTrips);
            //}

            // If not cached, fetch from database and store in Redis
            var trips = await _tripRepository.GetAllAsync();

            var assignedtrips = trips.Data.Where(r => r.DriverId == driver.Id).ToList() ;
            //await _cacheService.SetAsync(cacheKey, trips, TimeSpan.FromMinutes(10));

            return Result<List<Ride>>.Success(assignedtrips, "Drivers assigned trips successfully retrieved");
        }
    }
}
