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

public interface IMovieShowService
{
    public Task<MovieShow> Add(MovieShowAddUpdateDto entity);
    public Task Delete(int id);
    public Task<ICollection<MovieShowListDto>> GetAll(
        MovieGenre genre = 0,
        float rating = 0, bool isUpcoming = true, int limitResultsBy = -1);
    public Task<MovieShowGetDto> GetById(int id);
    public Task<MovieShow> Update(int id, MovieShowAddUpdateDto entity);
}

public class MovieShowService(
        IBaseRepository<MovieShow> repository, ILogger<MovieShowService> logger,
        IBaseRepository<Movie> movieRepository)
    : IMovieShowService
{
    private readonly IBaseRepository<MovieShow> _repository = repository;
    private readonly IBaseRepository<Movie> _movieRepository = movieRepository;
    private readonly ILogger<MovieShowService> _logger = logger;

    public async Task<MovieShow> Add(MovieShowAddUpdateDto entity)
    {
        _logger.LogInformation("Attempting to add a new movie show.");

        try
        {
            var movie = await _movieRepository.GetById(entity.MovieId);
            var movieShow = entity.CopyTo(new MovieShow());

            movieShow.BlockDurationAfterMovieShowMins =
                movie.Runtime.Hour * 60 + movie.Runtime.Minute;

            var result = await _repository.Add(movieShow);

            return result;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while fetching the movie with ID {Id}.",
                entity.MovieId);
            throw new ServiceException(
                $"An error occurred in the service while fetching the entity with ID {entity.MovieId}.",
                ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation error while adding the movie show.");
            throw new ServiceValidationException(ex.Message, ex);
        }
    }

    public async Task Delete(int id)
    {
        _logger.LogInformation(
            "Attempting to delete the movie show with ID {Id}.", id);

        try
        {
            await _repository.Delete(id);
            _logger.LogInformation(
                "Successfully deleted the movie show with ID {Id}.", id);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while deleting the movie show with ID {Id}.",
                id);
            throw new ServiceException(
                "An error occurred in the service while deleting the entity.",
                ex);
        }
    }

    public async Task<ICollection<MovieShowListDto>> GetAll(
        MovieGenre genre = 0,
        float rating = 0, bool isUpcoming = true, int limitResultsBy = -1)
    {
        _logger.LogInformation("Fetching all upcoming movie shows.");

        try
        {
            IQueryable<MovieShow> queryObject = _repository.GetDbSet();

            if (isUpcoming)
            {
                queryObject = queryObject
                .Where(entity => entity.ShowTime >= DateTime.UtcNow)
                .Include(entity => entity.Movie);
            }

            if (genre != 0)
            {
                queryObject = queryObject
                    .Include(entity => entity.Movie)
                    .Where(entity => entity.Movie.Genre == genre);
            }

            if (rating > 0)
            {
                if (genre != 0)
                {
                    queryObject = queryObject
                        .Include(entity => entity.Movie);
                }

                queryObject = queryObject
                    .Where(entity => entity.Movie.Rating >= rating);
            }

            if (limitResultsBy != -1)
            {
                queryObject = queryObject.Take(limitResultsBy);
            }

            var movieShows = await queryObject
                .Select(entity => new MovieShowListDto().CopyFrom(entity))
                .ToListAsync();

            _logger.LogInformation(
                "Successfully fetched {Count} upcoming movie shows.",
                movieShows.Count);
            return movieShows;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
            "An error occurred in the repository while fetching movie shows.");
            throw new ServiceException(
                "An error occurred in the service while fetching entities.", ex);
        }
    }

    public async Task<MovieShowGetDto> GetById(int id)
    {
        _logger.LogInformation("Fetching the movie show with ID {Id}.", id);

        try
        {
            var movieShow = await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
                .Include(entity => entity.Movie)
                .FirstAsync();

            _logger.LogInformation(
                "Successfully fetched the movie show with ID {Id}.", id);
            return new MovieShowGetDto().CopyFrom(movieShow);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while fetching the movie show with ID {Id}.",
                id);
            throw new ServiceException(
                $"An error occurred in the service while fetching the entity with ID {id}.",
                ex);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex,
            "The movie show with ID {Id} does not exist.", id);
            throw new ServiceException(
                "The entity with the specified ID does not exist.");
        }
    }

    public async Task<MovieShow> Update(int id, MovieShowAddUpdateDto entity)
    {
        _logger.LogInformation(
            "Attempting to update the movie show with ID {Id}.", id);

        try
        {
            var existingEntity = await _repository.GetById(id);
            var updatedEntity = entity.CopyTo(existingEntity);
            var result = await _repository.Update(updatedEntity);

            _logger.LogInformation(
                "Successfully updated the movie show with ID {Id}.", result.Id);
            return result;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while updating the movie show with ID {Id}.",
                id);
            throw new ServiceException(
                "An error occurred in the service while updating the entity.",
                ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex,
                "Validation error while updating the movie show with ID {Id}.",
                id);
            throw new ServiceValidationException(ex.Message, ex);
        }
    }
}
