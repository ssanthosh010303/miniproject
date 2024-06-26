/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
#nullable disable

using System.ComponentModel.DataAnnotations;

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
    [Required(ErrorMessage = "Payment date is a required field.")]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Required(ErrorMessage = "Payment method is a required field.")]
    [EnumDataType(typeof(PaymentMethod))]
    public PaymentMethod Method { get; set; }

    [Required(ErrorMessage = "Payment status is a required field.")]
    [EnumDataType(typeof(PaymentStatus))]
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    [Required(ErrorMessage = "Amount paid is a required field.")]
    [Range(0, 10000.0, ErrorMessage = "Amount paid must be between 0 and 10000.")]
    public double AmountPaid { get; set; } = 0;

    public int PayerId { get; set; }
    public UserAccount Payer { get; set; }
}
