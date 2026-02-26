using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SweetShop.Models;

public class ProductReview
{
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }
    public Product Product { get; set; } = default!;

    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;

    [Required]
    [Range(1, 5, ErrorMessage = "التقييم يجب أن يكون بين 1 و 5 نجوم")]
    public int Rating { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Set to true by default for immediate display, or false to require admin approval
    public bool IsApproved { get; set; } = true;
}
