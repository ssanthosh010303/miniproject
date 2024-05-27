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

    [Required(ErrorMessage = "Theater ID is a required field.")]
    public int TheaterId { get; set; }
    public Theater Theater { get; set; }

    [Required(ErrorMessage = "Screen number is a required field.")]
    public int ScreenId { get; set; }
    public Screen Screen { get; set; }

    [Required(ErrorMessage = "Booked seats is a required field.")]
    public string SeatsBooked { get; set; }
}
