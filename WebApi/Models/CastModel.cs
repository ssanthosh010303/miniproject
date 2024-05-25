#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Cast : BaseModel
{
    [Required(ErrorMessage = "Name field is required.")]
    [MaxLength(64, ErrorMessage = "Name field cannot have more than 64 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Title field is required.")]
    [MaxLength(64, ErrorMessage = "Title field cannot be more than 64 characters.")]
    public string Title { get; set; }

    public DateTime Birthday { get; set; }

    [MaxLength(32, ErrorMessage = "Birthplace field cannot be more than 32 characters.")]
    public string Birthplace { get; set; }

    public string Description { get; set; }

    public string Illustration { get; set; }

    // M2M
    public ICollection<Movie> Movies { get; set; }
}
