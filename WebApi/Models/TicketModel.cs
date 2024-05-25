#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Ticket : BaseModel
{
    public int MovieShowId { get; set; }
    public MovieShow MovieShow { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public int PaymentId { get; set; }
    public Payment Payment { get; set; }

    [Required(ErrorMessage = "Screen number is a required field.")]
    public int ScreenNumber { get; set; }

    [Required(ErrorMessage = "Booked seats is a required field.")]
    public string SeatsBooked { get; set; }

    [Required(ErrorMessage = "Total price is a required field.")]
    [Range(0, 10000.0, ErrorMessage = "Total price must be between 0 and 10000.")]
    public decimal TotalPrice { get; set; }
}
