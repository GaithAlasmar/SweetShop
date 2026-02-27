using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SweetShop.Models;

public class ProductVariant
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    [Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public Product Product { get; set; } = default!;

    [Required(ErrorMessage = "وزن أو حجم المنتج مطلوب")]
    public string Weight { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    [Required]
    [Range(0.01, 10000, ErrorMessage = "يجب أن يكون السعر أكبر من صفر")]
    public decimal Price { get; set; }

    public bool IsDefault { get; set; }
    public bool InStock { get; set; } = true;
}
