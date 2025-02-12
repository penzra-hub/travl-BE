using AspNetCoreHero.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Domain.Entities;

namespace Travl.Application.IRepositories
{
    public interface IDriverRepository : IRepositoryBase<Driver>
    {
        Task<IResult<Driver>> GetDriverByAppUserId(string id);
    }
}
