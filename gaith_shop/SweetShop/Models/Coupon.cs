using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SweetShop.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "كود الخصم مطلوب")]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Range(1, 100, ErrorMessage = "نسبة الخصم يجب أن تكون بين 1 و 100")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountPercentage { get; set; } // نسبة الخصم (مثلاً 15%)

        [Required]
        public DateTime ExpiryDate { get; set; } // تاريخ الانتهاء

        public bool IsActive { get; set; } = true; // للتحكم بإيقاف الكوبون يدوياً
    }
}
