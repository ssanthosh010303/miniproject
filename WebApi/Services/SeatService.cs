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
    public Task Delete(int theaterId, int screenId, string seatId);
    public Task<ICollection<SeatListDto>> GetAll(int theaterId, int screenId);
    public Task<SeatGetDto> GetById(int theaterId, int screenId, string seatId);
}

public class SeatService(ISeatRepository repository, IBaseRepository<Screen> screenRepository)
    : ISeatService
{
    private readonly ISeatRepository _repository = repository;
    private readonly IBaseRepository<Screen> _screenRepository = screenRepository;

    public async Task<SeatAddUpdateDto> Add(SeatAddUpdateDto entity)
    {
        try
        {
            var theaterId = entity.TheaterId;

            foreach (var seatSchema in entity.SeatSchemas)
            {
                char startChar = char.ToUpper(seatSchema.RowRange[0]);
                char endChar = char.ToUpper(seatSchema.RowRange[2]);
                var screenId = seatSchema.ScreenId;
                var seatTypeId = seatSchema.SeatTypeId;

                var screen = await _screenRepository.GetById(screenId);

                if (screen.TheaterId != theaterId)
                    throw new ServiceException(
                        "The screen ID provided does not belong to the theater ID provided.");

                for (char c = startChar; c <= endChar; c++)
                {
                    for (int i = 1; i <= seatSchema.NumberOfColumns; i++)
                    {
                        var seat = new Seat
                        {
                            Id = $"{c}{i}",
                            ScreenId = screenId,
                            TheaterId = theaterId,
                            SeatTypeId = seatTypeId,
                        };

                        await _repository.Add(seat);
                    }
                }
            }

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

    public async Task Delete(int theaterId, int screenId, string seatId)
    {
        try
        {
            await _repository.Delete(theaterId, screenId, seatId);
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while deleting the entity.",
                ex);
        }
    }

    public async Task<ICollection<SeatListDto>> GetAll(int theaterId, int screenId)
    {
        try
        {
            if (theaterId == 0 && screenId != 0)
                throw new ServiceException("Theater ID is required when screen ID is provided.");

            IQueryable<Seat> queryableObj = _repository.GetDbSet();

            if (theaterId != 0)
            {
                queryableObj = queryableObj
                    .Where(entity => entity.TheaterId == theaterId);
            }
            if (screenId != 0)
            {
                queryableObj = queryableObj
                    .Where(entity => entity.ScreenId == screenId);
            }

            return await queryableObj
                .Include(entity => entity.SeatType)
                .Select(entity => new SeatListDto().CopyFrom(entity))
                .ToListAsync();
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while fetching entities.",
                ex);
        }
    }

    public async Task<SeatGetDto> GetById(int theaterId, int screenId, string seatId)
    {
        try
        {
            return new SeatGetDto().CopyFrom(await _repository.GetDbSet()
                .Where(entity => entity.Id == seatId
                    && entity.ScreenId == screenId
                    && entity.TheaterId == theaterId)
                .Include(entity => entity.SeatType)
                .Include(entity => entity.Theater)
                .FirstAsync());
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while fetching the entity " +
                $"with ID {seatId}.", ex);
        }
        catch (InvalidOperationException)
        {
            throw new ServiceException(
                "The entity with the specified ID does not exist.");
        }
    }
}
