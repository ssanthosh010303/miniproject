/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Theater : BaseModel
{
    [Required(ErrorMessage = "Name field is required.")]
    [MaxLength(64, ErrorMessage = "Name field cannot have more than 64 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Theater city is a required field.")]
    public string City { get; set; }

    // M2M
    public ICollection<Screen> Screens { get; set; }
    public ICollection<SeatType> SeatTypes { get; set; }
    public ICollection<Facility> Facilities { get; set; }
    public ICollection<MovieShow> MovieShows { get; set; }
}
