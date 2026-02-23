using Microsoft.AspNetCore.Mvc;
using SweetShop.ViewModels;

namespace SweetShop.Controllers;

public class BranchesController : Controller
{
    // GET: Branches
    public IActionResult Index()
    {
        // Dummy branch data - replace with actual data later
        var branches = new List<BranchViewModel>
        {
            new BranchViewModel
            {
                Name = "فرع عمان - الدوار السابع",
                PhotoUrl = "https://images.unsplash.com/photo-1556910103-1c02745aae4d?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80",
                Address = "شارع الجامعة، الدوار السابع، عمان - الأردن",
                PhoneNumber = "+962 6 123 4567",
                WorkingHours = "السبت - الخميس: 8:00 ص - 11:00 م | الجمعة: 2:00 م - 11:00 م",
                GoogleMapsDirectionUrl = "https://www.google.com/maps/dir/?api=1&destination=31.9454,35.8728",
                GoogleMapsEmbedUrl = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3384.6158!2d35.8728!3d31.9454!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x0%3A0x0!2zMzHCsDU2JzQzLjQiTiAzNcKwNTInMjIuMSJF!5e0!3m2!1sen!2sjo!4v1234567890"
            },
            new BranchViewModel
            {
                Name = "فرع عمان - الشميساني",
                PhotoUrl = "https://images.unsplash.com/photo-1567521464027-f127ff144326?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80",
                Address = "شارع الملكة رانيا العبدالله، الشميساني، عمان - الأردن",
                PhoneNumber = "+962 6 234 5678",
                WorkingHours = "السبت - الخميس: 8:00 ص - 11:00 م | الجمعة: 2:00 م - 11:00 م",
                GoogleMapsDirectionUrl = "https://www.google.com/maps/dir/?api=1&destination=31.9615,35.8889",
                GoogleMapsEmbedUrl = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3384.0!2d35.8889!3d31.9615!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x0%3A0x0!2zMzHCsDU3JzQxLjQiTiAzNcKwNTMnMjAuMCJF!5e0!3m2!1sen!2sjo!4v1234567890"
            },
            new BranchViewModel
            {
                Name = "فرع إربد - شارع الجامعة",
                PhotoUrl = "https://images.unsplash.com/photo-1514933651103-005eec06c04b?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80",
                Address = "شارع الجامعة، مقابل الجامعة الأردنية، إربد - الأردن",
                PhoneNumber = "+962 2 345 6789",
                WorkingHours = "السبت - الخميس: 9:00 ص - 10:00 م | الجمعة: 2:00 م - 10:00 م",
                GoogleMapsDirectionUrl = "https://www.google.com/maps/dir/?api=1&destination=32.5333,35.8333",
                GoogleMapsEmbedUrl = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3348.0!2d35.8333!3d32.5333!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x0%3A0x0!2zMzLCsDMyJzAwLjAiTiAzNcKwNTAnMDAuMCJF!5e0!3m2!1sen!2sjo!4v1234567890"
            },
            new BranchViewModel
            {
                Name = "فرع الزرقاء - وسط البلد",
                PhotoUrl = "https://images.unsplash.com/photo-1559329007-40df8a9345d8?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80",
                Address = "شارع الملك طلال، وسط البلد، الزرقاء - الأردن",
                PhoneNumber = "+962 5 456 7890",
                WorkingHours = "السبت - الخميس: 8:00 ص - 10:30 م | الجمعة: 2:00 م - 10:30 م",
                GoogleMapsDirectionUrl = "https://www.google.com/maps/dir/?api=1&destination=32.0722,36.0880",
                GoogleMapsEmbedUrl = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3381.0!2d36.0880!3d32.0722!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x0%3A0x0!2zMzLCsDA0JzIwLjAiTiAzNsKwMDUnMTcuMCJF!5e0!3m2!1sen!2sjo!4v1234567890"
            }
        };

        return View(branches);
    }
}
