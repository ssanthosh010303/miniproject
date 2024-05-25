#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.DataTransferObjects;

public class SeatTypeListDto : BaseListDto
{
    [Required]
    [MaxLength(64)]
    public string Name { get; set; }

    public SeatTypeListDto CopyFrom(SeatType entity)
    {
        Id = entity.Id;
        Name = entity.Name;

        return this; // XXX
    }
}

public class SeatTypeAddUpdateDto : BaseAddUpdateDto
{
    [Required(ErrorMessage = "Seat-type name cannot be an empty field.")]
    [MaxLength(16, ErrorMessage = "Seat name cannot be more than 16 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Seat price cannot be an empty field.")]
    public double Price { get; set; }

    public SeatType CopyTo(SeatType entity)
    {
        entity.Name = Name;
        entity.Price = Price;

        return entity;
    }
}
