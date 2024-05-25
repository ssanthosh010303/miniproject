#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.DataTransferObjects;

public class CastListDto : BaseListDto
{
    public string Name { get; set; }

    public CastListDto CopyFrom(Cast entity)
    {
        Id = entity.Id;
        Name = entity.Name;

        return this; // XXX
    }
}

public class CastAddUpdateDto : BaseAddUpdateDto
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

    public Cast CopyTo(Cast entity)
    {
        entity.Name = Name;
        entity.Title = Title;
        entity.Birthday = Birthday;
        entity.Birthplace = Birthplace;
        entity.Description = Description;
        entity.Illustration = Illustration;

        return entity;
    }
}

public class CastGetDto : BaseGetDto
{
    public string Name { get; set; }

    public string Title { get; set; }

    public DateTime Birthday { get; set; }

    public string Birthplace { get; set; }

    public string Description { get; set; }

    public string Illustration { get; set; }

    public ICollection<CastGetMovieListDto> Movies { get; set; } = [];

    public CastGetDto CopyFrom(Cast entity)
    {
        Id = entity.Id;
        IsActive = entity.IsActive;
        CreatedOn = entity.CreatedOn;
        UpdatedOn = entity.UpdatedOn;

        Name = entity.Name;
        Title = entity.Title;
        Birthday = entity.Birthday;
        Birthplace = entity.Birthplace;
        Description = entity.Description;
        Illustration = entity.Illustration;

        foreach (var movie in entity.Movies)
            Movies.Add(new CastGetMovieListDto().CopyFrom(movie));

        return this;
    }
}

public class CastGetMovieListDto : BaseDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public CastGetMovieListDto CopyFrom(Movie entity)
    {
        Id = entity.Id;
        Name = entity.Name;

        return this;
    }
}
