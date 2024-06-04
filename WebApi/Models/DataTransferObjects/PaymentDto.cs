/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
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

    [Required(ErrorMessage = "Movie show ID is a required field.")]
    public int MovieShowId { get; set; }

    public Payment CopyTo(Payment entity)
    {
        entity.Method = Method;

        return entity;
    }
}

public class PaymentAddSuccessDto(string clientToken, double payableAmount) : BaseDto
{
    public string ClientToken { get; set; } = clientToken;
    public double payableAmount { get; set; } = payableAmount;
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
        PayerId = entity.PayerId;
        PayerName = entity.Payer.FullName;

        return this; // XXX
    }
}
