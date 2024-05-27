#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.DataTransferObjects;

public class ScreenListDto : BaseListDto
{
    public string Name { get; set; }

    public ScreenListDto CopyFrom(Screen entity)
    {
        Id = entity.Id;
        Name = entity.Name;

        return this; // XXX
    }
}

public class ScreenAddUpdateDto : BaseAddUpdateDto
{
    [Required]
    [MaxLength(16,
        ErrorMessage = "Screen name cannot be more than 16 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Display technology is a required field.")]
    [MaxLength(64, ErrorMessage = "Display technology cannot exceed 64 characters.")]
    public string DisplayTech { get; set; }

    [Required(ErrorMessage = "Audio technology is a required field.")]
    [MaxLength(64, ErrorMessage = "Audio technology cannot exceed 64 characters.")]
    public string AudioTech { get; set; }

    [Required]
    public int TheaterId { get; set; }

    public Screen CopyTo(Screen entity)
    {
        entity.Name = Name;
        entity.DisplayTech = DisplayTech;
        entity.AudioTech = AudioTech;
        entity.TheaterId  = TheaterId;

        return entity;
    }
}

public class ScreenGetDto : BaseGetDto
{
    public string Name { get; set; }

    public string DisplayTech { get; set; }

    public string AudioTech { get; set; }

    public string TheaterName { get; set; }

    public ScreenGetDto CopyFrom(Screen entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        DisplayTech = entity.DisplayTech;
        AudioTech = entity.AudioTech;
        TheaterName = entity.Theater.Name;

        return this; // XXX
    }
}
