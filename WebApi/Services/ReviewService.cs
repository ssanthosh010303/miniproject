using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using WebApi.Exceptions;
using WebApi.Models;
using WebApi.Models.DataTransferObjects;
using WebApi.Repositories;

namespace WebApi.Services;

public interface IReviewService
{
    public Task<Review> Add(ReviewAddUpdateDto entity);
    public Task Delete(int id);
    public Task<ICollection<ReviewListDto>> GetAll();
    public Task<ReviewGetDto> GetById(int id);
    public Task<Review> Update(int id, ReviewAddUpdateDto entity);
}

public class ReviewService(IBaseRepository<Review> repository)
    : IReviewService
{
    private readonly IBaseRepository<Review> _repository = repository;

    public async Task<Review> Add(ReviewAddUpdateDto entity)
    {
        try
        {
            // var movie = await _movieRepository.GetById(entity.MovieId);
            var review = entity.CopyTo(new Review());
            review.MovieId = entity.MovieId;

            return await _repository.Add(review);
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

    public async Task<ICollection<ReviewListDto>> GetAll()
    {
        try
        {
            return await _repository.GetDbSet()
                .Select(entity => new ReviewListDto().CopyFrom(entity))
                .ToListAsync();
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while fetching entities.",
                ex);
        }
    }

    public async Task<ReviewGetDto> GetById(int id)
    {
        try
        {
            return new ReviewGetDto().CopyFrom(await _repository.GetById(id));
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

    public async Task<Review> Update(int id, ReviewAddUpdateDto entity)
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
