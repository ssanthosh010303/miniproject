/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using WebApi.Exceptions;
using WebApi.Models;
using WebApi.Models.DataTransferObjects;
using WebApi.Repositories;

namespace WebApi.Services;

public interface ICastService
{
    Task<Cast> Add(CastAddUpdateDto entity);
    Task Delete(int id);
    Task<ICollection<CastListDto>> GetAll();
    Task<CastGetDto> GetById(int id);
    Task<Cast> Update(int id, CastAddUpdateDto entity);
}

public class CastService(
        IBaseRepository<Cast> repository, ILogger<CastService> logger)
    : ICastService
{
    private readonly IBaseRepository<Cast> _repository = repository;
    private readonly ILogger<CastService> _logger = logger;

    public async Task<Cast> Add(CastAddUpdateDto entity)
    {
        try
        {
            _logger.LogInformation("Attempting to add a new cast entity.");

            var newEntity = await _repository.Add(entity.CopyTo(new Cast()));

            _logger.LogInformation(
                "Successfully added a new cast entity with ID {Id}.",
                newEntity.Id);
            return newEntity;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(
                ex,
                "Error while adding a new cast entity: {Message}",
                ex.Message);
            throw new ServiceException(
                "An error occurred while adding the cast entity.", ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(
                ex,
                "Validation error while adding a new cast entity: {Message}",
                ex.Message);
            throw new ServiceValidationException(
                "Validation failed for the new cast entity.", ex);
        }
    }

    public async Task Delete(int id)
    {
        try
        {
            _logger.LogInformation(
                "Attempting to delete cast entity with ID {Id}.",
                id);
            await _repository.Delete(id);
            _logger.LogInformation(
                "Successfully deleted cast entity with ID {Id}.",
                id);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(
                ex,
                "Error while deleting cast entity with ID {Id}: {Message}",
                id,
                ex.Message);
            throw new ServiceException(
                "An error occurred while deleting the cast entity.", ex);
        }
    }

    public async Task<ICollection<CastListDto>> GetAll()
    {
        try
        {
            _logger.LogInformation("Fetching all cast entities.");

            var entities = await _repository.GetDbSet()
                .Select(entity => new CastListDto().CopyFrom(entity))
                .ToListAsync();

            _logger.LogInformation(
                "Successfully fetched {Count} cast entities.", entities.Count);
            return entities;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(
                ex,
                "Error while fetching cast entities: {Message}",
                ex.Message);
            throw new ServiceException(
                "An error occurred while fetching cast entities.",
                ex);
        }
    }

    public async Task<CastGetDto> GetById(int id)
    {
        try
        {
            _logger.LogInformation("Fetching cast entity with ID {Id}.", id);

            var entity = await _repository.GetDbSet()
                .Where(e => e.Id == id)
                .Include(e => e.Movies)
                .FirstAsync();

            _logger.LogInformation(
                "Successfully fetched cast entity with ID {Id}.", id);
            return new CastGetDto().CopyFrom(entity);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(
                ex, "Error while fetching cast entity with ID {Id}: {Message}",
                id, ex.Message);
            throw new ServiceException(
                $"An error occurred while fetching the cast entity with ID {id}.",
                ex);
        }
        catch (InvalidOperationException)
        {
            _logger.LogWarning("No cast entity found with ID {Id}.", id);
            throw new ServiceException(
                $"The cast entity with ID {id} does not exist.");
        }
    }

    public async Task<Cast> Update(int id, CastAddUpdateDto entity)
    {
        try
        {
            _logger.LogInformation(
                "Attempting to update cast entity with ID {Id}.", id);

            var existingEntity = await _repository.GetById(id);
            var updatedEntity = await _repository.Update(
                entity.CopyTo(existingEntity));

            _logger.LogInformation(
                "Successfully updated cast entity with ID {Id}.",
                updatedEntity.Id);
            return updatedEntity;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(
                ex, "Error while updating cast entity with ID {Id}: {Message}",
                id, ex.Message);
            throw new ServiceException(
                "An error occurred while updating the cast entity.", ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(
                ex,
                "Validation error while updating cast entity with ID {Id}: {Message}",
                id, ex.Message);
            throw new ServiceValidationException(
                "Validation failed for the cast entity update.", ex);
        }
    }
}
