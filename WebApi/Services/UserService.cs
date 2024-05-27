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
    public Task<User> ValidateToken(string jwt);
}

public class UserService(
    IBaseRepository<User> repository,
    ICryptographyService cryptographyService,
    IEmailService emailService,
    IJwtService jwtService)
    : IUserService
{
    private readonly IBaseRepository<User> _repository = repository;
    private readonly ICryptographyService _cryptographyService = cryptographyService;
    private readonly IEmailService _emailService = emailService;
    private readonly IJwtService _jwtService = jwtService;

    public async Task<UserGetDto> Add(UserAddDto entity)
    {
        try
        {
            var newUser = entity.CopyTo(new User());

            newUser.PasswordHash = _cryptographyService.HashPassword(
                newUser.PasswordHash);
            await _emailService.SendEmailAsync(
                newUser.Email,
                "Welcome to Movie Booking System!",
                "You have successfully registered. Kindly verify your email by"
                +" clicking on the link provided in the email.\n\n"
                + $"Email Activation Link: http://4.240.98.131:5064/api/user/verify/{_jwtService.GenerateToken(newUser.Username, "Admin")}\n\n"
                + "With Regards,\nThe Movie Booking Team\n"
            );
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

    public async Task<User> ValidateToken(string jwt)
    {
        try
        {
            var username = _jwtService.ValidateToken(jwt);

            Console.WriteLine("JWT validated successfully!");
            var user = await _repository.GetDbSet()
                .Where(entity => entity.Username == username)
                .FirstAsync();

            user.IsActive = true;

            return await _repository.Update(user);
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while verifying the user.",
                ex);
        }
        catch (InvalidOperationException)
        {
            throw new ServiceException("The user does not exist.");
        }
    }
}
