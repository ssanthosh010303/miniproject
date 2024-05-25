#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.DataTransferObjects;

public class MovieShowListDto : BaseListDto
{
    public string MovieName { get; set; }

    public DateTime ShowTime { get; set; }

    public MovieShowListDto CopyFrom(MovieShow entity)
    {
        Id = entity.Id;
        MovieName = entity.Movie.Name;
        ShowTime = entity.ShowTime;

        return this; // XXX
    }
}

public class MovieShowAddUpdateDto : BaseAddUpdateDto
{
    public int MovieId { get; set; }

    public DateTime ShowTime { get; set; }

    public MovieShow CopyTo(MovieShow entity)
    {
        entity.MovieId = MovieId;
        entity.ShowTime = ShowTime;

        return entity;
    }
}

public class MovieShowGetDto : BaseGetDto
{
    public string MovieName { get; set; }

    public DateTime ShowTime { get; set; }

    public MovieShowGetDto CopyFrom(MovieShow entity)
    {
        Id = entity.Id;
        IsActive = entity.IsActive;
        CreatedOn = entity.CreatedOn;
        UpdatedOn = entity.UpdatedOn;

        MovieName = entity.Movie.Name;
        ShowTime = entity.ShowTime;

        return this;
    }
}
