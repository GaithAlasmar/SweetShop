using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using SweetShop.Models;


namespace SweetShop.Data;

public class DbSeeder
{
    public static async Task SeedAsync(IApplicationBuilder applicationBuilder)
    {
        using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
        var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
        var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

        if (context == null || userManager == null || roleManager == null) return;

        await context.Database.MigrateAsync();

        // Seed Roles
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        if (!await roleManager.RoleExistsAsync("Manager"))
            await roleManager.CreateAsync(new IdentityRole("Manager"));

        if (!await roleManager.RoleExistsAsync("Customer"))
            await roleManager.CreateAsync(new IdentityRole("Customer"));

        // Seed Admin
        if (await userManager.FindByEmailAsync("admin@sweetshop.com") == null)
        {
            var user = new ApplicationUser
            {
                UserName = "admin@sweetshop.com",
                Email = "admin@sweetshop.com",
                EmailConfirmed = true,
                FullName = "Admin User"
            };
            var result = await userManager.CreateAsync(user, "Password123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }

        // Seed Categories - التصنيفات الستة فقط
        foreach (var category in Categories.Values)
        {
            if (!context.Categories.Any(c => c.Name == category.Name))
            {
                context.Categories.Add(category);
            }
        }
        await context.SaveChangesAsync();

        // --- 1. حلويات عربية (للبقلاوة والمعمول) ---
        var arabicSweetsCategory = Categories["حلويات عربية"];

        var arabicSweetsToSeed = new[]
        {
            new { Name = "بقلاوة بالفستق", Price = 14.00M, Description = "بقلاوة طازجة محشوة بالفستق الحلبي", ImageUrl = "/images/ArabicSweets.jpeg" },
            new { Name = "بقلاوة بالجوز", Price = 13.00M, Description = "بقلاوة مقرمشة محشوة بالجوز", ImageUrl = "/images/ArabicSweets2.jpeg" },
            new { Name = "معمول بالتمر", Price = 10.00M, Description = "معمول طازج محشو بالتمر الفاخر", ImageUrl = "/images/ArabicSweets3.jpeg" },
            new { Name = "معمول بالجوز", Price = 11.00M, Description = "معمول طازج محشو بالجوز المحمص", ImageUrl = "/images/ArabicSweets1.jpeg" },
            new { Name = "Baklava", Price = 10.00M, Description = "Traditional Middle Eastern pastry with honey and pistachios", ImageUrl = "https://images.unsplash.com/photo-1519676867240-f03562e64548?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60" }
        };

        foreach (var item in arabicSweetsToSeed)
        {
            if (!context.Products.Any(p => p.Name == item.Name))
            {
                context.Products.Add(new Product
                {
                    Name = item.Name,
                    Price = item.Price,
                    Description = item.Description,
                    Category = arabicSweetsCategory,
                    ImageUrl = item.ImageUrl,
                    InStock = true,
                    IsPreferredSweet = true
                });
            }
        }
        await context.SaveChangesAsync();

        // --- 2. كنافة (لجميع أنواع الكنافة) ---
        var kunafaCategory = Categories["كنافة"];

        var kunafaToSeed = new[]
        {
            new { Name = "كنافة ناعمة", Price = 15.00M, Description = "كنافة ناعمة طازجة بالجبنة والقطر", ImageUrl = "/images/Knafha1.jpeg" },
            new { Name = "كنافة خشنة", Price = 15.00M, Description = "كنافة خشنة مقرمشة بالجبنة الطازجة", ImageUrl = "/images/Knafha4.jpeg" },
            new { Name = "كنافة مكس", Price = 16.00M, Description = "مزيج لذيذ من الكنافة الناعمة والخشنة", ImageUrl = "/images/Knafha6.jpeg" },
            new { Name = "كنافة وبوظة", Price = 18.00M, Description = "كنافة ساخنة مع بوظة باردة", ImageUrl = "/images/Knafha2.jpeg" },
            new { Name = "وربات كنافة", Price = 12.00M, Description = "وربات محشوة بالقشطة أو الجبنة", ImageUrl = "/images/Asab3.jpeg" },
            new { Name = "فطاير مثلثه", Price = 10.00M, Description = "فطاير مثلثة محشوة بالجبنة", ImageUrl = "/images/Fataer.jpeg" },
            new { Name = "مخدات", Price = 8.00M, Description = "معجنات على شكل مخدات محشوة بالجبنة", ImageUrl = "/images/Fataer1.jpeg" }
        };

        foreach (var item in kunafaToSeed)
        {
            if (!context.Products.Any(p => p.Name == item.Name))
            {
                context.Products.Add(new Product
                {
                    Name = item.Name,
                    Price = item.Price,
                    Description = item.Description,
                    Category = kunafaCategory,
                    ImageUrl = item.ImageUrl,
                    InStock = true,
                    IsPreferredSweet = true
                });
            }
        }
        await context.SaveChangesAsync();

        // --- 3. الحلويات الغربية (للكيك، التشيز كيك، الدونات، الكرواسون، البراونيز، والمافن) ---
        var westernSweetsCategory = Categories["الحلويات الغربية"];

        var westernSweetsToSeed = new[]
        {
            // منتجات عربية
            new { Name = "ليزي كيك", Price = 3.50M, Description = "كيكة البسكويت بالشوكولاتة اللذيذة", ImageUrl = "/images/Laizy.jpeg" },
            new { Name = "مولتن كيك", Price = 4.50M, Description = "كيكة الشوكولاتة الدافئة بقلب غني يذوب عند أول قضمة", ImageUrl = "/images/Molten.jpeg" },
            new { Name = "تيراميسو", Price = 5.00M, Description = "الحلوى الإيطالية الفاخرة بطبقات البسكويت المشبعة بالقهوة", ImageUrl = "/images/Tiramisu.jpeg" },
            new { Name = "ميل فوي", Price = 3.00M, Description = "رقائق الهشاشة الفرنسية التقليدية مع طبقات الكاسترد", ImageUrl = "/images/Mille-feuille.jpeg" },
            // كيك
            new { Name = "Chocolate Cake", Price = 12.95M, Description = "كيكة الشوكولاتة الغنية مع كريمة الفدج", ImageUrl = "https://images.unsplash.com/photo-1578985545062-69928b1d9587?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60" },
            new { Name = "Strawberry Cheesecake", Price = 15.50M, Description = "تشيز كيك كلاسيكي مع الفراولة الطازجة", ImageUrl = "https://images.unsplash.com/photo-1533134242443-d4fd215305ad?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60" },
            new { Name = "Red Velvet Cake", Price = 13.50M, Description = "كيكة ريد فيلفيت رطبة مع كريمة الجبن", ImageUrl = "https://images.unsplash.com/photo-1586985289688-ca3cf47d3e6e?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60" },
            new { Name = "Carrot Cake", Price = 12.00M, Description = "كيكة الجزر الرطبة مع كريمة الجبن والجوز", ImageUrl = "" },
            new { Name = "Cookies and Cream Cake", Price = 14.50M, Description = "طبقات من كيك الشوكولاتة مع حشوة الأوريو", ImageUrl = "https://images.unsplash.com/photo-1578985545062-69928b1d9587?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60" },
            new { Name = "Tiramisu", Price = 14.00M, Description = "الحلوى الإيطالية الكلاسيكية", ImageUrl = "https://images.unsplash.com/photo-1571877227200-a0d98ea607e9?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60" },
            // دونات وكرواسون
            new { Name = "Glazed Donut", Price = 3.50M, Description = "دونات كلاسيكي طازج مع طبقة سكر لامعة", ImageUrl = "https://images.unsplash.com/photo-1509440159596-0249088772ff?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60" },
            new { Name = "Croissant", Price = 5.00M, Description = "كرواسون فرنسي مخبوز بالزبدة", ImageUrl = "https://images.unsplash.com/photo-1555507036-ab1f4038808a?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60" },
            // براونيز وكوكيز
            new { Name = "Brownies", Price = 9.00M, Description = "براونيز الشوكولاتة الغنية مع الجوز", ImageUrl = "https://images.unsplash.com/photo-1607920591413-4ec007e70023?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60" },
            new { Name = "Chocolate Chip Cookies", Price = 8.00M, Description = "كوكيز الشوكولاتة المنزلي", ImageUrl = "https://images.unsplash.com/photo-1499636138143-bd630f5cf38b?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60" },
            new { Name = "Peanut Butter Cookies", Price = 7.00M, Description = "كوكيز زبدة الفول السوداني الكلاسيكي", ImageUrl = "" },
            new { Name = "Oatmeal Raisin Cookies", Price = 6.50M, Description = "كوكيز الشوفان بالزبيب", ImageUrl = "" },
            // مافن ومعجنات
            new { Name = "Blueberry Muffin", Price = 5.50M, Description = "مافن طازج محشو بالتوت الأزرق", ImageUrl = "https://images.unsplash.com/photo-1555507036-ab1f4038808a?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60" },
            new { Name = "Cinnamon Rolls", Price = 7.50M, Description = "رول القرفة الدافئ مع كريمة الجبن", ImageUrl = "https://images.unsplash.com/photo-1509440159596-0249088772ff?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60" }
        };

        foreach (var item in westernSweetsToSeed)
        {
            if (!context.Products.Any(p => p.Name == item.Name))
            {
                context.Products.Add(new Product
                {
                    Name = item.Name,
                    Price = item.Price,
                    Description = item.Description,
                    Category = westernSweetsCategory,
                    ImageUrl = item.ImageUrl,
                    InStock = true,
                    IsPreferredSweet = true
                });
            }
        }
        await context.SaveChangesAsync();

        // --- 4. الشوكلاتة والهدايا (للسكاكر، ألواح الشوكولاتة، الترافل، وغزل البنات) ---
        var chocolateGiftsCategory = Categories["الشوكلاتة والهدايا"];

        var chocolateGiftsToSeed = new[]
        {
            new { Name = "Chocolate Truffles", Price = 16.00M, Description = "ترافل الشوكولاتة المصنوع يدوياً", ImageUrl = "https://images.unsplash.com/photo-1548848864-005a6b7f2e21?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60" },
            new { Name = "Dark Chocolate Bar", Price = 6.00M, Description = "لوح شوكولاتة داكنة 70%", ImageUrl = "https://images.unsplash.com/photo-1548848864-005a6b7f2e21?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60" },
            new { Name = "Gummy Bears", Price = 4.50M, Description = "سكاكر جيلي بنكهات الفواكه المتنوعة", ImageUrl = "https://images.unsplash.com/photo-1582058091505-f87a2e55a40f?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60" },
            new { Name = "Lollipops", Price = 3.50M, Description = "مصاصات ملونة بنكهات الفواكه", ImageUrl = "https://images.unsplash.com/photo-1514517220017-8ce97a34a7b6?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60" },
            new { Name = "Cotton Candy", Price = 4.00M, Description = "غزل البنات الخفيف بنكهات متنوعة", ImageUrl = "" },
            new { Name = "Caramel Fudge", Price = 11.00M, Description = "فدج الكراميل الناعم والكريمي", ImageUrl = "" },
            new { Name = "Salted Caramels", Price = 12.50M, Description = "كراميل فاخر بالملح المتوازن", ImageUrl = "" }
        };

        foreach (var item in chocolateGiftsToSeed)
        {
            if (!context.Products.Any(p => p.Name == item.Name))
            {
                context.Products.Add(new Product
                {
                    Name = item.Name,
                    Price = item.Price,
                    Description = item.Description,
                    Category = chocolateGiftsCategory,
                    ImageUrl = item.ImageUrl,
                    InStock = true,
                    IsPreferredSweet = false
                });
            }
        }
        await context.SaveChangesAsync();

        // --- 5. عبوات خاصة (للمنتجات الفاخرة) ---
        var specialPackagesCategory = Categories["عبوات خاصة"];

        var specialPackagesToSeed = new[]
        {
            // المنتجات العربية الفاخرة
            new { Name = "صندوق الجواهر الشرقية", Price = 15.00M, Description = "تشكيلة فاخرة من أرقى الحلويات العربية", ImageUrl = "/images/ArabicSweets.jpeg" },
            new { Name = "باقة الفستق العاشق", Price = 18.00M, Description = "حلويات عربية فاخرة محشوة بالفستق الحلبي", ImageUrl = "/images/ArabicSweets1.jpeg" },
            new { Name = "صينية السلطان للضيافة", Price = 12.50M, Description = "مجموعة مميزة من الحلويات العربية للمناسبات", ImageUrl = "/images/ArabicSweets2.jpeg" },
            new { Name = "تشكيلة التراث الشامي", Price = 20.00M, Description = "أرقى أنواع الحلويات الشامية التقليدية", ImageUrl = "/images/ArabicSweets3.jpeg" },
            // منتجات غربية فاخرة
            new { Name = "Macaron", Price = 18.00M, Description = "ماكارون فرنسي أنيق بنكهات متنوعة", ImageUrl = "https://images.unsplash.com/photo-1569864358642-9d1684040f43?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60" },
            new { Name = "Berry Tart", Price = 11.00M, Description = "تارت هش محشو بالكاسترد ومغطى بالتوت المشكل", ImageUrl = "https://images.unsplash.com/photo-1533134242443-d4fd215305ad?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60" },
            new { Name = "Lemon Tart", Price = 9.50M, Description = "تارت الليمون اللاذع مع قشرة مقرمشة", ImageUrl = "" }
        };

        foreach (var item in specialPackagesToSeed)
        {
            if (!context.Products.Any(p => p.Name == item.Name))
            {
                context.Products.Add(new Product
                {
                    Name = item.Name,
                    Price = item.Price,
                    Description = item.Description,
                    Category = specialPackagesCategory,
                    ImageUrl = item.ImageUrl,
                    InStock = true,
                    IsPreferredSweet = true
                });
            }
        }
        await context.SaveChangesAsync();

        // --- 6. بوظة (لجميع أنواع الآيس كريم) ---
        var iceCreamCategory = Categories["بوظة"];

        var iceCreamToSeed = new[]
        {
            new { Name = "Vanilla Ice Cream", Price = 6.50M, Description = "بوظة فانيليا كريمية مع حبوب الفانيليا الطبيعية", ImageUrl = "https://images.unsplash.com/photo-1570197788417-0e82375c9371?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60" },
            new { Name = "Mint Chocolate Ice Cream", Price = 7.50M, Description = "بوظة النعناع المنعشة مع رقائق الشوكولاتة", ImageUrl = "" },
            new { Name = "Pistachio Ice Cream", Price = 8.00M, Description = "بوظة الفستق الغنية المصنوعة من المكسرات الطبيعية", ImageUrl = "" }
        };

        foreach (var item in iceCreamToSeed)
        {
            if (!context.Products.Any(p => p.Name == item.Name))
            {
                context.Products.Add(new Product
                {
                    Name = item.Name,
                    Price = item.Price,
                    Description = item.Description,
                    Category = iceCreamCategory,
                    ImageUrl = item.ImageUrl,
                    InStock = true,
                    IsPreferredSweet = true
                });
            }
        }
        await context.SaveChangesAsync();

        // تحديث حالة IsPreferredSweet للمنتجات حسب التصنيف
        var allProducts = await context.Products.Include(p => p.Category).ToListAsync();
        foreach (var product in allProducts)
        {
            if (product.Category != null)
            {
                // جميع المنتجات في التصنيفات الجديدة تكون مفضلة ماعدا "الشوكلاتة والهدايا"
                if (product.Category.Name == "حلويات عربية" ||
                    product.Category.Name == "كنافة" ||
                    product.Category.Name == "الحلويات الغربية" ||
                    product.Category.Name == "عبوات خاصة" ||
                    product.Category.Name == "بوظة")
                {
                    product.IsPreferredSweet = true;
                }
                else if (product.Category.Name == "الشوكلاتة والهدايا")
                {
                    product.IsPreferredSweet = false;
                }
            }
        }
        await context.SaveChangesAsync();
    }

    private static Dictionary<string, Category>? _categories;

    public static Dictionary<string, Category> Categories
    {
        get
        {
            _categories ??= new Category[]
            {
                new() { Name = "حلويات عربية", Description = "بقلاوة ومعمول وحلويات عربية تقليدية" },
                new() { Name = "كنافة", Description = "جميع أنواع الكنافة الأصيلة" },
                new() { Name = "الحلويات الغربية", Description = "كيك، تشيز كيك، دونات، كرواسون، براونيز، ومافن" },
                new() { Name = "الشوكلاتة والهدايا", Description = "سكاكر، ألواح شوكولاتة، ترافل، وغزل البنات" },
                new() { Name = "عبوات خاصة", Description = "منتجات فاخرة ومميزة للمناسبات" },
                new() { Name = "بوظة", Description = "جميع أنواع الآيس كريم" }
            }.ToDictionary(c => c.Name);

            return _categories;
        }
    }
}
