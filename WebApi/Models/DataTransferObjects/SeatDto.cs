#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.DataTransferObjects;

public class SeatAddUpdateDto : BaseAddUpdateDto
{
    [Required]
    public int TheaterId { get; set; }

    public ICollection<SeatSchemaAddUpdateDto> SeatSchemas { get; set; } = [];
}

public class SeatSchemaAddUpdateDto : BaseAddUpdateDto
{
    [Required]
    [RegularExpression(@"[A-Za-z]-[A-Za-z]",
        ErrorMessage = "Invalid Row Range. The pattern should be \"x-x\" where 'x' is an alphabet.")]
    public string RowRange { get; set; }

    [Required]
    public int ScreenId { get; set; }

    [Required]
    [Range(1, 100,
        ErrorMessage = "Number of columns should be between 1 and 100.")]
    public int NumberOfColumns { get; set; }

    [Required]
    public int SeatTypeId { get; set; }
}

public class SeatGetDto : BaseDto
{
    public string Id { get; set; }

    public string FoundAtTheater { get; set; }

    public string SeatType { get; set; }

    public double Price { get; set; }

    public SeatGetDto CopyFrom(Seat entity)
    {
        Id = entity.Id;
        FoundAtTheater = entity.Theater.Name;
        SeatType = entity.SeatType.Name;
        Price = entity.SeatType.Price;

        return this;
    }
}

public class SeatRequestDto : BaseDto
{
    public int TheaterId { get; set; }
    public int ScreenId { get; set; }
}

public class SeatListDto : BaseDto
{
    public string Id { get; set; }

    public string SeatType { get; set; }

    public SeatListDto CopyFrom(Seat entity)
    {
        Id = entity.Id;
        SeatType = entity.SeatType.Name;

        return this;
    }
}
