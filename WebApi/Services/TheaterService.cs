using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using WebApi.Exceptions;
using WebApi.Models;
using WebApi.Models.DataTransferObjects;
using WebApi.Repositories;

namespace WebApi.Services;

public interface ITheaterService
{
    public Task<Theater> Add(TheaterAddUpdateDto entity);
    public Task<TheaterAddRemoveFacilitiesDto> AddFacility(
        int id, TheaterAddRemoveFacilitiesDto theaterAddFacilitiesDto);
    public Task Delete(int id);
    public Task<ICollection<TheaterListDto>> GetAll();
    public Task<TheaterGetDto> GetById(int id);
    public Task<TheaterAddRemoveFacilitiesDto> RemoveFacility(
        int id, TheaterAddRemoveFacilitiesDto theaterRemoveFacilitiesDto);
    public Task<Theater> Update(int id, TheaterAddUpdateDto entity);
}

public class TheaterService(
    IBaseRepository<Theater> repository, IBaseRepository<Facility> facilityRepository)
    : ITheaterService
{
    private readonly IBaseRepository<Theater> _repository = repository;
    private readonly IBaseRepository<Facility> _facilityRepository = facilityRepository;

    public async Task<Theater> Add(TheaterAddUpdateDto entity)
    {
        try
        {
            return await _repository.Add(entity.CopyTo(new Theater()));
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occured in the service while adding the entity.",
                ex);
        }
        catch (ValidationException ex)
        {
            throw new ServiceValidationException(
                ex.Message,
                ex);
        }
    }

    public async Task<TheaterAddRemoveFacilitiesDto> AddFacility(
        int id, TheaterAddRemoveFacilitiesDto theaterAddFacilitiesDto)
    {
        try
        {
            var theaterEntity = await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
                .Include(entity => entity.Facilities)
                .FirstAsync();
            theaterEntity.Facilities ??= [];

            foreach (int facilityId in theaterAddFacilitiesDto.FacilityIds)
            {
                theaterEntity.Facilities.Add(
                    await _facilityRepository.GetById(facilityId));
            }

            await _repository.Update(theaterEntity);
            return theaterAddFacilitiesDto;
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while fetching the entity.",
                ex);
        }
        catch (InvalidOperationException)
        {
            throw new ServiceException(
                "The entity with the specified ID does not exist.");
        }
    }

    public async Task<TheaterAddRemoveFacilitiesDto> RemoveFacility(
        int id, TheaterAddRemoveFacilitiesDto theaterRemoveFacilitiesDto)
    {
        try
        {
            var theaterEntity = await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
                .Include(entity => entity.Facilities)
                .FirstAsync();
            theaterEntity.Facilities ??= [];

            foreach (int facilityId in theaterRemoveFacilitiesDto.FacilityIds)
            {
                theaterEntity.Facilities.Remove(
                    await _facilityRepository.GetById(facilityId));
            }

            await _repository.Update(theaterEntity);
            return theaterRemoveFacilitiesDto;
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while fetching the entity.",
                ex);
        }
        catch (InvalidOperationException)
        {
            throw new ServiceException(
                "The entity with the specified ID does not exist.");
        }
    }

    public async Task Delete(int id)
    {
        try
        {
            await _repository.Delete(id);
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while deleting the entity.",
                ex);
        }
    }

    public async Task<ICollection<TheaterListDto>> GetAll()
    {
        try
        {
            return await _repository.GetDbSet()
                .Select(entity => new TheaterListDto().CopyFrom(entity))
                .ToListAsync();
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while fetching entities.",
                ex);
        }
    }

    public async Task<TheaterGetDto> GetById(int id)
    {
        try
        {
            return new TheaterGetDto().CopyFrom(await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
                .Include(entity => entity.Facilities)
                .FirstAsync());
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while fetching the entity " +
                $"with ID {id}.", ex);
        }
        catch (InvalidOperationException)
        {
            throw new ServiceException(
                "The entity with the specified ID does not exist.");
        }
    }

    public async Task<Theater> Update(int id, TheaterAddUpdateDto entity)
    {
        try
        {
            var existingEntity = await _repository.GetById(id);

            return await _repository.Update(entity.CopyTo(existingEntity));
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occured in service while updating the entity.",
                ex);
        }
        catch (ValidationException ex)
        {
            throw new ServiceValidationException(
                ex.Message,
                ex);
        }
    }
}
