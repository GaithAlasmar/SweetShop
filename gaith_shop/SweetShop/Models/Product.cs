using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SweetShop.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public int CategoryId { get; set; }

    [ValidateNever]
    public Category Category { get; set; } = default!;

    public bool IsPreferredSweet { get; set; }
    public bool InStock { get; set; }

    public bool IsDeleted { get; set; } = false;

    [ValidateNever]
    public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();

    [ValidateNever]
    public ICollection<ProductVariant> Configurations { get; set; } = new List<ProductVariant>();
}
