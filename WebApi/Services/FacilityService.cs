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

public interface IFacilityService
{
    public Task<Facility> Add(FacilityAddUpdateDto entity);
    public Task Delete(int id);
    public Task<ICollection<FacilityListDto>> GetAll();
    public Task<FacilityGetDto> GetById(int id);
    public Task<Facility> Update(int id, FacilityAddUpdateDto entity);
}

public class FacilityService(
    IBaseRepository<Facility> repository, ILogger<FacilityService> logger)
    : IFacilityService
{
    private readonly IBaseRepository<Facility> _repository = repository;
    private readonly ILogger<FacilityService> _logger = logger;

    public async Task<Facility> Add(FacilityAddUpdateDto entity)
    {
        try
        {
            _logger.LogInformation("Adding a new facility entity.");

            var newEntity = await _repository.Add(entity.CopyTo(new Facility()));

            _logger.LogInformation(
                "Successfully added a new facility with ID {Id}.", newEntity.Id);
            return newEntity;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(
                ex, "Error while adding a new facility: {Message}",
                ex.Message);
            throw new ServiceException(
                "An error occurred while adding the facility.", ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(
                ex, "Validation error while adding a new facility: {Message}",
                ex.Message);
            throw new ServiceValidationException(
                "Validation failed for the new facility.", ex);
        }
    }

    public async Task Delete(int id)
    {
        try
        {
            _logger.LogInformation("Deleting facility with ID {Id}.", id);

            await _repository.Delete(id);

            _logger.LogInformation(
                "Successfully deleted facility with ID {Id}.", id);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(
                ex, "Error while deleting facility with ID {Id}: {Message}",
                id, ex.Message);
            throw new ServiceException(
                "An error occurred while deleting the facility.", ex);
        }
    }

    public async Task<ICollection<FacilityListDto>> GetAll()
    {
        try
        {
            _logger.LogInformation("Fetching all facility entities.");

            var entities = await _repository.GetDbSet()
                .Select(entity => new FacilityListDto().CopyFrom(entity))
                .ToListAsync();

            _logger.LogInformation(
                "Successfully fetched {Count} facility entities.",
                entities.Count);
            return entities;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex, "Error while fetching facilities: {Message}",
                ex.Message);
            throw new ServiceException(
                "An error occurred while fetching facilities.", ex);
        }
    }

    public async Task<FacilityGetDto> GetById(int id)
    {
        try
        {
            _logger.LogInformation("Fetching facility with ID {Id}.", id);

            var entity = await _repository.GetDbSet()
                .Where(e => e.Id == id)
                .Include(e => e.Theaters)
                .FirstAsync();

            _logger.LogInformation(
                "Successfully fetched facility with ID {Id}.", id);
            return new FacilityGetDto().CopyFrom(entity);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(
                ex, "Error while fetching facility with ID {Id}: {Message}",
                id, ex.Message);
            throw new ServiceException(
                $"An error occurred while fetching the facility " +
                $"with ID {id}.", ex);
        }
        catch (InvalidOperationException)
        {
            _logger.LogWarning("No facility found with ID {Id}.", id);
            throw new ServiceException(
                $"The facility with ID {id} does not exist.");
        }
    }

    public async Task<Facility> Update(int id, FacilityAddUpdateDto entity)
    {
        try
        {
            _logger.LogInformation("Updating facility with ID {Id}.", id);

            var existingEntity = await _repository.GetById(id);
            var updatedEntity = await _repository.Update(
                entity.CopyTo(existingEntity));

            _logger.LogInformation(
                "Successfully updated facility with ID {Id}.",
                updatedEntity.Id);
            return updatedEntity;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(
                ex, "Error while updating facility with ID {Id}: {Message}",
                id, ex.Message);
            throw new ServiceException(
                "An error occurred while updating the facility.",
                ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(
                ex, "Validation error while updating facility with ID {Id}: " +
                "{Message}", id, ex.Message);
            throw new ServiceValidationException(
                "Validation failed for the facility update.",
                ex);
        }
    }
}
