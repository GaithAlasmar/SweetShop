using System.ComponentModel.DataAnnotations;

namespace SweetShop.ViewModels;

public class OrderItemViewModel
{
    [Required]
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    [Required(ErrorMessage = "الرجاء إدخال الكمية")]
    [Range(0.1, 1000, ErrorMessage = "الكمية يجب أن تكون بين 0.1 و 1000")]
    [Display(Name = "الكمية")]
    public decimal Quantity { get; set; }

    [Required(ErrorMessage = "الرجاء اختيار الوحدة")]
    [Display(Name = "الوحدة")]
    public string Unit { get; set; } = "كيلو";

    [Display(Name = "السعر")]
    public decimal Price { get; set; }
}
