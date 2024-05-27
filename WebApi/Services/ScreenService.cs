using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using WebApi.Exceptions;
using WebApi.Models;
using WebApi.Models.DataTransferObjects;
using WebApi.Repositories;

namespace WebApi.Services;

public interface IScreenService
{
    public Task<ScreenAddUpdateDto> Add(ScreenAddUpdateDto entity);
    public Task Delete(int id);
    public Task<ICollection<ScreenListDto>> GetAll();
    public Task<ScreenGetDto> GetById(int id);
    public Task<Screen> Update(int id, ScreenAddUpdateDto entity);
}

public class ScreenService(IBaseRepository<Screen> repository)
    : IScreenService
{
    private readonly IBaseRepository<Screen> _repository = repository;

    public async Task<ScreenAddUpdateDto> Add(ScreenAddUpdateDto entity)
    {
        try
        {
            await _repository.Add(entity.CopyTo(new Screen()));
            return entity;
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

    public async Task<ICollection<ScreenListDto>> GetAll()
    {
        try
        {
            return await _repository.GetDbSet()
                .Select(entity => new ScreenListDto().CopyFrom(entity))
                .ToListAsync();
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while fetching entities.",
                ex);
        }
    }

    public async Task<ScreenGetDto> GetById(int id)
    {
        try
        {
            return new ScreenGetDto().CopyFrom(await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
                .Include(entity => entity.Theater)
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

    public async Task<Screen> Update(int id, ScreenAddUpdateDto entity)
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
