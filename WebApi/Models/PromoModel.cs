/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Promo : BaseModel
{
    [Required(ErrorMessage = "Promo code is a required field.")]
    [MaxLength(64, ErrorMessage = "Promo code cannot have more than 64 characters.")]
    public string Code { get; set; }

    [Required(ErrorMessage = "Description is a required field.")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Discount percentage is a required field.")]
    [Range(0, 100, ErrorMessage = "Discount percent must be between 0 and 100.")]
    public int DiscountPercent { get; set; }

    [Required(ErrorMessage = "Minimum purchase is required.")]
    [Range(0, 500.0, ErrorMessage = "Minimum purchase must be a positive number.")]
    public double MinimumPurchase { get; set; }

    [Required(ErrorMessage = "Allowed payment method is required.")]
    [EnumDataType(typeof(PaymentMethod), ErrorMessage = "Invalid payment method.")]
    public PaymentMethod AllowedPaymentMethod { get; set; }

    [Required(ErrorMessage = "Valid from date is required.")]
    [DataType(DataType.Date)]
    public DateTime ValidFrom { get; set; }

    [Required(ErrorMessage = "Valid to date is required.")]
    [DataType(DataType.Date)]
    public DateTime ValidTo { get; set; }
}
