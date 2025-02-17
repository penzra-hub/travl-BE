using AspNetCoreHero.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travl.Application.IRepositories
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        Task<IResult<IQueryable<TEntity>>> GetAllAsync();
        Task<IResult<TEntity>> FindByIdAsync(string id);
        Task<IResult> AddAsync(TEntity entity);
        Task<IResult> UpdateAsync(TEntity entity);
        Task<IResult> DeleteAsync(string id);
        Task<bool> HasSavedChangesAsync();
    }
}
