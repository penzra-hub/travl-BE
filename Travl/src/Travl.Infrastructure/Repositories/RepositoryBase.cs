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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Travl.Infrastructure.Repositories
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        private readonly ApplicationContext _context;

        public RepositoryBase(ApplicationContext context)
        {
            _context = context;
        }


        public async Task<bool> HasSavedChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IResult> AddAsync(TEntity entity)
        {
            if (entity == null) return await Result<TEntity>.FailAsync("Entity cannot be null");

            await _context.Set<TEntity>().AddAsync(entity);

            if (!await HasSavedChangesAsync())
            {
                return await Result.FailAsync("An error occurred while saving the entity");
            }

            return await Result.SuccessAsync("Entity successfully saved to the database");
        }

        public async Task<IResult> DeleteAsync(string id)
        {
            if (id == null) return await Result.FailAsync("Id cannot be null");

            var data = await _context.Set<TEntity>().FindAsync(id);

            if (data == null)
                return await Result.FailAsync("Entity not found");

            _context.Set<TEntity>().Remove(data);

            if (!await HasSavedChangesAsync())
            {
                return await Result.FailAsync("Failed to delete entity from database");
            }

            return await Result.SuccessAsync("Entity successfully deleted");
        }

        public async Task<IResult<TEntity>> FindByIdAsync(string id)
        {
            if (id == null) return await Result<TEntity>.FailAsync("Id cannot be null");

            var data = await _context.Set<TEntity>().FindAsync(id);

            if (data == null)
            {
                return await Result<TEntity>.FailAsync("There is no entity available");
            }

            return await Result<TEntity>.SuccessAsync(data, "Successfully retrieved entity from the database");
        }

        public async Task<IResult<IQueryable<TEntity>>> GetAllAsync()
        {
            var data = _context.Set<TEntity>().AsQueryable();

            if (data == null || !data.Any())
            {
                return Result<IQueryable<TEntity>>.Fail("There is no entity available");
            }

            return Result<IQueryable<TEntity>>.Success(data, "Successfully retrieved all entities from the database");
        }

        public async Task<IResult> UpdateAsync(TEntity entity)
        {
            if (entity == null) return await Result<TEntity>.FailAsync("Enitity cannot be null");

            _context.Set<TEntity>().Update(entity);

            if (!await HasSavedChangesAsync())
            {
                return await Result.FailAsync("An error occurred while updating the entity");
            }

            return await Result.SuccessAsync("Entity successfully updated in the database");
        }

    }
}
