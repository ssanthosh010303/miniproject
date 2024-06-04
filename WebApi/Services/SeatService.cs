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

public interface ISeatService
{
    public Task<SeatAddUpdateDto> Add(SeatAddUpdateDto entity);
    public Task Delete(SeatAddUpdateDto entity);
    public Task<ICollection<SeatListDto>> GetAll(int screenId);
    public Task<SeatGetDto> GetById(int screenId, string seatId);
}

public class SeatService(
        ISeatRepository repository,
        IBaseRepository<Screen> screenRepository,
        ILogger<SeatService> logger)
    : ISeatService
{
    private readonly ISeatRepository _repository = repository;
    private readonly IBaseRepository<Screen> _screenRepository = screenRepository;
    private readonly ILogger<SeatService> _logger = logger;

    public async Task<SeatAddUpdateDto> Add(SeatAddUpdateDto entity)
    {
        _logger.LogInformation("Attempting to add new seats.");

        try
        {
            var dbSet = _repository.GetDbSet();

            foreach (var seatSchema in entity.SeatSchemas)
            {
                char startChar = char.ToUpper(seatSchema.RowRange[0]);
                char endChar = char.ToUpper(seatSchema.RowRange[2]);

                var seatTypeId = seatSchema.SeatTypeId;

                var screen = await _screenRepository.GetById(entity.ScreenId);

                for (char c = startChar; c <= endChar; c++)
                {
                    for (int i = 1; i <= seatSchema.NumberOfColumns; i++)
                    {
                        var seat = new Seat
                        {
                            Id = $"{c}{i}",
                            ScreenId = entity.ScreenId,
                            SeatTypeId = seatTypeId,
                        };

                        await dbSet.AddAsync(seat);
                    }
                }
                // Batch Save
                await _repository.SaveChanges();
            }

            _logger.LogInformation("Successfully added new seats.");
            return entity;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while adding seats.");
            throw new ServiceException(
                "An error occurred in the service while adding the entity.", ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation error while adding seats.");
            throw new ServiceValidationException(ex.Message, ex);
        }
    }

    public async Task Delete(SeatAddUpdateDto entity)
    {
        _logger.LogInformation("Attempting to delete seats.");

        try
        {
            var dbSet = _repository.GetDbSet();

            foreach (var seatSchema in entity.SeatSchemas)
            {
                char startChar = char.ToUpper(seatSchema.RowRange[0]);
                char endChar = char.ToUpper(seatSchema.RowRange[2]);

                var seatTypeId = seatSchema.SeatTypeId;

                var screen = await _screenRepository.GetById(entity.ScreenId);

                for (char c = startChar; c <= endChar; c++)
                {
                    for (int i = 1; i <= seatSchema.NumberOfColumns; i++)
                    {
                        var seat = new Seat
                        {
                            Id = $"{c}{i}",
                            ScreenId = entity.ScreenId,
                            SeatTypeId = seatTypeId,
                        };

                        dbSet.Remove(seat);
                    }
                }
                // Batch Save
                await _repository.SaveChanges();
            }

            _logger.LogInformation("Successfully deleted all seats.");
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while adding seats.");
            throw new ServiceException(
                "An error occurred in the service while adding the entity.", ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation error while adding seats.");
            throw new ServiceValidationException(ex.Message, ex);
        }
    }

    public async Task<ICollection<SeatListDto>> GetAll(int screenId)
    {
        _logger.LogInformation(
            "Fetching all seats for screen with ID {ScreenId}.",
            screenId);

        try
        {
            IQueryable<Seat> queryableObj = _repository.GetDbSet();

            if (screenId != 0)
            {
                queryableObj = queryableObj.Where(
                    entity => entity.ScreenId == screenId);
            }

            var seats = await queryableObj
                .Include(entity => entity.SeatType)
                .Include(entity => entity.BookedForShow)
                .Select(entity => new SeatListDto().CopyFrom(entity))
                .ToListAsync();

            _logger.LogInformation(
                "Successfully fetched {Count} seats.", seats.Count);
            return seats;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while fetching seats.");
            throw new ServiceException(
                "An error occurred in the service while fetching entities.", ex);
        }
    }

    public async Task<SeatGetDto> GetById(int screenId, string seatId)
    {
        _logger.LogInformation(
            "Fetching the seat with ID {SeatId} for screen ID {ScreenId}.",
            seatId, screenId);

        try
        {
            var seat = await _repository.GetDbSet()
                .Where(entity => entity.Id == seatId
                    && entity.ScreenId == screenId)
                .Include(entity => entity.SeatType)
                .Include(entity => entity.Screen)
                .ThenInclude(entity => entity.Theater)
                .FirstAsync();

            _logger.LogInformation(
                "Successfully fetched the seat with ID {SeatId}.", seatId);
            return new SeatGetDto().CopyFrom(seat);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while fetching the seat with ID {SeatId}.",
                seatId);
            throw new ServiceException(
                $"An error occurred in the service while fetching the entity with ID {seatId}.",
                ex);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex,
                "The seat with ID {SeatId} does not exist.", seatId);
            throw new ServiceException(
                "The entity with the specified ID does not exist.");
        }
    }
}
