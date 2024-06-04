/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
using Microsoft.EntityFrameworkCore;

using WebApi.Exceptions;
using WebApi.Models;

namespace WebApi.Repositories;

public interface IBaseRepository<TEntity> where TEntity : BaseModel
{
    DbSet<TEntity> GetDbSet();
    Task<TEntity> GetById(int id);
    Task<TEntity> Add(TEntity entity);
    Task<TEntity> Update(TEntity entity);
    Task Delete(int id);
    Task SaveChanges();
}

public abstract class BaseRepository<TEntity>(
        ApplicationDbContext applicationDbContext,
        ILogger<BaseRepository<TEntity>> logger)
    : IBaseRepository<TEntity> where TEntity : BaseModel
{
    private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
    private readonly ILogger<BaseRepository<TEntity>> _logger = logger;

    public DbSet<TEntity> GetDbSet()
    {
        _logger.LogInformation(
            "Retrieving DbSet for entity {EntityName}.", typeof(TEntity).Name);
        return _applicationDbContext.Set<TEntity>();
    }

    public virtual async Task<TEntity> GetById(int id)
    {
        var data = await _applicationDbContext.Set<TEntity>().FindAsync(id)
            ?? throw new RepositoryException(
                $"Entity {typeof(TEntity).Name} with ID {id} not found.");

        _logger.LogInformation(
            "Retrieved entity {EntityName} with ID {EntityId}.",
            typeof(TEntity).Name, id);
        return data;
    }

    public virtual async Task<TEntity> Add(TEntity entity)
    {
        try
        {
            await _applicationDbContext.Set<TEntity>().AddAsync(entity);
            await _applicationDbContext.SaveChangesAsync();

            _logger.LogInformation(
                "Added a new {EntityName} to the database.", typeof(TEntity).Name);
            return entity;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(
                ex,
                "Failed to add a new {EntityName} to the database.",
                typeof(TEntity).Name);
            throw new RepositoryException(
                $"Error occurred while adding a new {typeof(TEntity).Name} to the database.",
                ex);
        }
    }

    public virtual async Task<TEntity> Update(TEntity entity)
    {
        try
        {
            entity.UpdatedOn = DateTime.UtcNow; // TODO: Update Only on Successful Change

            _applicationDbContext.Attach(entity);
            _applicationDbContext.Entry(entity).State = EntityState.Modified;
            _logger.LogInformation(
                "Updating {EntityName} in the database.", typeof(TEntity).Name);

            await _applicationDbContext.SaveChangesAsync();
            return entity;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(
                ex,
                "Concurrency conflict detected while updating {EntityName}.",
                typeof(TEntity).Name);
            throw new RepositoryException(
                $"Error occurred while updating {typeof(TEntity).Name} due to a concurrency conflict.",
                ex);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(
                ex,
                "Database update exception occurred while updating {EntityName}.",
                typeof(TEntity).Name);
            throw new RepositoryException(
                $"Database update exception occurred while updating {typeof(TEntity).Name}.",
                ex);
        }
    }

    public virtual async Task Delete(int id)
    {
        try
        {
            _logger.LogInformation(
                "Deleting {EntityName} from the database.", typeof(TEntity).Name);

            var entity = await GetById(id);

            _applicationDbContext.Set<TEntity>().Remove(entity);
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(
                ex,
                "Failed to delete {EntityName} from the database.",
                typeof(TEntity).Name);
            throw new RepositoryException(
                $"Error occurred while deleting {typeof(TEntity).Name} from the database.",
                ex);
        }
    }

    public async Task SaveChanges()
    {
        try
        {
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to save changes to the database.");
            throw new RepositoryException(
                "Error occurred while saving changes to the database.", ex);
        }
    }
}
