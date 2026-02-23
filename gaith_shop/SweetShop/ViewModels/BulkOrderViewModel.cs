using System.ComponentModel.DataAnnotations;
using SweetShop.Models;

namespace SweetShop.ViewModels;

public class BulkOrderViewModel
{
    [Required(ErrorMessage = "الرجاء إدخال اسمك")]
    [Display(Name = "اسم العميل")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "الرجاء إدخال رقم الهاتف")]
    [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
    [Display(Name = "رقم الهاتف")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "الرجاء اختيار تاريخ المناسبة")]
    [Display(Name = "تاريخ المناسبة")]
    [DataType(DataType.Date)]
    public DateTime EventDate { get; set; } = DateTime.Now.AddDays(7);

    [Display(Name = "ملاحظات إضافية")]
    [DataType(DataType.MultilineText)]
    public string? AdditionalNotes { get; set; }

    // List of categories with quantities
    public List<CategoryOrderItem> CategoryItems { get; set; } = [];
}

public class CategoryOrderItem
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    [Display(Name = "الكمية")]
    [Range(0, 1000, ErrorMessage = "الكمية يجب أن تكون بين 0 و 1000")]
    public decimal Quantity { get; set; }

    [Display(Name = "الوحدة")]
    public string Unit { get; set; } = "كيلو"; // "كيلو" or "صينية"
}
