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

public interface ISeatTypeService
{
    public Task<SeatType> Add(SeatTypeAddUpdateDto entity);
    public Task Delete(int id);
    public Task<ICollection<SeatTypeListDto>> GetAll();
    public Task<SeatType> GetById(int id);
    public Task<SeatType> Update(int id, SeatTypeAddUpdateDto entity);
}

public class SeatTypeService(
        IBaseRepository<SeatType> repository,
        ILogger<SeatTypeService> logger)
    : ISeatTypeService
{
    private readonly IBaseRepository<SeatType> _repository = repository;
    private readonly ILogger<SeatTypeService> _logger = logger;

    public async Task<SeatType> Add(SeatTypeAddUpdateDto entity)
    {
        _logger.LogInformation("Attempting to add a new seat type.");

        try
        {
            var seatType = await _repository.Add(entity.CopyTo(new SeatType()));

            _logger.LogInformation(
                "Successfully added a new seat type with ID {SeatTypeId}.",
                seatType.Id);
            return seatType;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while adding a new seat type.");
            throw new ServiceException(
                "An error occurred in the service while adding the entity.", ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex,
                "Validation error while adding a new seat type.");
            throw new ServiceValidationException(ex.Message, ex);
        }
    }

    public async Task Delete(int id)
    {
        _logger.LogInformation(
            "Attempting to delete the seat type with ID {SeatTypeId}.",
            id);

        try
        {
            await _repository.Delete(id);

            _logger.LogInformation(
                "Successfully deleted the seat type with ID {SeatTypeId}.",
                id);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while deleting the seat type with ID {SeatTypeId}.",
                id);
            throw new ServiceException(
                "An error occurred in the service while deleting the entity.",
                ex);
        }
    }

    public async Task<ICollection<SeatTypeListDto>> GetAll()
    {
        _logger.LogInformation("Fetching all seat types.");

        try
        {
            var seatTypes = await _repository.GetDbSet()
                .Select(entity => new SeatTypeListDto().CopyFrom(entity))
                .ToListAsync();

            _logger.LogInformation(
                "Successfully fetched {Count} seat types.", seatTypes.Count);
            return seatTypes;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while fetching seat types.");
            throw new ServiceException(
                "An error occurred in the service while fetching entities.", ex);
        }
    }

    public async Task<SeatType> GetById(int id)
    {
        _logger.LogInformation(
            "Fetching the seat type with ID {SeatTypeId}.", id);

        try
        {
            var seatType = await _repository.GetById(id);

            _logger.LogInformation(
                "Successfully fetched the seat type with ID {SeatTypeId}.", id);
            return seatType;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while fetching the seat type with ID {SeatTypeId}.",
                id);
            throw new ServiceException(
                $"An error occurred in the service while fetching the entity with ID {id}.",
                ex);
        }
    }

    public async Task<SeatType> Update(int id, SeatTypeAddUpdateDto entity)
    {
        _logger.LogInformation(
            "Attempting to update the seat type with ID {SeatTypeId}.",
            id);

        try
        {
            var existingEntity = await _repository.GetById(id);
            var updatedSeatType = await _repository.Update(
                entity.CopyTo(existingEntity));

            _logger.LogInformation(
                "Successfully updated the seat type with ID {SeatTypeId}.", id);
            return updatedSeatType;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while updating the seat type with ID {SeatTypeId}.",
                id);
            throw new ServiceException(
                "An error occurred in the service while updating the entity.",
                ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex,
                "Validation error while updating the seat type with ID {SeatTypeId}.",
                id);
            throw new ServiceValidationException(ex.Message, ex);
        }
    }
}
