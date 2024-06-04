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

public interface IScreenService
{
    public Task<ScreenAddUpdateDto> Add(ScreenAddUpdateDto entity);
    public Task Delete(int id);
    public Task<ICollection<ScreenListDto>> GetAll();
    public Task<ScreenGetDto> GetById(int id);
    public Task<Screen> Update(int id, ScreenAddUpdateDto entity);

    Task<ScreenAddRemoveMovieShowsDto> AddMovieShows(
        int id, ScreenAddRemoveMovieShowsDto theaterAddMovieShowDto);
    Task<ScreenAddRemoveMovieShowsDto> RemoveMovieShows(
        int id, ScreenAddRemoveMovieShowsDto theaterAddMovieShowDto);
}

public class ScreenService(
        IBaseRepository<Screen> repository,
        ILogger<ScreenService> logger,
        IBaseRepository<MovieShow> movieShowRepository)
    : IScreenService
{
    private readonly IBaseRepository<Screen> _repository = repository;
    private readonly ILogger<ScreenService> _logger = logger;
    private readonly IBaseRepository<MovieShow> _movieShowRepository = movieShowRepository;

    public async Task<ScreenAddUpdateDto> Add(ScreenAddUpdateDto entity)
    {
        _logger.LogInformation("Attempting to add a new screen.");

        try
        {
            await _repository.Add(entity.CopyTo(new Screen()));

            _logger.LogInformation("Successfully added a new screen.");
            return entity;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
            "An error occurred in the repository while adding the screen.");
            throw new ServiceException(
                "An error occurred in the service while adding the entity.",
                ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation error while adding the screen.");
            throw new ServiceValidationException(ex.Message, ex);
        }
    }

    public async Task<ScreenAddRemoveMovieShowsDto> AddMovieShows(
        int id, ScreenAddRemoveMovieShowsDto theaterAddMovieShowDto)
    {
        _logger.LogInformation(
            "Attempting to add movie shows to screen with ID {Id}.", id);

        try
        {
            Screen screen = await _repository.GetDbSet()
                .Include(entity => entity.MovieShows)
                .Where(entity => entity.Id == id)
                .FirstAsync();
            ICollection<MovieShow> movieShowsInFuture = screen.MovieShows
                .Where(entity => entity.ShowTime > DateTime.UtcNow)
                .ToList();

            foreach (int yetToAddMovieShowId in theaterAddMovieShowDto.MovieShowIds)
            {
                _logger.LogInformation(
                    "Attempting to add movie show with ID {MovieShowId} to screen with ID {ScreenId}.",
                    yetToAddMovieShowId, id);

                try
                {
                    var YetToAddMovieShow = await _movieShowRepository.GetDbSet()
                        .Include(entity => entity.Movie)
                        .Where(entity => entity.Id == yetToAddMovieShowId)
                        .FirstAsync();
                    var YetToAddMovieShowBlockTimeStart = YetToAddMovieShow.ShowTime.AddMinutes(
                        -YetToAddMovieShow.BlockDurationBeforeMovieShowMins);
                    var YetToAddMovieShowBlockTimeEnd = YetToAddMovieShow.ShowTime.AddMinutes(
                        YetToAddMovieShow.BlockDurationAfterMovieShowMins);

                    foreach (var movieShowinFuture in movieShowsInFuture)
                    {
                        var movieShowInFutureBlockTimeStart = movieShowinFuture.ShowTime.AddMinutes(
                            -movieShowinFuture.BlockDurationBeforeMovieShowMins);
                        var movieShowInFutureBlockTimeEnd = movieShowinFuture.ShowTime.AddMinutes(
                            movieShowinFuture.BlockDurationAfterMovieShowMins);

                        if ((YetToAddMovieShowBlockTimeStart > movieShowInFutureBlockTimeStart &&
                                YetToAddMovieShowBlockTimeStart < movieShowInFutureBlockTimeEnd) ||
                            (YetToAddMovieShowBlockTimeEnd > movieShowInFutureBlockTimeStart &&
                                YetToAddMovieShowBlockTimeEnd < movieShowInFutureBlockTimeEnd))
                        {
                            _logger.LogError(
                                "The movie show with ID {MovieShowId} overlaps with another movie show.",
                                yetToAddMovieShowId);
                            throw new ServiceException(
                                $"The movie show with ID {yetToAddMovieShowId} overlaps with another movie show.");
                        }
                    }

                    if (screen.AudioTech != YetToAddMovieShow.Movie.AudioTech)
                    {
                        _logger.LogError(
                            "The audio tech of the movie show with ID {MovieShowId} does not match the screen's audio tech.",
                            yetToAddMovieShowId);
                        throw new ServiceException(
                            $"The audio tech of the movie show with ID {yetToAddMovieShowId} does not match the screen's audio tech.");
                    }

                    if (screen.DisplayTech != YetToAddMovieShow.Movie.DisplayTech)
                    {
                        _logger.LogError(
                            "The display tech of the movie show with ID {MovieShowId} does not match the screen's display tech.",
                            yetToAddMovieShowId);
                        throw new ServiceException(
                            $"The display tech of the movie show with ID {yetToAddMovieShowId} does not match the screen's display tech.");
                    }

                    screen.MovieShows.Add(YetToAddMovieShow);
                }
                catch (ArgumentNullException ex)
                {
                    _logger.LogError(ex,
                        "The movie show with ID {MovieShowId} does not exist.",
                        yetToAddMovieShowId);
                    throw new ServiceException(
                        "The entity with the specified ID does not exist.",
                        ex);
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogError(ex,
                        "The movie show with ID {MovieShowId} does not exist.",
                        yetToAddMovieShowId);
                    throw new ServiceException(
                        "The entity with the specified ID does not exist.",
                        ex);
                }
            }

            await _repository.Update(screen);
            return theaterAddMovieShowDto;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while fetching screen with ID {ScreenId}.",
                id);
            throw new ServiceException(
                "An error occurred in the service while adding the entity.",
                ex);
        }
    }

    public async Task Delete(int id)
    {
        _logger.LogInformation(
            "Attempting to delete the screen with ID {Id}.", id);

        try
        {
            await _repository.Delete(id);
            _logger.LogInformation(
                "Successfully deleted the screen with ID {Id}.", id);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while deleting the screen with ID {Id}.",
                id);
            throw new ServiceException(
                "An error occurred in the service while deleting the entity.",
                ex);
        }
    }

    public async Task<ICollection<ScreenListDto>> GetAll()
    {
        _logger.LogInformation("Fetching all screens.");

        try
        {
            var screens = await _repository.GetDbSet()
                .Select(entity => new ScreenListDto().CopyFrom(entity))
                .ToListAsync();

            _logger.LogInformation(
                "Successfully fetched {Count} screens.", screens.Count);
            return screens;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while fetching screens.");
            throw new ServiceException(
                "An error occurred in the service while fetching entities.",
                ex);
        }
    }

    public async Task<ScreenGetDto> GetById(int id)
    {
        _logger.LogInformation("Fetching the screen with ID {Id}.", id);

        try
        {
            var screen = await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
                .Include(entity => entity.Theater)
                .FirstAsync();

            _logger.LogInformation(
                "Successfully fetched the screen with ID {Id}.", id);
            return new ScreenGetDto().CopyFrom(screen);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while fetching the screen with ID {Id}.",
                id);
            throw new ServiceException(
                $"An error occurred in the service while fetching the entity with ID {id}.",
                ex);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex,
                "The screen with ID {Id} does not exist.", id);
            throw new ServiceException(
                "The entity with the specified ID does not exist.");
        }
    }

    public async Task<ScreenAddRemoveMovieShowsDto> RemoveMovieShows(
        int id, ScreenAddRemoveMovieShowsDto theaterRemoveMovieShowDto)
    {
        var screen = await _repository.GetDbSet()
            .Include(entity => entity.MovieShows)
            .Where(entity => entity.Id == id)
            .FirstAsync();

        foreach (int movieShowId in theaterRemoveMovieShowDto.MovieShowIds)
        {
            var movieShow = screen.MovieShows
                .FirstOrDefault(entity => entity.Id == movieShowId);

            if (movieShow != null)
            {
                screen.MovieShows.Remove(movieShow);
            }
        }
        return theaterRemoveMovieShowDto;
    }

    public async Task<Screen> Update(int id, ScreenAddUpdateDto entity)
    {
        _logger.LogInformation(
            "Attempting to update the screen with ID {Id}.", id);

        try
        {
            var existingEntity = await _repository.GetById(id);
            var updatedEntity = entity.CopyTo(existingEntity);
            var result = await _repository.Update(updatedEntity);

            _logger.LogInformation(
                "Successfully updated the screen with ID {Id}.", result.Id);
            return result;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while updating the screen with ID {Id}.",
                id);
            throw new ServiceException(
                "An error occurred in the service while updating the entity.",
                ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex,
                "Validation error while updating the screen with ID {Id}.", id);
            throw new ServiceValidationException(ex.Message, ex);
        }
    }
}
