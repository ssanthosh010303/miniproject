#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.DataTransferObjects;

public class PaymentListDto : BaseListDto
{
    public DateTime Date { get; set; }

    public PaymentListDto CopyFrom(Payment entity)
    {
        Id = entity.Id;
        Date = entity.Date;

        return this; // XXX
    }
}

public class PaymentAddDto : BaseAddUpdateDto
{

    [Required(ErrorMessage = "Payment date is a required field.")]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "Payment method is a required field.")]
    [EnumDataType(typeof(PaymentMethod))]
    public PaymentMethod Method { get; set; }

    [Required(ErrorMessage = "Booked seats is a required field.")]
    [RegularExpression(@"^[A-Za-z]\d{1,2}(,[A-Za-z]\d{1,2})*$",
        ErrorMessage = "Invalid seats format. Seats should be in the format \"AXX,BYY\".")]
    public string SeatsBooked { get; set; }

    public int? PromoId { get; set; }

    [Required(ErrorMessage = "Screen ID is a required field.")]
    public int ScreenId { get; set; }

    [Required(ErrorMessage = "Theater ID is a required field.")]
    public int TheaterId { get; set; }

    [Required(ErrorMessage = "Payer ID is a required field.")]
    public int MovieShowId { get; set; }

    public Payment CopyTo(Payment entity)
    {
        entity.Date = Date;
        entity.Method = Method;
        entity.PromoId = PromoId;

        return entity;
    }
}

public class PaymentSuccessDto : BaseDto
{
    [Required(ErrorMessage = "Movie show ID is a required field.")]
    public int MovieShowId { get; set; }

    [Required(ErrorMessage = "Yser ID is a required field.")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Payment ID is a required field.")]
    public int PaymentId { get; set; }

    [Required(ErrorMessage = "Theater ID is a required field.")]
    public int TheaterId { get; set; }

    [Required(ErrorMessage = "Screen number is a required field.")]
    public int ScreenId { get; set; }

    [Required(ErrorMessage = "Booked seats is a required field.")]
    public string SeatsBooked { get; set; }

    public Ticket CopyTo(Ticket entity)
    {
        entity.MovieShowId = MovieShowId;
        entity.UserId = UserId;
        entity.PaymentId = PaymentId;
        entity.TheaterId = TheaterId;
        entity.ScreenId = ScreenId;
        entity.SeatsBooked = SeatsBooked;

        return entity;
    }
}

public class PaymentGetDto : BaseGetDto
{
    public DateTime Date { get; set; }

    public PaymentMethod Method { get; set; }

    public PaymentStatus Status { get; set; }

    public double AmountPaid { get; set; }

    public int? PromoId { get; set; }
    public string PromoCode { get; set; }

    public int PayerId { get; set; }
    public string PayerName { get; set; }

    public PaymentGetDto CopyFrom(Payment entity)
    {
        Id = entity.Id;
        IsActive = entity.IsActive;
        CreatedOn = entity.CreatedOn;
        UpdatedOn = entity.UpdatedOn;

        Date = entity.Date;
        Method = entity.Method;
        Status = entity.Status;
        AmountPaid = entity.AmountPaid;
        PromoId = entity.PromoId;
        PayerId = entity.PayerId;
        PayerName = entity.Payer.FullName;

        if (entity.Promo != null)
        {
            PromoCode = entity.Promo.Code;
        }

        return this; // XXX
    }
}
