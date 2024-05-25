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

public class SeatTypeService(IBaseRepository<SeatType> repository)
    : ISeatTypeService
{
    private readonly IBaseRepository<SeatType> _repository = repository;

    public async Task<SeatType> Add(SeatTypeAddUpdateDto entity)
    {
        try
        {
            return await _repository.Add(entity.CopyTo(new SeatType()));
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

    public async Task<ICollection<SeatTypeListDto>> GetAll()
    {
        try
        {
            return await _repository.GetDbSet()
                .Select(entity => new SeatTypeListDto().CopyFrom(entity))
                .ToListAsync();
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while fetching entities.",
                ex);
        }
    }

    public async Task<SeatType> GetById(int id)
    {
        try
        {
            return await _repository.GetById(id);
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while fetching the entity " +
                $"with ID {id}.", ex);
        }
    }

    public async Task<SeatType> Update(int id, SeatTypeAddUpdateDto entity)
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
