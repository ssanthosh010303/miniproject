/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.DataTransferObjects;

public class ReviewListDto : BaseListDto
{
    public string Comment { get; set; }

    public float Rating { get; set; }

    public ReviewListDto CopyFrom(Review entity)
    {
        Id = entity.Id;
        Comment = entity.Comment;
        Rating = entity.Rating;

        return this; // XXX
    }
}

public class ReviewAddUpdateDto : BaseAddUpdateDto
{
    [Required(ErrorMessage = "Comment is a required field.")]
    [MaxLength(512, ErrorMessage = "Comment cannot be longer than 512 characters.")]
    public string Comment { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public float Rating { get; set; }

    public int MovieId { get; set; }

    public Review CopyTo(Review entity)
    {
        entity.Comment = Comment;
        entity.Rating = Rating;

        return entity;
    }
}

public class ReviewGetDto : BaseGetDto
{
    public string Comment { get; set; }

    public float Rating { get; set; }

    public ReviewGetDto CopyFrom(Review entity)
    {
        Id = entity.Id;
        IsActive = entity.IsActive;
        CreatedOn = entity.CreatedOn;
        UpdatedOn = entity.UpdatedOn;

        Comment = entity.Comment;
        Rating = entity.Rating;

        return this; // XXX
    }
}
