#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Models;

public enum Membership
{
    Basic,
    Standard,
    Premium
}

[Index(nameof(User.Username), IsUnique = true)]
[Index(nameof(User.Email), IsUnique = true)]
public class User : BaseModel
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
    public string PasswordHash { get; set; }

    [Required(ErrorMessage = "Membership is a required field.")]
    [EnumDataType(typeof(Membership))]
    public Membership MemberType { get; set; }

    [Required(ErrorMessage = "Phone number is a required field.")]
    [Phone(ErrorMessage = "Invalid phone number format provided.")]
    public string Phone { get; set; }

    public int RoleId { get; set; }
    public UserRole Role { get; set; }
}

public class UserRole : BaseModel
{
    [MaxLength(16,
        ErrorMessage = "Role name cannot be more than 16 characters.")]
    public string Name { get; set; }
}
