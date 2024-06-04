/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.DataTransferObjects;

public class TheaterListDto : BaseListDto
{
    public string Name { get; set; }

    public string City { get; set; }

    public TheaterListDto CopyFrom(Theater entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        City = entity.City;

        return this; // XXX
    }
}

public class TheaterAddUpdateDto : BaseAddUpdateDto
{
    [Required(ErrorMessage = "Name field is required.")]
    [MaxLength(64, ErrorMessage = "Name field cannot have more than 64 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Theater address is a required field.")]
    public string City { get; set; }

    public Theater CopyTo(Theater entity)
    {
        entity.Name = Name;
        entity.City = City;

        return entity;
    }
}

public class TheaterGetDto : BaseGetDto
{
    public string Name { get; set; }

    public string Address { get; set; }

    public string City { get; set; }

    public int ZipCode { get; set; }

    public ICollection<TheaterGetFacilitiesListDto> Facilities { get; set; } = [];
    public ICollection<TheaterGetScreensDto> Screens { get; set; } = [];

    public TheaterGetDto CopyFrom(Theater entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        City = entity.City;

        foreach (var facility in entity.Facilities)
        {
            Facilities.Add(
                new TheaterGetFacilitiesListDto().CopyFrom(facility));
        }

        foreach (var screen in entity.Screens)
        {
            Screens.Add(new TheaterGetScreensDto().CopyFrom(screen));
        }

        return this; // XXX
    }
}

public class TheaterAddRemoveFacilitiesDto : BaseDto
{
    public ICollection<int> FacilityIds { get; set; }
}

public class TheaterGetFacilitiesListDto : BaseDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public TheaterGetFacilitiesListDto CopyFrom(Facility entity)
    {
        Id = entity.Id;
        Name = entity.Name;

        return this; // XXX
    }
}

public class TheaterGetScreensDto : BaseDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public DisplayTech DisplayTech { get; set; }

    public AudioTech AudioTech { get; set; }

    public TheaterGetScreensDto CopyFrom(Screen entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        DisplayTech = entity.DisplayTech;
        AudioTech = entity.AudioTech;

        return this; // XXX
    }
}
