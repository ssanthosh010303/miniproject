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

public interface IMovieService
{
    public Task<Movie> Add(MovieAddUpdateDto entity);
    public Task<MovieAddRemoveCastMembersDto> AddCast(
        int id, MovieAddRemoveCastMembersDto movieAddCastDto);
    public Task<MovieAddRemoveCastMembersDto> RemoveCast(
        int id, MovieAddRemoveCastMembersDto movieRemoveCastDto);
    public Task Delete(int id);
    public Task<ICollection<MovieListDto>> GetAll(MovieGenre genre);
    public Task<MovieGetDto> GetById(int id);
    public Task<Movie> Update(int id, MovieAddUpdateDto entity);
}

public class MovieService(
        IBaseRepository<Movie> repository,
        IBaseRepository<Cast> castRepository,
        ILogger<MovieService> logger)
    : IMovieService
{
    private readonly IBaseRepository<Movie> _repository = repository;
    private readonly IBaseRepository<Cast> _castRepository = castRepository;
    private readonly ILogger<MovieService> _logger = logger;

    public async Task<Movie> Add(MovieAddUpdateDto entity)
    {
        try
        {
            _logger.LogInformation("Adding a new movie entity.");

            var newEntity = await _repository.Add(entity.CopyTo(new Movie()));

            _logger.LogInformation(
                "Successfully added a new movie with ID {Id}.", newEntity.Id);
            return newEntity;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(
                ex, "Error while adding a new movie: {Message}", ex.Message);
            throw new ServiceException(
                "An error occurred while adding the movie.", ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(
                ex, "Validation error while adding a new movie: {Message}",
                ex.Message);
            throw new ServiceValidationException(
                "Validation failed for the new movie.", ex);
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
                movieEntity.CastMembers.Add(
                    await _castRepository.GetById(castId));
            }

            await _repository.Update(movieEntity);
            _logger.LogInformation(
                "Added cast members to the movie with ID {Id}.", id);
            return movieAddCastDto;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(
                ex, "Error while adding cast to movie with ID {Id}: {Message}",
                id, ex.Message);
            throw new ServiceException(
                "An error occurred while adding cast to the movie.", ex);
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

            if (movieEntity.CastMembers == null
                || movieEntity.CastMembers.Count == 0)
                throw new ServiceException(
                    "There are no cast members for this movie.");

            foreach (int castId in movieRemoveCastDto.CastMemberIds)
            {
                movieEntity.CastMembers.Remove(
                    await _castRepository.GetById(castId));
            }

            await _repository.Update(movieEntity);
            _logger.LogInformation(
                "Removed cast members from the movie with ID {Id}.", id);
            return movieRemoveCastDto;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(
                ex,
                "Error while removing cast from movie with ID {Id}: {Message}",
                id, ex.Message);
            throw new ServiceException(
                "An error occurred while removing cast from the movie.", ex);
        }
        catch (InvalidOperationException)
        {
            _logger.LogError(
                "The movie with the specified ID {Id} does not exist.", id);
            throw new ServiceException(
                "The movie with the specified ID does not exist.");
        }
    }

    public async Task Delete(int id)
    {
        try
        {
            _logger.LogInformation("Deleting movie with ID {Id}.", id);

            await _repository.Delete(id);

            _logger.LogInformation(
                "Successfully deleted movie with ID {Id}.", id);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(
                ex, "Error while deleting movie with ID {Id}: {Message}",
                id, ex.Message);
            throw new ServiceException(
                "An error occurred while deleting the movie.",
                ex);
        }
    }

    public async Task<ICollection<MovieListDto>> GetAll(MovieGenre genre)
    {
        try
        {
            _logger.LogInformation("Fetching all movie entities.");

            if (genre != 0)
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
            _logger.LogError(ex, "Error while fetching movies: {Message}",
                ex.Message);
            throw new ServiceException(
                "An error occurred while fetching movies.", ex);
        }
    }

    public async Task<MovieGetDto> GetById(int id)
    {
        try
        {
            _logger.LogInformation("Fetching movie with ID {Id}.", id);

            var movieEntity = await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
                .Include(entity => entity.CastMembers)
                .FirstAsync();

            _logger.LogInformation(
                "Successfully fetched movie with ID {Id}.", id);
            return new MovieGetDto().CopyFrom(movieEntity);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(
                ex, "Error while fetching movie with ID {Id}: {Message}",
                id, ex.Message);
            throw new ServiceException(
                "An error occurred while fetching the movie.",
                ex);
        }
        catch (InvalidOperationException)
        {
            _logger.LogError(
                "The movie with the specified ID {Id} does not exist.",
                id);
            throw new ServiceException(
                "The movie with the specified ID does not exist.");
        }
    }

    public async Task<Movie> Update(int id, MovieAddUpdateDto entity)
    {
        try
        {
            _logger.LogInformation("Updating movie with ID {Id}.", id);

            var existingEntity = await _repository.GetById(id);
            var updatedEntity = await _repository.Update(
                entity.CopyTo(existingEntity));

            _logger.LogInformation("Successfully updated movie with ID {Id}.",
                updatedEntity.Id);
            return updatedEntity;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(
                ex, "Error while updating movie with ID {Id}: {Message}",
                id, ex.Message);
            throw new ServiceException(
                "An error occurred while updating the movie.",
                ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(
                ex,
                "Validation error while updating movie with ID {Id}: {Message}",
                id, ex.Message);
            throw new ServiceValidationException(
                "Validation failed for the movie update.", ex);
        }
    }
}
