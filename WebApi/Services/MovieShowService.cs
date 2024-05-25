using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using WebApi.Exceptions;
using WebApi.Models;
using WebApi.Models.DataTransferObjects;
using WebApi.Repositories;

namespace WebApi.Services;

public interface IMovieShowService
{
    public Task<MovieShow> Add(MovieShowAddUpdateDto entity);
    public Task Delete(int id);
    public Task<ICollection<MovieShowListDto>> GetAll();
    public Task<MovieShowGetDto> GetById(int id);
    public Task<MovieShow> Update(int id, MovieShowAddUpdateDto entity);
}

public class MovieShowService(IBaseRepository<MovieShow> repository)
    : IMovieShowService
{
    private readonly IBaseRepository<MovieShow> _repository = repository;

    public async Task<MovieShow> Add(MovieShowAddUpdateDto entity)
    {
        try
        {
            return await _repository.Add(entity.CopyTo(new MovieShow()));
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

    public async Task<ICollection<MovieShowListDto>> GetAll()
    {
        try
        {
            return await _repository.GetDbSet()
                .Include(entity => entity.Movie)
                .Select(entity => new MovieShowListDto().CopyFrom(entity))
                .ToListAsync();
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while fetching entities.",
                ex);
        }
    }

    public async Task<MovieShowGetDto> GetById(int id)
    {
        try
        {
            return new MovieShowGetDto().CopyFrom(await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
                .Include(entity => entity.Movie)
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

    public async Task<MovieShow> Update(int id, MovieShowAddUpdateDto entity)
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
