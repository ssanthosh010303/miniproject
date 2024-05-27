#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.DataTransferObjects;

public class PromoListDto : BaseListDto
{
    public string Code { get; set; }

    public PromoListDto CopyFrom(Promo entity)
    {
        Id = entity.Id;
        Code = entity.Code;

        return this; // XXX
    }
}

public class PromoAddUpdateDto : BaseAddUpdateDto
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
    public double MinimumPurchase { get; set;}

    [Required(ErrorMessage = "Allowed payment method is required.")]
    [EnumDataType(typeof(PaymentMethod), ErrorMessage = "Invalid payment method.")]
    public PaymentMethod AllowedPaymentMethod { get; set; }

    [Required(ErrorMessage = "Valid from date is required.")]
    [DataType(DataType.Date)]
    public DateTime ValidFrom { get; set; }

    [Required(ErrorMessage = "Valid to date is required.")]
    [DataType(DataType.Date)]
    public DateTime ValidTo { get; set; }

    public Promo CopyTo(Promo entity)
    {
        entity.Code = Code;
        entity.Description = Description;
        entity.DiscountPercent = DiscountPercent;
        entity.MinimumPurchase = MinimumPurchase;
        entity.AllowedPaymentMethod = AllowedPaymentMethod;
        entity.ValidFrom = ValidFrom;
        entity.ValidTo = ValidTo;

        return entity;
    }
}

public class PromoGetDto : BaseGetDto
{
    public string Code { get; set; }

    public string Description { get; set; }

    public int DiscountPercent { get; set; }

    public double MinimumPurchase { get; set;}

    public PaymentMethod AllowedPaymentMethod { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime ValidTo { get; set; }

    public PromoGetDto CopyFrom(Promo entity)
    {
        Id = entity.Id;
        IsActive = entity.IsActive;
        CreatedOn = entity.CreatedOn;
        UpdatedOn = entity.UpdatedOn;

        Code = entity.Code;
        Description = entity.Description;
        DiscountPercent = entity.DiscountPercent;
        MinimumPurchase = entity.MinimumPurchase;
        AllowedPaymentMethod = entity.AllowedPaymentMethod;
        ValidFrom = entity.ValidFrom;
        ValidTo = entity.ValidTo;

        return this;
    }
}

