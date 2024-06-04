/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
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

[Index(nameof(Username), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
[Index(nameof(Phone), IsUnique = true)]
public class UserAccount : BaseModel
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
    public Membership MemberType { get; set; } = Membership.Basic;

    [Required(ErrorMessage = "Phone number is a required field.")]
    [Phone(ErrorMessage = "Invalid phone number format provided.")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "City of residence is a required field.")]
    [MaxLength(64, ErrorMessage = "City cannot be more than 64 characters.")]
    public string City { get; set; }

    [Required(ErrorMessage = "Role is a required field.")]
    [MaxLength(128, ErrorMessage = "Role cannot be more than 128 characters.")]
    public string Role { get; set; }

    public string Illustration { get; set; }
}
