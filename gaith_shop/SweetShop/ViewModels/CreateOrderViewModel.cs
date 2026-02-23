using System.ComponentModel.DataAnnotations;

namespace SweetShop.ViewModels;

public class CreateOrderViewModel
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

    public List<OrderItemViewModel> Items { get; set; } = [];
}
