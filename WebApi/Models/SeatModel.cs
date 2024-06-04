/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Models;

[PrimaryKey(nameof(Id), nameof(ScreenId))]
public class Seat
{
    [MaxLength(3, ErrorMessage = "Seat ID cannot be more than 3 characters.")]
    [RegularExpression(@"^[A-Z]\d{2}$",
        ErrorMessage = "Seat ID must be in the format (A-Z)(0-9)(0-9)")]
    public string Id { get; set; }

    public int ScreenId { get; set; }
    public Screen Screen { get; set; }

    public int SeatTypeId { get; set; }
    public SeatType SeatType { get; set; }

    public int? BookedForShowId { get; set; }
    public MovieShow BookedForShow { get; set; }

    public int? UserAccountId { get; set; }
    public UserAccount UserAccount { get; set; }

    public DateTime? TemporaryLockUntil { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedOn { get; set; }
}
