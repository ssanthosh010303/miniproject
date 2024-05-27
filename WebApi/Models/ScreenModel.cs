#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Screen : BaseModel
{
    [Required(ErrorMessage = "Screen name is a required field.")]
    [MaxLength(64, ErrorMessage = "Screen name cannot exceed 64 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Display technology is a required field.")]
    [MaxLength(64, ErrorMessage = "Display technology cannot exceed 64 characters.")]
    public string DisplayTech { get; set; }

    [Required(ErrorMessage = "Audio technology is a required field.")]
    [MaxLength(64, ErrorMessage = "Audio technology cannot exceed 64 characters.")]
    public string AudioTech { get; set; }

    public int TheaterId { get; set; }
    public Theater Theater { get; set; }
}
