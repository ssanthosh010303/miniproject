using Microsoft.EntityFrameworkCore;
using WebApi.Exceptions;
using WebApi.Models;

namespace WebApi.Repositories;

public interface ISeatRepository
{
    public DbSet<Seat> GetDbSet();
    public Task<Seat> GetById(int theaterId, int screenId, string seatId);
    public Task<Seat> Add(Seat seat);
    public Task<Seat> Update(Seat seat);
    public Task Delete(int theaterId, int screenId, string seatId);
}

public class SeatRepository(ApplicationDbContext applicationDbContext)
    : ISeatRepository
{
    private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

    public DbSet<Seat> GetDbSet()
    {
        return _applicationDbContext.Set<Seat>();
    }

    public async Task<Seat> GetById(int theaterId, int screenId, string seatId)
    {
        return await GetDbSet()
            .Where(entity => entity.TheaterId == theaterId &&
                entity.ScreenId == screenId &&
                entity.Id == seatId)
            .FirstAsync();
    }

    public async Task<Seat> Add(Seat seat)
    {
        try
        {
            _applicationDbContext.Seats.Add(seat);
            await _applicationDbContext.SaveChangesAsync();
            return seat;
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException(
                $"Seat with ID {seat.Id} already exists or is found already.",
                ex);
        }
    }

    public async Task<Seat> Update(Seat seat)
    {
        try
        {
            _applicationDbContext.Entry(seat).State = EntityState.Modified;
            await _applicationDbContext.SaveChangesAsync();
            return seat;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new RepositoryException(
                "Error occurred while updating because a concurrency " +
                "conflict was detected.", ex);
        }
        catch (DbUpdateException)
        {
            throw new RepositoryException(
                $"Seat with ID {seat.Id} does not exist.");
        }
    }

    public async Task Delete(int theaterId, int screenId, string seatId)
    {
        try
        {
            _applicationDbContext.Seats.Remove(await GetById(
                theaterId, screenId, seatId));
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException(
                "Error occurred while deleting entity.", ex);
        }
    }
}
