/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Facility : BaseModel
{
    [Required]
    [MaxLength(16, ErrorMessage = "Facility name cannot be more than 16 characters.")]
    public string Name { get; set; }

    public string Illustration { get; set; }

    public string Description { get; set; }

    // M2M
    public ICollection<Theater> Theaters { get; set; }
}
