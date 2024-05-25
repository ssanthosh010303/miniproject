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

public class FacilityService(IBaseRepository<Facility> repository)
    : IFacilityService
{
    private readonly IBaseRepository<Facility> _repository = repository;

    public async Task<Facility> Add(FacilityAddUpdateDto entity)
    {
        try
        {
            return await _repository.Add(entity.CopyTo(new Facility()));
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

    public async Task<ICollection<FacilityListDto>> GetAll()
    {
        try
        {
            return await _repository.GetDbSet()
                .Select(entity => new FacilityListDto().CopyFrom(entity))
                .ToListAsync();
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while fetching entities.",
                ex);
        }
    }

    public async Task<FacilityGetDto> GetById(int id)
    {
        try
        {
            return new FacilityGetDto().CopyFrom(await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
                .Include(entity => entity.Theaters)
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

    public async Task<Facility> Update(int id, FacilityAddUpdateDto entity)
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
