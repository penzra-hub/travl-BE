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
            var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.AppUserId == appUserId);

            if (driver == null)
            {
                return await Result<Driver>.FailAsync("Driver is not found");
            }

            return await Result<Driver>.SuccessAsync(driver, $"Successfully retrieved driver with id: {appUserId}");
        }

    }
}
