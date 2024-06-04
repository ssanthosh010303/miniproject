/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Ticket : BaseModel
{
    public int MovieShowId { get; set; }
    public MovieShow MovieShow { get; set; }

    public int UserAccountId { get; set; }
    public UserAccount UserAccount { get; set; }

    public int PaymentId { get; set; }
    public Payment Payment { get; set; }

    [Required(ErrorMessage = "Screen number is a required field.")]
    public int ScreenId { get; set; }
    public Screen Screen { get; set; }

    [Required(ErrorMessage = "Booked seats is a required field.")]
    public string SeatsBooked { get; set; }
}
