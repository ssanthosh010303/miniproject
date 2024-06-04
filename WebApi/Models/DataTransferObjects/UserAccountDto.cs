/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.DataTransferObjects;

public class UserAccountListDto : BaseListDto
{
    public string FullName { get; set; }

    public UserAccountListDto CopyFrom(UserAccount entity)
    {
        Id = entity.Id;
        FullName = entity.FullName;

        return this; // XXX
    }
}

public class UserAccountAddDto : BaseAddUpdateDto
{
    [Required(ErrorMessage = "Name is a required field.")]
    [MaxLength(64, ErrorMessage = "Name cannot be more than 64 characters.")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Username is a required field.")]
    [MaxLength(64, ErrorMessage = "Username cannot be more than 64 characters.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Email is a required field.")]
    [EmailAddress(ErrorMessage = "Invalid email address provided.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password hash cannot be empty.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Phone number is a required field.")]
    [Phone(ErrorMessage = "Invalid phone number format provided.")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "City of residence is a required field.")]
    [MaxLength(64, ErrorMessage = "City of residence cannot be more than 64 characters.")]
    public string City { get; set; }

    public UserAccount CopyTo(UserAccount entity)
    {
        entity.FullName = FullName;
        entity.Username = Username;
        entity.Email = Email;
        entity.PasswordHash = Password; // Hashed in Service Layer
        entity.Phone = Phone;
        entity.Role = "User";
        entity.City = City;

        return entity;
    }
}

public class UserAccountUpdateDto : BaseAddUpdateDto
{
    [Required(ErrorMessage = "Name is a required field.")]
    [MaxLength(64, ErrorMessage = "Name cannot be more than 64 characters.")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Username is a required field.")]
    [MaxLength(64, ErrorMessage = "Username cannot be more than 64 characters.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Phone number is a required field.")]
    [Phone(ErrorMessage = "Invalid phone number format provided.")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "City of residence is a required field.")]
    [MaxLength(64, ErrorMessage = "City of residence cannot be more than 64 characters.")]
    public string City { get; set; }

    public UserAccount CopyTo(UserAccount entity)
    {
        entity.FullName = FullName;
        entity.Username = Username;
        entity.Phone = Phone;
        entity.City = City;

        return entity;
    }
}

public class UserAccountUpdateEmailAndForgotPasswordDto : BaseDto
{
    [Required(ErrorMessage = "Email is a required field.")]
    [EmailAddress(ErrorMessage = "Invalid email address provided.")]
    public string Email { get; set; }
}

public class UserAccountResetPasswordDto : BaseDto
{
    [Required(ErrorMessage = "Password hash cannot be empty.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    public string Password { get; set; }
}

public class UserAccountSetPasswordDto : BaseDto
{
    [Required(ErrorMessage = "Old password cannot be empty.")]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "Password hash cannot be empty.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Password confirmation cannot be empty.")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }
}

public class UserAccountGetDto : BaseGetDto
{
    public string FullName { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public Membership MemberType { get; set; } = Membership.Basic;

    public string Phone { get; set; }

    public string Role { get; set; }

    public UserAccountGetDto CopyFrom(UserAccount entity)
    {
        Id = entity.Id;
        FullName = entity.FullName;
        Username = entity.Username;
        Email = entity.Email;
        MemberType = entity.MemberType;
        Phone = entity.Phone;
        Role = entity.Role;

        return this; // XXX
    }
}
