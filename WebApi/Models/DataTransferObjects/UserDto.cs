#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.DataTransferObjects;

public class UserListDto : BaseListDto
{
    public string FullName { get; set; }

    public UserListDto CopyFrom(User entity)
    {
        Id = entity.Id;
        FullName = entity.FullName;

        return this; // XXX
    }
}

public class UserAddDto : BaseAddUpdateDto
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
    public string Password { get; set; }

    [Required(ErrorMessage = "Phone number is a required field.")]
    [Phone(ErrorMessage = "Invalid phone number format provided.")]
    public string Phone { get; set; }

    public User CopyTo(User entity)
    {
        entity.FullName = FullName;
        entity.Username = Username;
        entity.Email = Email;
        entity.PasswordHash = Password; // Hashed in Service Layer
        entity.Phone = Phone;
        entity.Role = "User";

        return entity;
    }
}

public class UserUpdateDto : BaseAddUpdateDto
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

    [Required(ErrorMessage = "Phone number is a required field.")]
    [Phone(ErrorMessage = "Invalid phone number format provided.")]
    public string Phone { get; set; }

    public User CopyTo(User entity)
    {
        entity.FullName = FullName;
        entity.Username = Username;
        entity.Email = Email;
        entity.Phone = Phone;

        return entity;
    }
}

public class SetPasswordDto
{
    [Required(ErrorMessage = "Old password cannot be empty.")]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "Password hash cannot be empty.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Password confirmation cannot be empty.")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }
}

public class UserGetDto : BaseGetDto
{
    public string FullName { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public Membership MemberType { get; set; } = Membership.Basic;

    public string Phone { get; set; }

    public string Role { get; set; }

    public UserGetDto CopyFrom(User entity)
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
