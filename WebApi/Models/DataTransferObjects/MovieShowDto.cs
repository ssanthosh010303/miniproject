/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
#nullable disable

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

    public int BlockDurationBeforeMovieShowMins { get; set; }

    public int BlockDurationAfterMovieShowMins { get; set; }

    public ICollection<MovieShowGetScreensDto> Theaters { get; set; } = [];

    public MovieShowGetDto CopyFrom(MovieShow entity)
    {
        Id = entity.Id;
        IsActive = entity.IsActive;
        CreatedOn = entity.CreatedOn;
        UpdatedOn = entity.UpdatedOn;

        MovieName = entity.Movie.Name;
        ShowTime = entity.ShowTime;
        BlockDurationBeforeMovieShowMins = entity.BlockDurationBeforeMovieShowMins;
        BlockDurationAfterMovieShowMins = entity.BlockDurationAfterMovieShowMins;

        foreach (var screen in entity.Screens)
        {
            Theaters.Add(new MovieShowGetScreensDto().CopyFrom(screen));
        }

        return this;
    }
}

public class MovieShowGetScreensDto : BaseListDto
{
    public string ScreenName { get; set; }

    public DisplayTech DisplayTech { get; set; }

    public AudioTech AudioTech { get; set; }

    public MovieShowGetScreensDto CopyFrom(Screen entity)
    {
        Id = entity.Id;
        ScreenName = entity.Name;
        DisplayTech = entity.DisplayTech;
        AudioTech = entity.AudioTech;

        return this;
    }
}
