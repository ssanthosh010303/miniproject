/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.DataTransferObjects;

public class MovieListDto : BaseListDto
{
    public string Name { get; set; }
    public string Illustration { get; set; }

    public MovieListDto CopyFrom(Movie entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        Illustration = entity.Illustration;

        return this; // XXX
    }
}

public class MovieAddUpdateDto : BaseAddUpdateDto
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

    public Movie CopyTo(Movie entity)
    {
        entity.Name = Name;
        entity.Genre = Genre;
        entity.ReleasedOn = ReleasedOn;
        entity.StudioName = StudioName;
        entity.Runtime = Runtime;
        entity.Rating = Rating;
        entity.Languages = Languages;
        entity.DisplayTech = DisplayTech;
        entity.AudioTech = AudioTech;
        entity.Description = Description;
        entity.Illustration = Illustration;

        return entity;
    }
}

public class MovieGetDto : BaseGetDto
{
    public string Name { get; set; }

    public MovieGenre Genre { get; set; }

    public DateTime ReleasedOn { get; set; }

    public string StudioName { get; set; }

    public TimeOnly Runtime { get; set; }

    public float Rating { get; set; }

    public string Languages { get; set; }

    public DisplayTech DisplayTech { get; set; }

    public AudioTech AudioTech { get; set; }

    public string Description { get; set; }

    public string Illustration { get; set; }

    public List<MovieGetCastListDto> CastMembers { get; set; } = [];
    public List<MovieGetReviewListDto> Reviews { get; set; } = [];

    public MovieGetDto CopyFrom(Movie entity)
    {
        Id = entity.Id;
        IsActive = entity.IsActive;
        CreatedOn = entity.CreatedOn;
        UpdatedOn = entity.UpdatedOn;

        Name = entity.Name;
        Genre = entity.Genre;
        ReleasedOn = entity.ReleasedOn;
        StudioName = entity.StudioName;
        Runtime = entity.Runtime;
        Rating = entity.Rating;
        Languages = entity.Languages;
        DisplayTech = entity.DisplayTech;
        AudioTech = entity.AudioTech;
        Description = entity.Description;
        Illustration = entity.Illustration;

        foreach (var castMember in entity.CastMembers)
        {
            CastMembers.Add(
                new MovieGetCastListDto().CopyFrom(castMember)
            );
        }

        // foreach (var review in entity.Reviews)
        // {
        //     Reviews.Add(
        //         new MovieGetReviewListDto().CopyFrom(review)
        //     );
        // }

        return this;
    }
}

public class MovieAddRemoveCastMembersDto : BaseDto
{
    public List<int> CastMemberIds { get; set; }
}

public class MovieGetCastListDto : BaseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Illustration { get; set; }

    public MovieGetCastListDto CopyFrom(Cast entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        Illustration = entity.Illustration;

        return this;
    }
}

public class MovieGetReviewListDto : BaseDto
{
    public int Id { get; set; }
    public string Comment { get; set; }
    public float Rating { get; set; }

    public MovieGetReviewListDto CopyFrom(Review entity)
    {
        Id = entity.Id;
        Comment = entity.Comment;
        Rating = entity.Rating;

        return this;
    }
}
