using Microsoft.EntityFrameworkCore;
using System.Data.Common;

using WebApi.Exceptions;
using WebApi.Models;

namespace WebApi.Repositories;

public interface IBaseRepository<TEntity>
    where TEntity : BaseModel
{
    public DbSet<TEntity> GetDbSet();
    public Task<TEntity> GetById(int id);
    public Task<TEntity> Add(TEntity entity);
    public Task<TEntity> Update(TEntity entity);
    public Task Delete(int id);
}

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : BaseModel
{
    private readonly ApplicationDbContext _applicationDbContext;

    public BaseRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public DbSet<TEntity> GetDbSet()
    {
        try
        {
            // Service Layer Projects Data
            return _applicationDbContext.Set<TEntity>();
        }
        catch (DbException ex)
        {
            throw new RepositoryException(
                "Unable to retrieve entries from the database.", ex);
        }
    }

    public virtual async Task<TEntity> GetById(int id)
    {
        try
        {
            var data = await _applicationDbContext.Set<TEntity>().FindAsync(id)
                ?? throw new RepositoryException();

            return data;
        }
        catch (DbException ex)
        {
            throw new RepositoryException(
                $"Unable to retrieve entry with ID {id}.", ex);
        }
    }

    public virtual async Task<TEntity> Add(TEntity entity)
    {
        try
        {
            await _applicationDbContext.Set<TEntity>().AddAsync(entity);
            await _applicationDbContext.SaveChangesAsync();
            return entity;
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException(
                "A database update exception has occured.", ex);
        }
    }

    public virtual async Task<TEntity> Update(TEntity entity)
    {
        try
        {
            entity.UpdatedOn = DateTime.UtcNow; // TODO: Update Only on Successful Change

            _applicationDbContext.Attach(entity);
            _applicationDbContext.Entry(entity).State = EntityState.Modified;

            await _applicationDbContext.SaveChangesAsync();
            return entity;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new RepositoryException(
                "Error occurred while updating because a concurrency " +
                "conflict was detected.", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException(
                "A database update exception occured.", ex);
        }
    }

    public virtual async Task Delete(int id)
    {
        try
        {
            _applicationDbContext.Set<TEntity>().Remove(await GetById(id));
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException(
                "Error occurred while deleting entity.", ex);
        }
    }
}
