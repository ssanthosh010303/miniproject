using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using WebApi.Exceptions;
using WebApi.Models;
using WebApi.Models.DataTransferObjects;
using WebApi.Repositories;

namespace WebApi.Services;

public interface IPromoService
{
    public Task<Promo> Add(PromoAddUpdateDto entity);
    public Task Delete(int id);
    public Task<ICollection<PromoListDto>> GetAll();
    public Task<PromoGetDto> GetById(int id);
    public Task<Promo> Update(int id, PromoAddUpdateDto entity);
}

public class PromoService(IBaseRepository<Promo> repository)
    : IPromoService
{
    private readonly IBaseRepository<Promo> _repository = repository;

    public async Task<Promo> Add(PromoAddUpdateDto entity)
    {
        try
        {
            return await _repository.Add(entity.CopyTo(new Promo()));
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

    public async Task<ICollection<PromoListDto>> GetAll()
    {
        try
        {
            return await _repository.GetDbSet()
                .Select(entity => new PromoListDto().CopyFrom(entity))
                .ToListAsync();
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while fetching entities.",
                ex);
        }
    }

    public async Task<PromoGetDto> GetById(int id)
    {
        try
        {
            return new PromoGetDto().CopyFrom(await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
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

    public async Task<Promo> Update(int id, PromoAddUpdateDto entity)
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
