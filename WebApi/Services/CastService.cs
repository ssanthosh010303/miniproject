using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using WebApi.Exceptions;
using WebApi.Models;
using WebApi.Models.DataTransferObjects;
using WebApi.Repositories;

namespace WebApi.Services;

public interface ICastService
{
    public Task<Cast> Add(CastAddUpdateDto entity);
    public Task Delete(int id);
    public Task<ICollection<CastListDto>> GetAll();
    public Task<CastGetDto> GetById(int id);
    public Task<Cast> Update(int id, CastAddUpdateDto entity);
}

public class CastService(IBaseRepository<Cast> repository)
    : ICastService
{
    private readonly IBaseRepository<Cast> _repository = repository;

    public async Task<Cast> Add(CastAddUpdateDto entity)
    {
        try
        {
            return await _repository.Add(entity.CopyTo(new Cast()));
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

    public async Task<ICollection<CastListDto>> GetAll()
    {
        try
        {
            return await _repository.GetDbSet()
                .Select(entity => new CastListDto().CopyFrom(entity))
                .ToListAsync();
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while fetching entities.",
                ex);
        }
    }

    public async Task<CastGetDto> GetById(int id)
    {
        try
        {
            return new CastGetDto().CopyFrom(await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
                .Include(entity => entity.Movies)
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

    public async Task<Cast> Update(int id, CastAddUpdateDto entity)
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
