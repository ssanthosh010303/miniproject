using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using WebApi.Exceptions;
using WebApi.Models;
using WebApi.Models.DataTransferObjects;
using WebApi.Repositories;

namespace WebApi.Services;

public interface IUserService
{
    public Task<UserGetDto> Add(UserAddDto entity);
    public Task Delete(int id);
    public Task<ICollection<UserListDto>> GetAll();
    public Task<UserGetDto> GetById(int id);
    public Task<User> Update(int id, UserUpdateDto entity);

    public Task<User> ValidateUser(string username, string password);
}

public class UserService(
    IBaseRepository<User> repository, ICryptographyService cryptographyService)
    : IUserService
{
    private readonly IBaseRepository<User> _repository = repository;
    private readonly ICryptographyService _cryptographyService = cryptographyService;

    public async Task<UserGetDto> Add(UserAddDto entity)
    {
        try
        {
            var newUser = entity.CopyTo(new User());

            newUser.PasswordHash = _cryptographyService.HashPassword(
                newUser.PasswordHash);
            return new UserGetDto().CopyFrom(await _repository.Add(newUser));
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "The username or email ID already exists.",
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

    public async Task<ICollection<UserListDto>> GetAll()
    {
        try
        {
            return await _repository.GetDbSet()
                .Select(entity => new UserListDto().CopyFrom(entity))
                .ToListAsync();
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while fetching entities.",
                ex);
        }
    }

    public async Task<UserGetDto> GetById(int id)
    {
        try
        {
            return new UserGetDto().CopyFrom(await _repository.GetDbSet()
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

    public async Task<User> Update(int id, UserUpdateDto entity)
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

    public async Task<User> ValidateUser(string username, string password)
    {
        try
        {
            var user = await _repository.GetDbSet()
                .Where(entity => entity.Username == username)
                .FirstAsync();

            if (!_cryptographyService.VerifyPassword(password, user.PasswordHash))
            {
                throw new ServiceException("Invalid password.");
            }

            return user;
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while validating the user.",
                ex);
        }
        catch (InvalidOperationException)
        {
            throw new ServiceException("The user does not exist.");
        }
    }
}
