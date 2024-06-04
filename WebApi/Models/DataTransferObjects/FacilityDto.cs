/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.DataTransferObjects;

public class FacilityListDto : BaseListDto
{
    public string Name { get; set; }

    public FacilityListDto CopyFrom(Facility entity)
    {
        Id = entity.Id;
        Name = entity.Name;

        return this; // XXX
    }
}

public class FacilityAddUpdateDto : BaseAddUpdateDto
{
    [Required]
    [MaxLength(16,
        ErrorMessage = "Facility name cannot be more than 16 characters.")]
    public string Name { get; set; }

    public string Illustration { get; set; }

    public string Description { get; set; }

    public Facility CopyTo(Facility entity)
    {
        entity.Name = Name;
        entity.Description = Description;
        entity.Illustration = Illustration;

        return entity;
    }
}

public class FacilityGetDto : BaseGetDto
{
    public string Name { get; set; }

    public string Illustration { get; set; }

    public string Description { get; set; }

    public ICollection<FacilityGetTheatersListDto> Theaters { get; set; } = [];

    public FacilityGetDto CopyFrom(Facility entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        Illustration = entity.Illustration;
        Description = entity.Description;

        foreach (var theater in entity.Theaters)
        {
            Theaters.Add(
                new FacilityGetTheatersListDto().CopyFrom(theater));
        }

        return this; // XXX
    }
}

public class FacilityAddTheaterDto : BaseDto
{
    public ICollection<int> TheaterIds { get; set; }
}

public class FacilityGetTheatersListDto : BaseListDto
{
    public string Name { get; set; }

    public FacilityGetTheatersListDto CopyFrom(Theater entity)
    {
        Id = entity.Id;
        Name = entity.Name;

        return this; // XXX
    }
}
