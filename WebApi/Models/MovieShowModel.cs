/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Models;

[Index(nameof(ShowTime), IsUnique = true)]
public class MovieShow : BaseModel
{
    public int MovieId { get; set; }
    public Movie Movie { get; set; }

    [Required]
    public DateTime ShowTime { get; set; }

    [Required]
    public int BlockDurationBeforeMovieShowMins { get; set; } = 30;

    [Required]
    public int BlockDurationAfterMovieShowMins { get; set; }

    public ICollection<Screen> Screens { get; set; }
}
