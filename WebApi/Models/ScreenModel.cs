/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Screen : BaseModel
{
    [Required(ErrorMessage = "Screen name is a required field.")]
    [MaxLength(64, ErrorMessage = "Screen name cannot exceed 64 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Display technology is a required field.")]
    [EnumDataType(typeof(DisplayTech),
        ErrorMessage = "Invalid display technology provided.")]
    public DisplayTech DisplayTech { get; set; }

    [Required(ErrorMessage = "Audio technology is a required field.")]
    [EnumDataType(typeof(AudioTech),
        ErrorMessage = "Invalid audio technology provided.")]
    public AudioTech AudioTech { get; set; }

    public int TheaterId { get; set; }
    public Theater Theater { get; set; }

    // M2M
    public ICollection<Seat> Seats { get; set; }
    public ICollection<MovieShow> MovieShows { get; set; }
}
