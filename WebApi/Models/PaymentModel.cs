#nullable disable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public enum PaymentMethod
{
    DebitCard,
    CreditCard,
    NetBanking,
    UnifiedPaymentsInterface
}

public enum PaymentStatus
{
    Success,
    Pending,
    Failed,
    Refunded
}

public class Payment : BaseModel
{
    [Required(ErrorMessage = "Amount is a required field.")]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Payment date is a required field.")]
    [DataType(DataType.Date)]
    public DateTime PaymentDate { get; set; }

    [Required(ErrorMessage = "Payment method is a required field.")]
    [EnumDataType(typeof(PaymentMethod))]
    public PaymentMethod PaymentMethod { get; set; }

    [Required(ErrorMessage = "Payment status is a required field.")]
    [EnumDataType(typeof(PaymentStatus))]
    public PaymentStatus PaymentStatus { get; set; }

    public int? PromoId { get; set; }
    public Promo Promo { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }
}
