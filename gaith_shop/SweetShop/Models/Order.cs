using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SweetShop.Models;

public class Order
{
    public int Id { get; set; }

    public List<OrderDetail> OrderDetails { get; set; } = new();

    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;
    [Required]
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    [Required]
    public string ZipCode { get; set; } = string.Empty;
    [Required]
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    [Required]
    public string Country { get; set; } = string.Empty;
    [Required]
    public string PhoneNumber { get; set; } = string.Empty;
    [Required]
    public string Email { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal OrderTotal { get; set; }

    public DateTime OrderPlaced { get; set; }

    [Display(Name = "الحالة")]
    public string Status { get; set; } = "Pending";

    [Display(Name = "حالة الدفع")]
    public string PaymentStatus { get; set; } = "Pending";

    public string? TransactionId { get; set; }

    public bool IsDeleted { get; set; } = false;
}

public class OrderDetail
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }

    public int? ProductVariantId { get; set; }
    public ProductVariant? ProductVariant { get; set; }
    public int Amount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public Product Product { get; set; } = default!;
    public Order Order { get; set; } = default!;
}
