/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using WebApi.Exceptions;
using WebApi.Email;
using WebApi.Models;
using WebApi.Models.DataTransferObjects;
using WebApi.Repositories;
using System.Security.Claims;

namespace WebApi.Services;

public interface IUserAccountService
{
    Task<UserAccountGetDto> Add(UserAccountAddDto entity);
    Task Delete(int id);
    Task<ICollection<UserAccountListDto>> GetAll();
    Task<UserAccountGetDto> GetByUsername(string id);
    Task<UserAccountGetDto> Update(int id, UserAccountUpdateDto entity);
    Task<UserAccountUpdateEmailAndForgotPasswordDto> UpdateEmail(
        string username, UserAccountUpdateEmailAndForgotPasswordDto entity);
    Task FinishEmailUpdate(string jwt);
    Task<UserAccount> ValidateUser(string username, string password);
    Task<UserAccount> ActivateAccount(string jwt);
    Task ForgotPassword(UserAccountUpdateEmailAndForgotPasswordDto entity);
    Task ResetPassword(string jwt, UserAccountResetPasswordDto entity);
    Task SetPassword(UserAccountSetPasswordDto entity, string username);
}

public class UserAccountService(
        IBaseRepository<UserAccount> repository,
        ICryptographyService cryptographyService,
        IEmailService emailService,
        IJwtService jwtService,
        ILogger<UserAccountService> logger)
    : IUserAccountService
{
    private readonly IBaseRepository<UserAccount> _repository = repository;
    private readonly ICryptographyService _cryptographyService = cryptographyService;
    private readonly IEmailService _emailService = emailService;
    private readonly IJwtService _jwtService = jwtService;
    private readonly ILogger<UserAccountService> _logger = logger;

    public async Task<UserAccountGetDto> Add(UserAccountAddDto entity)
    {
        _logger.LogInformation("Starting to add a new user.");
        try
        {
            var newUser = entity.CopyTo(new UserAccount());

            newUser.PasswordHash = _cryptographyService.HashPassword(
                newUser.PasswordHash);
            newUser.IsActive = false; // Override
            _logger.LogInformation("Password hashed for the new user.");

            var token = _jwtService.GenerateToken(
                newUser.Username, "User", accessTokenExpirationHours: 1);
            _logger.LogInformation("JWT token generated for the new user.");

            EmailTemplate welcomeTemplate = EmailTemplateFactory.CreateTemplate(
                EmailTemplateType.Welcome,
                newUser.FullName,
                token);

            await _emailService.SendEmail(newUser.Email, welcomeTemplate);

            _logger.LogInformation(
                "Welcome email sent successfully to the new user with email {Email}.",
                newUser.Email);

            var addedUser = await _repository.Add(newUser);
            _logger.LogInformation(
                "Successfully added a new user with ID {UserId}.",
                addedUser.Id);

            return new UserAccountGetDto().CopyFrom(addedUser);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex, "The username or email ID already exists.");
            throw new ServiceException("The username or email ID already exists.", ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex,
                "Validation error occurred while adding the user: {Message}",
                ex.Message);
            throw new ServiceValidationException(ex.Message, ex);
        }
    }

    public async Task Delete(int id)
    {
        _logger.LogInformation("Starting to delete user with ID {UserId}.", id);
        try
        {
            await _repository.Delete(id);
            _logger.LogInformation(
                "Successfully deleted user with ID {UserId}.", id);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the service while deleting the user.");
            throw new ServiceException(
                "An error occurred in the service while deleting the user.", ex);
        }
    }

    public async Task<ICollection<UserAccountListDto>> GetAll()
    {
        _logger.LogInformation("Starting to fetch all users.");
        try
        {
            var users = await _repository.GetDbSet()
                .Select(entity => new UserAccountListDto().CopyFrom(entity))
                .ToListAsync();

            _logger.LogInformation(
                "Successfully fetched all users. Count: {Count}", users.Count);
            return users;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the service while fetching all users.");
            throw new ServiceException(
                "An error occurred in the service while fetching all users.", ex);
        }
    }

    public async Task<UserAccountGetDto> GetByUsername(string username)
    {
        _logger.LogInformation("Starting to fetch user with username {Username}.",
            username);
        try
        {
            var user = await _repository.GetDbSet()
                .Where(entity => entity.Username == username)
                .FirstAsync();

            _logger.LogInformation(
                "Successfully fetched user with username {Username}.", username);
            return new UserAccountGetDto().CopyFrom(user);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the service while fetching the user with username {Username}.",
                username);
            throw new ServiceException(
                $"An error occurred in the service while fetching the user with username {username}.",
                ex);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex,
                "The user with the specified ID {Username} does not exist.", username);
            throw new ServiceException(
                "The user with the specified username does not exist.");
        }
    }

    public async Task<UserAccountGetDto> Update(int id, UserAccountUpdateDto entity)
    {
        _logger.LogInformation("Starting to update user with ID {UserId}.", id);
        try
        {
            var existingEntity = await _repository.GetById(id);
            var updatedEntity = await _repository.Update(
                entity.CopyTo(existingEntity));

            _logger.LogInformation(
                "Successfully updated user with ID {UserId}.", id);

            return new UserAccountGetDto().CopyFrom(updatedEntity);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the service while updating the user.");
            throw new ServiceException(
                "An error occurred in the service while updating the user.", ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex,
                "Validation error occurred while updating the user: {Message}",
                ex.Message);
            throw new ServiceValidationException(ex.Message, ex);
        }
    }

    public async Task<UserAccount> ValidateUser(string username, string password)
    {
        _logger.LogInformation(
            "Starting to validate user with username {Username}.", username);
        try
        {
            var user = await _repository.GetDbSet()
                .Where(entity => entity.Username == username)
                .FirstAsync();

            if (!_cryptographyService.VerifyPassword(password, user.PasswordHash))
            {
                _logger.LogWarning(
                    "Invalid password for user with username {Username}.",
                    username);
                throw new ServiceException("Invalid password.");
            }

            _logger.LogInformation(
                "Successfully validated user with username {Username}.",
                username);
            return user;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the service while validating the user.");
            throw new ServiceException(
                "An error occurred in the service while validating the user.", ex);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex,
                "The user with the specified username {Username} does not exist.",
                username);
            throw new ServiceException("The user does not exist.");
        }
    }

    public async Task<UserAccount> ActivateAccount(string jwt)
    {
        _logger.LogInformation("Starting to validate JWT token.");
        try
        {
            var username = _jwtService.ValidateTokenAndGetClaims(jwt).
                Find(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (username == null)
            {
                _logger.LogWarning("Invalid JWT token.");
                throw new ServiceException("Invalid JWT token.");
            }

            _logger.LogInformation(
                "JWT token validated successfully for username {Username}.",
                username);

            var user = await _repository.GetDbSet()
                .Where(entity => entity.Username == username)
                .FirstAsync();

            user.IsActive = true; // XXX
            _logger.LogInformation(
                "User account activated for username {Username}.", username);

            var updatedUser = await _repository.Update(user);
            _logger.LogInformation(
                "Successfully updated user account for username {Username}.",
                username);

            return updatedUser;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the service while verifying the user.");
            throw new ServiceException(
                "An error occurred in the service while verifying the user.", ex);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex,
                "The user with the specified username {Username} does not exist.",
                ex);
            throw new ServiceException("The user does not exist.");
        }
    }

    public async Task ForgotPassword(UserAccountUpdateEmailAndForgotPasswordDto entity)
    {
        _logger.LogInformation(
            "Starting to process forgot password request for user with email {Email}.",
            entity.Email);
        try
        {
            var user = await _repository.GetDbSet()
                .Where(existingEntity => existingEntity.Email == entity.Email)
                .FirstAsync();

            var token = _jwtService.GenerateToken(user.Username, "User", accessTokenExpirationHours: 1);
            _logger.LogInformation("JWT token generated for the user.");

            EmailTemplate forgotPasswordTemplate = EmailTemplateFactory
                .CreateTemplate(EmailTemplateType.ForgotPassword,
                    user.FullName, token);

            await _emailService.SendEmail(user.Email, forgotPasswordTemplate);

            _logger.LogInformation(
                "Forgot password email sent successfully to the user with email {Email}.",
                user.Email);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the service while processing the forgot password request.");
            throw new ServiceException(
                "An error occurred in the service while processing the forgot password request.", ex);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex,
                "The user with the specified email {Email} does not exist.",
                entity.Email);
            throw new ServiceException("The user does not exist.");
        }
    }

    public async Task ResetPassword(string jwt, UserAccountResetPasswordDto entity)
    {
        var username = _jwtService.ValidateTokenAndGetClaims(jwt).Find(
            claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

        if (username == null)
        {
            _logger.LogWarning("Invalid JWT token.");
            throw new ServiceException("Invalid JWT token.");
        }

        _logger.LogInformation("Starting to reset password for user.");

        try
        {
            var user = await _repository.GetDbSet()
                .Where(entity => entity.Username == username)
                .FirstAsync();

            user.PasswordHash = _cryptographyService.HashPassword(
                entity.Password);

            await _repository.Update(user);

            EmailTemplate passwordChangedTemplate = EmailTemplateFactory
                .CreateTemplate(EmailTemplateType.PasswordResetConfirmation,
                    user.FullName);

            await _emailService.SendEmail(user.Email, passwordChangedTemplate);

            _logger.LogInformation("Successfully updated password for user.");
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the service while resetting the password.");
            throw new ServiceException(
                "An error occurred in the service while resetting the password.", ex);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex,
                "The user with the specified username {Username} does not exist.",
                username);
            throw new ServiceException("The user does not exist.");
        }
    }

    public async Task SetPassword(UserAccountSetPasswordDto entity, string username)
    {
        _logger.LogInformation(
            "Starting to set password for user with username {Username}.",
            username);
        try
        {
            var user = await _repository.GetDbSet()
                .Where(entity => entity.Username == username)
                .FirstAsync();
            var hashedPassword = _cryptographyService.HashPassword(
                entity.Password);

            if (!_cryptographyService.VerifyPassword(
                entity.OldPassword, user.PasswordHash))
            {
                _logger.LogWarning(
                    "Invalid old password for user with username {Username}.",
                    username);
                throw new ServiceException(
                    "Invalid old password. You can instead reset your password.");
            }

            user.PasswordHash = _cryptographyService.HashPassword(
                entity.Password);

            _logger.LogInformation(
                "Password hashed successfully for user with username {Username}.",
                username);

            await _repository.Update(user);

            _logger.LogInformation(
                "Sending email to user with username {Username}.", username);

            EmailTemplate passwordChangedTemplate = EmailTemplateFactory
                .CreateTemplate(EmailTemplateType.PasswordResetConfirmation,
                    user.FullName);

            await _emailService.SendEmail(user.Email, passwordChangedTemplate);

            _logger.LogInformation(
                "Successfully updated password for user with username {Username}.",
                username);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the service while setting the password.");
            throw new ServiceException(
                "An error occurred in the service while setting the password.", ex);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex,
                "The user with the specified username {Username} does not exist.",
                username);
            throw new ServiceException("The user does not exist.");
        }
    }

    public async Task<UserAccountUpdateEmailAndForgotPasswordDto> UpdateEmail(
        string username, UserAccountUpdateEmailAndForgotPasswordDto dtoEntity)
    {
        var user = await _repository.GetDbSet()
            .Where(entity => entity.Username == username)
            .FirstAsync();

        if (_repository.GetDbSet()
            .Where(entity =>
                entity.Username != username && entity.Email == dtoEntity.Email)
            .Any())
        {
            throw new ServiceException("The email ID already registered with someone else.");
        }
        if (user.Email == dtoEntity.Email)
        {
            throw new ServiceException("The email ID is already associated with the user.");
        }

        user.IsActive = false;
        await _repository.Update(user);

        var token = _jwtService.GenerateToken(
            user.Username, "User", accessTokenExpirationHours: 1,
            additionalClaims: new Claim(ClaimTypes.Email, dtoEntity.Email));

        EmailTemplate updateEmailTemplate = EmailTemplateFactory
            .CreateTemplate(EmailTemplateType.EmailIdUpdateConfirmation,
                user.FullName, token);

        await _emailService.SendEmail(user.Email, updateEmailTemplate);
        return dtoEntity;
    }

    public Task FinishEmailUpdate(string jwt)
    {
        var username = _jwtService.ValidateTokenAndGetClaims(jwt).Find(
            claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

        var email = _jwtService.ValidateTokenAndGetClaims(jwt).Find(
            claim => claim.Type == ClaimTypes.Email)?.Value;

        if (username == null || email == null)
        {
            _logger.LogWarning("Invalid JWT token.");
            throw new ServiceException("Invalid JWT token.");
        }

        var user = _repository.GetDbSet()
            .Where(entity => entity.Username == username)
            .First();

        user.Email = email;
        user.IsActive = true;
        return _repository.Update(user);
    }
}
