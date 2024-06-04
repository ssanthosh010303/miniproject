/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.DataTransferObjects;

public class TicketListDto : BaseListDto
{
    public string SeatsBooked { get; set; }

    public int UserId { get; set; }

    public int MovieShowId { get; set; }

    public TicketListDto CopyFrom(Ticket entity)
    {
        Id = entity.Id;
        SeatsBooked = entity.SeatsBooked;
        UserId = entity.UserAccountId;
        MovieShowId = entity.MovieShowId;

        return this; // XXX
    }
}

public class TicketAddUpdateDto : BaseAddUpdateDto
{
    public int MovieShowId { get; set; }

    public int UserId { get; set; }

    public int PaymentId { get; set; }

    [Required(ErrorMessage = "Screen number is a required field.")]
    public int ScreenId { get; set; }

    [Required(ErrorMessage = "Booked seats is a required field.")]
    [RegularExpression(@"^[A-Za-z]\d{2}(,[A-Za-z]\d{2})*$",
        ErrorMessage = "Invalid seats format. Seats should be in the format \"AXX,BYY\".")]
    public string SeatsBooked { get; set; }

    [Required(ErrorMessage = "Total price is a required field.")]
    [Range(0, 10000.0, ErrorMessage = "Total price must be between 0 and 10000.")]
    public decimal TotalPrice { get; set; }

    public Ticket CopyTo(Ticket entity)
    {
        entity.ScreenId = ScreenId;
        entity.SeatsBooked = SeatsBooked;

        entity.UserAccountId = UserId;
        entity.MovieShowId = MovieShowId;
        entity.PaymentId = PaymentId;

        return entity;
    }
}

public class TicketGetDto : BaseGetDto
{
    public string SeatsBooked { get; set; }

    public string BookedUser { get; set; }

    public string MovieShowName { get; set; }

    public TicketGetDto CopyFrom(Ticket entity)
    {
        Id = entity.Id;
        IsActive = entity.IsActive;
        CreatedOn = entity.CreatedOn;
        UpdatedOn = entity.UpdatedOn;

        SeatsBooked = entity.SeatsBooked;
        BookedUser = entity.UserAccount.FullName;
        MovieShowName = entity.MovieShow.Movie.Name;

        return this; // XXX
    }
}

public class TicketPaymentGatewayGenerateDto : BaseDto
{
    public string ClientToken { get; set; }
}
