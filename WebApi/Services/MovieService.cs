using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using WebApi.Exceptions;
using WebApi.Models;
using WebApi.Models.DataTransferObjects;
using WebApi.Repositories;

namespace WebApi.Services;

public interface IMovieService
{
    public Task<Movie> Add(MovieAddUpdateDto entity);
    public Task<MovieAddRemoveCastMembersDto> AddCast(
        int id, MovieAddRemoveCastMembersDto movieAddCastDto);
    public Task<MovieAddRemoveCastMembersDto> RemoveCast(int id, MovieAddRemoveCastMembersDto movieAddCastDto);
    public Task Delete(int id);
    public Task<ICollection<MovieListDto>> GetAll(MovieGenre genre);
    public Task<MovieGetDto> GetById(int id);
    public Task<Movie> Update(int id, MovieAddUpdateDto entity);
}

public class MovieService(
    IBaseRepository<Movie> repository, IBaseRepository<Cast> castRepository)
    : IMovieService
{
    private readonly IBaseRepository<Movie> _repository = repository;
    private readonly IBaseRepository<Cast> _castRepository = castRepository;

    public async Task<Movie> Add(MovieAddUpdateDto entity)
    {
        try
        {
            return await _repository.Add(entity.CopyTo(new Movie()));
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
                "Validation failed when adding the entity.",
                ex);
        }
    }

    public async Task<MovieAddRemoveCastMembersDto> AddCast(
        int id, MovieAddRemoveCastMembersDto movieAddCastDto)
    {
        try
        {
            var movieEntity = await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
                .Include(entity => entity.CastMembers)
                .FirstAsync();
            movieEntity.CastMembers ??= [];

            foreach (int castId in movieAddCastDto.CastMemberIds)
            {
                movieEntity.CastMembers.Add(await _castRepository.GetById(castId));
            }

            await _repository.Update(movieEntity);
            return movieAddCastDto;
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while fetching the entity.",
                ex);
        }
    }

    public async Task<MovieAddRemoveCastMembersDto> RemoveCast(
        int id, MovieAddRemoveCastMembersDto movieRemoveCastDto)
    {
        try
        {
            var movieEntity = await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
                .Include(entity => entity.CastMembers)
                .FirstAsync();

            if (movieEntity.CastMembers == null || movieEntity.CastMembers.Count == 0)
                throw new ServiceException(
                    "There are no cast members for this movie.");

            foreach (int castId in movieRemoveCastDto.CastMemberIds)
            {
                movieEntity.CastMembers.Remove(
                    await _castRepository.GetById(castId));
            }

            await _repository.Update(movieEntity);
            return movieRemoveCastDto;
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

    public async Task<ICollection<MovieListDto>> GetAll(MovieGenre genre)
    {
        try
        {
            if (genre != MovieGenre.All)
            {
                return await _repository.GetDbSet()
                    .Where(entity => entity.Genre == genre)
                    .Select(entity => new MovieListDto().CopyFrom(entity))
                    .ToListAsync();
            }

            return await _repository.GetDbSet()
                .Select(entity => new MovieListDto().CopyFrom(entity))
                .ToListAsync();
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while fetching entities.",
                ex);
        }
    }

    public async Task<MovieGetDto> GetById(int id)
    {
        try
        {
            return new MovieGetDto().CopyFrom(await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
                .Include(entity => entity.CastMembers) // TODO: Projection for Cast Members
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

    public async Task<Movie> Update(int id, MovieAddUpdateDto entity)
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
