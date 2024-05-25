#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Theater : BaseModel
{
    [Required(ErrorMessage = "Name field is required.")]
    [MaxLength(64, ErrorMessage = "Name field cannot have more than 64 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Theater address is a required field.")]
    public string Address { get; set; }

    [Required(ErrorMessage = "Display technology is a required field.")]
    [MaxLength(64, ErrorMessage = "Display technology cannot exceed 64 characters.")]
    public string DisplayTech { get; set; }

    [Required(ErrorMessage = "Audio technology is a required field.")]
    [MaxLength(64, ErrorMessage = "Audio technology cannot exceed 64 characters.")]
    public string AudioTech { get; set; }

    // M2M
    public ICollection<SeatType> SeatTypes { get; set; }
    public ICollection<Facility> Facilities { get; set; }
    public ICollection<MovieShow> MovieShows { get; set; }
}
