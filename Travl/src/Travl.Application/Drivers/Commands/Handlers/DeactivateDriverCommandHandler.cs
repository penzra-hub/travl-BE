using AspNetCoreHero.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Application.IRepositories;

namespace Travl.Application.Drivers.Commands.Handlers
{
    public class DeactivateDriverCommandHandler : IRequestHandler<DeactivateDriverCommand, IResult>
    {
        private readonly IDriverRepository _driverRepository;
        public DeactivateDriverCommandHandler(IDriverRepository driverRepository)
        {
            _driverRepository = driverRepository;
        }

        public async Task<IResult> Handle(DeactivateDriverCommand request, CancellationToken cancellationToken)
        {
            var driverResult = await _driverRepository.GetDriverByIdAsync(request.DriverId);

            if (!driverResult.Succeeded || driverResult.Data == null)
            {
                return Result.Fail("Driver not found.");
            }

            var driver = driverResult.Data;

            driver.Status = Domain.Enums.Status.Deactivated;
            driver.UpdatedAt = DateTime.UtcNow;

            var updateResult = await _driverRepository.UpdateAsync(driver);

            if (!updateResult.Succeeded)
            {
                return Result.Fail("Failed to deactivate driver.");
            }
            return Result.Success("Driver deactivated successfully.");
        }
    }
}
