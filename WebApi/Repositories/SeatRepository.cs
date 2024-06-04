/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
using Microsoft.EntityFrameworkCore;
using WebApi.Exceptions;
using WebApi.Models;

namespace WebApi.Repositories;

public interface ISeatRepository
{
    public DbSet<Seat> GetDbSet();
    public Task<Seat> GetById(int screenId, string seatId);
    public Task<Seat> Add(Seat seat);
    public Task<Seat> Update(Seat seat);
    public Task Delete(int screenId, string seatId);
    public Task SaveChanges();
}

public class SeatRepository(
    ApplicationDbContext applicationDbContext, ILogger<SeatRepository> logger)
    : ISeatRepository
{
    private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
    private readonly ILogger<SeatRepository> _logger = logger;

    public DbSet<Seat> GetDbSet()
    {
        return _applicationDbContext.Set<Seat>();
    }

    public async Task<Seat> GetById(int screenId, string seatId)
    {
        _logger.LogInformation(
            "Getting seat with ID {SeatId} from screen with ID {ScreenId}.",
            seatId, screenId);

        return await GetDbSet()
            .Where(entity => entity.ScreenId == screenId && entity.Id == seatId)
            .FirstAsync();
    }

    public async Task<Seat> Add(Seat seat)
    {
        try
        {
            _logger.LogInformation(
                "Adding seat with ID {SeatId} to screen with ID {ScreenId}.",
                seat.Id, seat.ScreenId);

            _applicationDbContext.Seats.Add(seat);
            await _applicationDbContext.SaveChangesAsync();
            return seat;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(
                $"Seat with ID {seat.Id} already exists or is found already.",
                ex);

            throw new RepositoryException(
                $"Seat with ID {seat.Id} already exists or is found already.",
                ex);
        }
    }

    public async Task<Seat> Update(Seat seat)
    {
        try
        {
            _logger.LogInformation(
                "Updating seat with ID {SeatId} in screen with ID {ScreenId}",
                seat.Id, seat.ScreenId);

            _applicationDbContext.Entry(seat).State = EntityState.Modified;
            await _applicationDbContext.SaveChangesAsync();
            return seat;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while updating because a concurrency " +
                "conflict was detected.");

            throw new RepositoryException(
                "Error occurred while updating because a concurrency " +
                "conflict was detected.", ex);
        }
        catch (DbUpdateException)
        {
            _logger.LogError(
                $"Seat with ID {seat.Id} does not exist.");
            throw new RepositoryException(
                $"Seat with ID {seat.Id} does not exist.");
        }
    }

    public async Task Delete(int screenId, string seatId)
    {
        try
        {
            _logger.LogInformation(
                "Deleting seat with ID {SeatId} from screen with ID {ScreenId}.",
                seatId, screenId);

            _applicationDbContext.Seats.Remove(await GetById(
                screenId, seatId));
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while deleting entity.");
            throw new RepositoryException(
                "Error occurred while deleting entity.", ex);
        }
    }

    public Task SaveChanges()
    {
        try
        {
            _logger.LogInformation("Saving changes to the database.");
            return _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex,
                "Error occurred while saving changes to the database because a concurrency conflict was detected.");
            throw new RepositoryException(
                "Error occurred while saving changes to the database because a concurrency conflict was detected.",
                ex);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex,
                "Error occurred while saving changes to the database.");
            throw new RepositoryException(
                "Error occurred while saving changes to the database.", ex);
        }
    }
}
