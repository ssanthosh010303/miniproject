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

public interface ITheaterService
{
    Task<Theater> Add(TheaterAddUpdateDto entity);
    Task Delete(int id);
    Task<ICollection<TheaterListDto>> GetAll();
    Task<TheaterGetDto> GetById(int id);
    Task<Theater> Update(int id, TheaterAddUpdateDto entity);

    Task<TheaterAddRemoveFacilitiesDto> AddFacilities(
        int id, TheaterAddRemoveFacilitiesDto theaterAddFacilitiesDto);
    Task<TheaterAddRemoveFacilitiesDto> RemoveFacilities(
        int id, TheaterAddRemoveFacilitiesDto theaterRemoveFacilitiesDto);
}

public class TheaterService(
        IBaseRepository<Theater> repository,
        IBaseRepository<Facility> facilityRepository,
        IBaseRepository<MovieShow> movieShowRepository,
        ILogger<TheaterService> logger)
    : ITheaterService
{
    private readonly IBaseRepository<Theater> _repository = repository;
    private readonly IBaseRepository<Facility> _facilityRepository = facilityRepository;
    private readonly IBaseRepository<MovieShow> _movieShowRepository = movieShowRepository;
    private readonly ILogger<TheaterService> _logger = logger;

    public async Task<Theater> Add(TheaterAddUpdateDto entity)
    {
        _logger.LogInformation("Starting to add a new theater.");
        try
        {
            var theater = await _repository.Add(entity.CopyTo(new Theater()));

            _logger.LogInformation(
                "Successfully added a new theater with ID {TheaterId}.",
                theater.Id);
            return theater;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the service while adding the theater.");
            throw new ServiceException(
                "An error occurred in the service while adding the theater.",
                ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex,
                "Validation error occurred while adding the theater: {Message}",
                ex.Message);
            throw new ServiceValidationException(ex.Message, ex);
        }
    }

    public async Task<TheaterAddRemoveFacilitiesDto> AddFacilities(
        int id, TheaterAddRemoveFacilitiesDto theaterAddFacilitiesDto)
    {
        _logger.LogInformation(
            "Starting to add facilities to theater with ID {TheaterId}.", id);
        try
        {
            var theaterEntity = await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
                .Include(entity => entity.Facilities)
                .FirstAsync();

            _logger.LogDebug("Theater found: {Theater}", theaterEntity);

            theaterEntity.Facilities ??= [];

            foreach (int facilityId in theaterAddFacilitiesDto.FacilityIds)
            {
                var facility = await _facilityRepository.GetById(facilityId);

                theaterEntity.Facilities.Add(facility);
                _logger.LogInformation(
                    "Added facility with ID {FacilityId} to theater with ID {TheaterId}.",
                    facilityId, id);
            }

            await _repository.Update(theaterEntity);
            return theaterAddFacilitiesDto;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(
                ex,
                "An error occurred in the service while fetching the theater entity.");
            throw new ServiceException(
                "An error occurred in the service while fetching the theater entity.",
                ex);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex,
                "The theater with the specified ID {TheaterId} does not exist.", id);
            throw new ServiceException(
                "The theater with the specified ID does not exist.");
        }
    }

    public async Task<TheaterAddRemoveFacilitiesDto> RemoveFacilities(
        int id, TheaterAddRemoveFacilitiesDto theaterRemoveFacilitiesDto)
    {
        _logger.LogInformation(
            "Starting to remove facilities from theater with ID {TheaterId}.",
            id);
        try
        {
            var theaterEntity = await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
                .Include(entity => entity.Facilities)
                .FirstAsync();

            _logger.LogDebug("Theater found: {Theater}", theaterEntity);

            theaterEntity.Facilities ??= new List<Facility>();

            foreach (int facilityId in theaterRemoveFacilitiesDto.FacilityIds)
            {
                var facility = await _facilityRepository.GetById(facilityId);

                theaterEntity.Facilities.Remove(facility);
                _logger.LogInformation(
                    "Removed facility with ID {FacilityId} from theater with ID {TheaterId}.",
                    facilityId, id);
            }

            await _repository.Update(theaterEntity);
            return theaterRemoveFacilitiesDto;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the service while fetching the theater entity.");
            throw new ServiceException(
                "An error occurred in the service while fetching the theater entity.",
                ex);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex,
                "The theater with the specified ID {TheaterId} does not exist.",
                id);
            throw new ServiceException(
                "The theater with the specified ID does not exist.");
        }
    }

    public async Task Delete(int id)
    {
        _logger.LogInformation(
            "Starting to delete theater with ID {TheaterId}.", id);
        try
        {
            await _repository.Delete(id);

            _logger.LogInformation(
                "Successfully deleted theater with ID {TheaterId}.", id);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the service while deleting the theater.");
            throw new ServiceException(
                "An error occurred in the service while deleting the theater.",
                ex);
        }
    }

    public async Task<ICollection<TheaterListDto>> GetAll()
    {
        _logger.LogInformation("Starting to fetch all theaters.");

        try
        {
            var theaters = await _repository.GetDbSet()
                .Select(entity => new TheaterListDto().CopyFrom(entity))
                .ToListAsync();

            _logger.LogInformation(
                "Successfully fetched all {Count} theaters.", theaters.Count);
            return theaters;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the service while fetching all theaters.");
            throw new ServiceException(
                "An error occurred in the service while fetching all theaters.",
                ex);
        }
    }

    public async Task<TheaterGetDto> GetById(int id)
    {
        _logger.LogInformation(
            "Starting to fetch theater with ID {TheaterId}.", id);

        try
        {
            var theater = await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
                .Include(entity => entity.Facilities)
                .Include(entity => entity.Screens)
                .FirstAsync();

            var theaterDto = new TheaterGetDto().CopyFrom(theater);

            _logger.LogInformation(
                "Successfully fetched theater with ID {TheaterId}.", id);
            return theaterDto;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the service while fetching the theater with ID {TheaterId}.",
                id);
            throw new ServiceException(
                $"An error occurred in the service while fetching the theater with ID {id}.",
                ex);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex,
                "The theater with the specified ID {TheaterId} does not exist.", id);
            throw new ServiceException(
                "The theater with the specified ID does not exist.");
        }
    }

    public async Task<Theater> Update(int id, TheaterAddUpdateDto entity)
    {
        _logger.LogInformation(
            "Starting to update theater with ID {TheaterId}.", id);
        try
        {
            var existingEntity = await _repository.GetById(id);
            var updatedEntity = await _repository.Update(
                entity.CopyTo(existingEntity));

            _logger.LogInformation(
                "Successfully updated theater with ID {TheaterId}.", id);
            return updatedEntity;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the service while updating the theater.");
            throw new ServiceException(
                "An error occurred in the service while updating the theater.",
                ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex,
                "Validation error occurred while updating the theater: {Message}",
                ex.Message);
            throw new ServiceValidationException(ex.Message, ex);
        }
    }
}
