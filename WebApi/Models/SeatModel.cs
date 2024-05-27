#nullable disable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Models;

[PrimaryKey(nameof(Id), nameof(TheaterId), nameof(ScreenId))]
public class Seat
{
    [MaxLength(3, ErrorMessage = "Seat ID cannot be more than 3 characters.")]
    [RegularExpression(@"^[A-Z]\d{2}$",
        ErrorMessage = "Seat ID must be in the format (A-Z)(0-9)(0-9)")]
    public string Id { get; set; }

    public int TheaterId { get; set; }
    public Theater Theater { get; set; }

    public int ScreenId { get; set; }
    public Screen Screen { get; set; }

    public int SeatTypeId { get; set; }
    public SeatType SeatType { get; set; }

    [ForeignKey("BookedForShow")]
    public int? ShowId { get; set; }
    public MovieShow BookedForShow { get; set; }

    public bool IsActive { get; set; } = false;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedOn { get; set; }
}
