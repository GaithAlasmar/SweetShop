using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SweetShop.Data;

namespace SweetShop.Services
{
    public class CouponResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
    }

    public interface ICouponService
    {
        Task<CouponResult> ValidateCouponAsync(string code);
        decimal CalculateDiscountedTotal(decimal originalTotal, decimal discountPercentage);
    }

    public class CouponService : ICouponService
    {
        private readonly ApplicationDbContext _context;

        public CouponService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CouponResult> ValidateCouponAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return new CouponResult { IsValid = false, Message = "يرجى إدخال كود الخصم." };
            }

            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code.ToLower() == code.ToLower());

            if (coupon == null)
            {
                return new CouponResult { IsValid = false, Message = "كود الخصم غير صحيح أو غير موجود." };
            }

            if (!coupon.IsActive)
            {
                return new CouponResult { IsValid = false, Message = "كود الخصم هذا غير فعال حالياً." };
            }

            if (coupon.ExpiryDate < DateTime.Now)
            {
                return new CouponResult { IsValid = false, Message = "عذراً، كود الخصم منتهي الصلاحية." };
            }

            return new CouponResult
            {
                IsValid = true,
                Message = "تم تطبيق الخصم بنجاح!",
                DiscountPercentage = coupon.DiscountPercentage
            };
        }

        public decimal CalculateDiscountedTotal(decimal originalTotal, decimal discountPercentage)
        {
            if (discountPercentage <= 0 || discountPercentage > 100)
            {
                return originalTotal;
            }

            var discountAmount = originalTotal * (discountPercentage / 100m);
            return originalTotal - discountAmount;
        }
    }
}
