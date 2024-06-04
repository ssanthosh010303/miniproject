/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public enum MovieGenre
{
    Action,
    Adventure,
    Comedy,
    Drama,
    Fantasy,
    Horror,
    Mystery,
    Romance,
    SciFi,
    Thriller,
    Western,
    Animation,
    Documentary,
    Biography,
    Crime,
    Family,
    History,
    Musical,
    Sport,
    War,
    Other,
    All
}

public enum DisplayTech
{
    Imax2D,
    Imax3D,
    DolbyCinema,
    RealD3d,
    FourDx,
    ScreenX,
    Other
}

public enum AudioTech
{
    DolbyAtmos,
    DtsX,
    Auro3D,
    ImaxSound,
    Other
}

public class Movie : BaseModel
{
    [Required(ErrorMessage = "Movie name is a required field.")]
    [MaxLength(64, ErrorMessage = "Movie name cannot be more than 64 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Movie genre is a required field.")]
    [EnumDataType(typeof(MovieGenre))]
    public MovieGenre Genre { get; set; }

    [Required(ErrorMessage = "Movie release date is a required field.")]
    public DateTime ReleasedOn { get; set; }

    [Required(ErrorMessage = "Studio name is a required field.")]
    [MaxLength(64, ErrorMessage = "Studio name cannot be more than 64 characters.")]
    public string StudioName { get; set; }

    [Required(ErrorMessage = "Runtime is a required field.")]
    public TimeOnly Runtime { get; set; }

    [Required(ErrorMessage = "Movie rating is a required field.")]
    [Range(0, 5.0)]
    public float Rating { get; set; }

    [Required(ErrorMessage = "Langauges is a required field.")]
    [MaxLength(64, ErrorMessage = "Languages cannot contain more than 64 characters.")]
    public string Languages { get; set; }

    [Required(ErrorMessage = "Display technology is a required field.")]
    [EnumDataType(typeof(DisplayTech),
        ErrorMessage = "Invalid display technology provided.")]
    public DisplayTech DisplayTech { get; set; }

    [Required(ErrorMessage = "Audio technology is a required field.")]
    [EnumDataType(typeof(AudioTech),
        ErrorMessage = "Invalid audio technology provided.")]
    public AudioTech AudioTech { get; set; }

    [Required(ErrorMessage = "Movie description is a required field.")]
    public string Description { get; set; }

    public string Illustration { get; set; }

    // M2M
    public ICollection<Cast> CastMembers { get; set; }
    public ICollection<Review> Reviews { get; set; }
}
