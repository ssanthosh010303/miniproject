#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.DataTransferObjects;

public class TheaterListDto : BaseListDto
{
    [Required]
    [MaxLength(64)]
    public string Name { get; set; }

    public TheaterListDto CopyFrom(Theater entity)
    {
        Id = entity.Id;
        Name = entity.Name;

        return this; // XXX
    }
}

public class TheaterAddUpdateDto : BaseAddUpdateDto
{
    [Required(ErrorMessage = "Name field is required.")]
    [MaxLength(64, ErrorMessage = "Name field cannot have more than 64 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Theater address is a required field.")]
    public string Address { get; set; }

    [Required(ErrorMessage = "Display technology is a required field.")]
    [MaxLength(64, ErrorMessage = "Display technology cannot exceed 64 characters.")]
    public string DisplayTech { get; set; }

    [Required(ErrorMessage = "Audio technology is a required field.")]
    [MaxLength(64, ErrorMessage = "Audio technology cannot exceed 64 characters.")]
    public string AudioTech { get; set; }

    public Theater CopyTo(Theater entity)
    {
        entity.Name = Name;
        entity.Address = Address;
        entity.DisplayTech = DisplayTech;
        entity.AudioTech = AudioTech;

        return entity;
    }
}

public class TheaterGetDto : BaseGetDto
{
    public string Name { get; set; }

    public string Address { get; set; }

    public string DisplayTech { get; set; }

    public string AudioTech { get; set; }

    public ICollection<TheaterGetFacilitiesListDto> Facilities { get; set; } = [];

    public TheaterGetDto CopyFrom(Theater entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        Address = entity.Address;
        DisplayTech = entity.DisplayTech;
        AudioTech = entity.AudioTech;

        foreach (var facility in entity.Facilities)
        {
            Facilities.Add(
                new TheaterGetFacilitiesListDto().CopyFrom(facility));
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
