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

public interface IReviewService
{
    public Task<Review> Add(ReviewAddUpdateDto entity);
    public Task Delete(int id);
    public Task<ICollection<ReviewListDto>> GetAll(float rating);
    public Task<ReviewGetDto> GetById(int id);
    public Task<Review> Update(int id, ReviewAddUpdateDto entity);
}

public class ReviewService(
        IBaseRepository<Review> repository, ILogger<ReviewService> logger)
    : IReviewService
{
    private readonly IBaseRepository<Review> _repository = repository;
    private readonly ILogger<ReviewService> _logger = logger;

    public async Task<Review> Add(ReviewAddUpdateDto entity)
    {
        _logger.LogInformation("Attempting to add a new review.");

        try
        {
            var review = entity.CopyTo(new Review());
            review.MovieId = entity.MovieId;

            var result = await _repository.Add(review);

            _logger.LogInformation(
                "Successfully added a new review with ID {Id}.", result.Id);
            return result;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while adding the review.");
            throw new ServiceException(
                "An error occurred in the service while adding the entity.",
                ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation error while adding the review.");
            throw new ServiceValidationException(ex.Message, ex);
        }
    }

    public async Task Delete(int id)
    {
        _logger.LogInformation(
            "Attempting to delete the review with ID {Id}.", id);

        try
        {
            await _repository.Delete(id);
            _logger.LogInformation(
                "Successfully deleted the review with ID {Id}.", id);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while deleting the review with ID {Id}.",
                id);
            throw new ServiceException(
                "An error occurred in the service while deleting the entity.",
                ex);
        }
    }

    public async Task<ICollection<ReviewListDto>> GetAll(float rating)
    {
        _logger.LogInformation(
            "Fetching all reviews with rating {Rating}.", rating);

        try
        {
            var queryableObj = _repository.GetDbSet()
                .Select(entity => new ReviewListDto().CopyFrom(entity));

            if (rating != 0)
            {
                queryableObj = queryableObj
                    .Where(entity => entity.Rating == rating);
            }

            var reviews = await queryableObj.ToListAsync();
            _logger.LogInformation(
                "Successfully fetched {Count} reviews.", reviews.Count);
            return reviews;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(
                ex,
                "An error occurred in the repository while fetching reviews.");
            throw new ServiceException(
                "An error occurred in the service while fetching entities.", ex);
        }
    }

    public async Task<ReviewGetDto> GetById(int id)
    {
        _logger.LogInformation("Fetching the review with ID {Id}.", id);

        try
        {
            var review = await _repository.GetById(id);
            _logger.LogInformation(
                "Successfully fetched the review with ID {Id}.", id);
            return new ReviewGetDto().CopyFrom(review);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while fetching the review with ID {Id}.",
                id);
            throw new ServiceException(
                $"An error occurred in the service while fetching the entity with ID {id}.",
                ex);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "The review with ID {Id} does not exist.", id);
            throw new ServiceException(
                "The entity with the specified ID does not exist.");
        }
    }

    public async Task<Review> Update(int id, ReviewAddUpdateDto entity)
    {
        _logger.LogInformation(
            "Attempting to update the review with ID {Id}.", id);

        try
        {
            var existingEntity = await _repository.GetById(id);
            var updatedEntity = entity.CopyTo(existingEntity);
            var result = await _repository.Update(updatedEntity);

            _logger.LogInformation(
                "Successfully updated the review with ID {Id}.", result.Id);
            return result;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the repository while updating the review with ID {Id}.",
                id);
            throw new ServiceException(
                "An error occurred in the service while updating the entity.",
                ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex,
                "Validation error while updating the review with ID {Id}.", id);
            throw new ServiceValidationException(ex.Message, ex);
        }
    }
}
