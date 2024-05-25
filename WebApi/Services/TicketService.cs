using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using WebApi.Exceptions;
using WebApi.Models;
using WebApi.Models.DataTransferObjects;
using WebApi.Repositories;

namespace WebApi.Services;

public interface ITicketService
{
    public Task<Ticket> Add(TicketAddUpdateDto entity);
    public Task Delete(int id);
    public Task<ICollection<TicketListDto>> GetAll();
    public Task<TicketGetDto> GetById(int id);
}

public class TicketService(IBaseRepository<Ticket> repository)
    : ITicketService
{
    private readonly IBaseRepository<Ticket> _repository = repository;

    public async Task<Ticket> Add(TicketAddUpdateDto entity)
    {
        try
        {
            return await _repository.Add(entity.CopyTo(new Ticket()));
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

    public async Task<ICollection<TicketListDto>> GetAll()
    {
        try
        {
            return await _repository.GetDbSet()
                .Select(entity => new TicketListDto().CopyFrom(entity))
                .ToListAsync();
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while fetching entities.",
                ex);
        }
    }

    public async Task<TicketGetDto> GetById(int id)
    {
        try
        {
            return await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
                .Include(entity => entity.User)
                .Include(entity => entity.MovieShow)
                .ThenInclude(entity => entity.Movie.Name)
                .Select(entity => new TicketGetDto().CopyFrom(entity))
                .FirstAsync();
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while fetching the entity " +
                $"with ID {id}.", ex);
        }
    }
}
