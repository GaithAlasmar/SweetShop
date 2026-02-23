using System.ComponentModel.DataAnnotations;

namespace SweetShop.Models;

public class SiteSettings
{
    public int Id { get; set; }

    [Display(Name = "اسم الموقع")]
    public string SiteName { get; set; } = "حلويات الاسمر";

    [Display(Name = "رسالة الترحيب")]
    public string WelcomeMessage { get; set; } = "أهلاً بكم في حلويات الاسمر";

    [Display(Name = "رقم التواصل")]
    public string ContactPhone { get; set; } = "+962 79 123 4567";

    [Display(Name = "البريد الإلكتروني")]
    [EmailAddress]
    public string ContactEmail { get; set; } = "info@sweetshop.com";

    [Display(Name = "العنوان")]
    public string Address { get; set; } = "عمان، الأردن";

    [Display(Name = "رسالة التذييل")]
    public string FooterMessage { get; set; } = "جميع الحقوق محفوظة © حلويات الاسمر";
}
