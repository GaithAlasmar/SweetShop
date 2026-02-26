using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SweetShop.ViewModels;

public class ProductDetailsViewModel
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal BasePrice { get; set; }

    // The options to display in a dropdown or radio buttons
    public List<SelectListItem> WeightOptions { get; set; } = new List<SelectListItem>();

    // This field receives the selected Variant Id when the user clicks "Add to Cart"
    [Required(ErrorMessage = "يرجى اختيار الوزن أو الحجم المناسب")]
    public int SelectedVariantId { get; set; }

    // Optional: for displaying prices via JS based on selected variant
    public string VariantsPricesJson { get; set; } = string.Empty;

    // Review statistics and list
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public List<SweetShop.Models.ProductReview> Reviews { get; set; } = new List<SweetShop.Models.ProductReview>();

    // For submitting a new review
    [Range(1, 5, ErrorMessage = "يجب اختيار تقييم بين 1 و 5 نجوم")]
    public int NewReviewRating { get; set; }

    [MaxLength(1000, ErrorMessage = "التعليق يجب أن لا يتجاوز 1000 حرف")]
    public string NewReviewComment { get; set; } = string.Empty;
}
