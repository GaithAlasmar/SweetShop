namespace SweetShop.ViewModels;

public class BranchViewModel
{
    public string Name { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string GoogleMapsEmbedUrl { get; set; } = string.Empty;
    public string WorkingHours { get; set; } = "السبت - الخميس: 8:00 ص - 11:00 م";
    public string GoogleMapsDirectionUrl { get; set; } = "#";
}
