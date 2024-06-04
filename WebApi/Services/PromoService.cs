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

public interface IPromoService
{
    public Task<Promo> Add(PromoAddUpdateDto entity);
    public Task Delete(int id);
    public Task<ICollection<PromoListDto>> GetAll();
    public Task<PromoGetDto> GetById(int id);
    public Task<Promo> Update(int id, PromoAddUpdateDto entity);
}

public class PromoService(
        IBaseRepository<Promo> repository, ILogger<PromoService> logger)
    : IPromoService
{
    private readonly IBaseRepository<Promo> _repository = repository;
    private readonly ILogger<PromoService> _logger = logger;

    public async Task<Promo> Add(PromoAddUpdateDto entity)
    {
        _logger.LogInformation("Attempting to add a new promo.");

        try
        {
            var promo = entity.CopyTo(new Promo());
            var result = await _repository.Add(promo);
            _logger.LogInformation(
                "Successfully added a new promo with ID {Id}.",
                result.Id);
            return result;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
            "An error occurred in the repository while adding the promo.");
            throw new ServiceException(
                "An error occurred in the service while adding the entity.", ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation error while adding the promo.");
            throw new ServiceValidationException(ex.Message, ex);
        }
    }

    public async Task Delete(int id)
    {
        _logger.LogInformation(
            "Attempting to delete the promo with ID {Id}.", id);

        try
        {
            await _repository.Delete(id);
            _logger.LogInformation(
                "Successfully deleted the promo with ID {Id}.", id);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while deleting the promo with ID {Id}.",
                id);
            throw new ServiceException(
                "An error occurred in the service while deleting the entity.",
                ex);
        }
    }

    public async Task<ICollection<PromoListDto>> GetAll()
    {
        _logger.LogInformation("Fetching all promos.");

        try
        {
            var promos = await _repository.GetDbSet()
                .Select(entity => new PromoListDto().CopyFrom(entity))
                .ToListAsync();

            _logger.LogInformation(
                "Successfully fetched {Count} promos.", promos.Count);
            return promos;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while fetching promos.");
            throw new ServiceException(
                "An error occurred in the service while fetching entities.", ex);
        }
    }

    public async Task<PromoGetDto> GetById(int id)
    {
        _logger.LogInformation("Fetching the promo with ID {Id}.", id);

        try
        {
            var promo = await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
                .FirstAsync();

            _logger.LogInformation(
                "Successfully fetched the promo with ID {Id}.", id);
            return new PromoGetDto().CopyFrom(promo);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while fetching the promo with ID {Id}.",
                id);
            throw new ServiceException(
                $"An error occurred in the service while fetching the entity with ID {id}.",
                ex);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(
                ex, "The promo with ID {Id} does not exist.", id);
            throw new ServiceException(
                "The entity with the specified ID does not exist.");
        }
    }

    public async Task<Promo> Update(int id, PromoAddUpdateDto entity)
    {
        _logger.LogInformation(
            "Attempting to update the promo with ID {Id}.", id);

        try
        {
            var existingEntity = await _repository.GetById(id);
            var updatedEntity = entity.CopyTo(existingEntity);
            var result = await _repository.Update(updatedEntity);

            _logger.LogInformation(
                "Successfully updated the promo with ID {Id}.", result.Id);
            return result;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while updating the promo with ID {Id}.",
                id);
            throw new ServiceException(
                "An error occurred in the service while updating the entity.",
                ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex,
                "Validation error while updating the promo with ID {Id}.", id);
            throw new ServiceValidationException(ex.Message, ex);
        }
    }
}
