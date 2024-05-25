#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class SeatType : BaseModel
{
    [Required(ErrorMessage = "Seat-type name cannot be an empty field.")]
    [MaxLength(16, ErrorMessage = "Seat name cannot be more than 16 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Seat price cannot be an empty field.")]
    public double Price { get; set; }
}
