#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class MovieShow : BaseModel
{
    public int MovieId { get; set; }
    public Movie Movie { get; set; }

    [Required]
    public DateTime ShowTime { get; set; }

    public ICollection<Theater> Theaters { get; set; }
}
