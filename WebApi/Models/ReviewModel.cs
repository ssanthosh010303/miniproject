/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Review : BaseModel
{
    [Required(ErrorMessage = "Comment is a required field.")]
    [MaxLength(256, ErrorMessage = "Comment cannot be longer than 512 characters.")]
    public string Comment { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public float Rating { get; set; }

    public int MovieId { get; set; }
    public Movie Movie { get; set; }
}
