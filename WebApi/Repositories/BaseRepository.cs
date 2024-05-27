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
    private readonly ILogger<BaseRepository<TEntity>> _logger;

    public BaseRepository(
        ApplicationDbContext applicationDbContext,
        ILogger<BaseRepository<TEntity>> logger)
    {
        _applicationDbContext = applicationDbContext;
        _logger = logger;
    }

    public DbSet<TEntity> GetDbSet()
    {
        // Service Layer Projects Data
        _logger.LogInformation("Retrieving entries from the database.");
        return _applicationDbContext.Set<TEntity>();
    }

    public virtual async Task<TEntity> GetById(int id)
    {
        var data = await _applicationDbContext.Set<TEntity>().FindAsync(id)
            ?? throw new RepositoryException();

        _logger.LogInformation($"Retrieved entry with ID {id}.");
        return data;
    }

    public virtual async Task<TEntity> Add(TEntity entity)
    {
        try
        {
            await _applicationDbContext.Set<TEntity>().AddAsync(entity);
            await _applicationDbContext.SaveChangesAsync();
            _logger.LogInformation("Added a new entry to the database.");
            return entity;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to add a new entry to the database.");
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

            _logger.LogInformation("Updating an entry in the database.");
            await _applicationDbContext.SaveChangesAsync();
            return entity;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "A concurrency conflict was detected while updating.");
            throw new RepositoryException(
                "Error occurred while updating because a concurrency " +
                "conflict was detected.", ex);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "A database update exception occured.");
            throw new RepositoryException(
                "A database update exception occured.", ex);
        }
    }

    public virtual async Task Delete(int id)
    {
        try
        {
            _logger.LogInformation("Deleting an entry from the database.");
            _applicationDbContext.Set<TEntity>().Remove(await GetById(id));
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to delete an entry from the database.");
            throw new RepositoryException(
                "Error occurred while deleting entity.", ex);
        }
    }
}
