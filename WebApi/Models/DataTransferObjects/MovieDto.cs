#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.DataTransferObjects;

public class MovieListDto : BaseListDto
{
    public string Name { get; set; }

    public MovieListDto CopyFrom(Movie entity)
    {
        Id = entity.Id;
        Name = entity.Name;

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
    [MaxLength(64, ErrorMessage = "Display technology cannot exceed 64 characters.")]
    public string DisplayTech { get; set; }

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

    public string DisplayTech { get; set; }

    public string Description { get; set; }

    public string Illustration { get; set; }

    public List<MovieGetCastListDto> CastMembers { get; set; } = [];

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
        Description = entity.Description;
        Illustration = entity.Illustration;

        foreach (var castMember in entity.CastMembers)
        {
            CastMembers.Add(
                new MovieGetCastListDto().CopyFrom(castMember)
            );
        }

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

    public MovieGetCastListDto CopyFrom(Cast entity)
    {
        Id = entity.Id;
        Name = entity.Name;

        return this;
    }
}
