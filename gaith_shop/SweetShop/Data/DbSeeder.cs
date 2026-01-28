using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using SweetShop.Models;


namespace SweetShop.Data;

public class DbSeeder
{
    public static void Seed(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
            var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

            if (context == null || userManager == null || roleManager == null) return;

            context.Database.Migrate();

            // Seed Roles

            if (!roleManager.RoleExistsAsync("Admin").Result)
                roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
                
            if (!roleManager.RoleExistsAsync("Customer").Result)
                roleManager.CreateAsync(new IdentityRole("Customer")).Wait();

            // Seed Admin
             if (userManager.FindByEmailAsync("admin@sweetshop.com").Result == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin@sweetshop.com",
                    Email = "admin@sweetshop.com",
                    EmailConfirmed = true,
                    FullName = "Admin User"
                };
                var result = userManager.CreateAsync(user, "Password123!").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }

            if (!context.Categories.Any())

            {
                context.Categories.AddRange(Categories.Select(c => c.Value));
                context.SaveChanges();
            }

            if (!context.Products.Any())
            {
                context.Products.AddRange(
                    new Product
                    {
                        Name = "Chocolate Cake",
                        Price = 12.95M,
                        Description = "Rich chocolate cake with fudge frosting.",
                        Category = Categories["Cakes"],
                        ImageUrl = "https://images.unsplash.com/photo-1578985545062-69928b1d9587?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60",
                        InStock = true,
                        IsPreferredSweet = true
                    },
                    new Product
                    {
                        Name = "Strawberry Cheesecake",
                        Price = 15.50M,
                        Description = "Classic cheesecake with fresh strawberries.",
                        Category = Categories["Cakes"],
                        ImageUrl = "https://images.unsplash.com/photo-1533134242443-d4fd215305ad?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60",
                        InStock = true,
                        IsPreferredSweet = true
                    },
                    new Product
                    {
                        Name = "Gummy Bears",
                        Price = 4.50M,
                        Description = "Assorted fruit flavored gummy bears.",
                        Category = Categories["Candies"],
                        ImageUrl = "https://images.unsplash.com/photo-1582058091505-f87a2e55a40f?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60",
                        InStock = true,
                        IsPreferredSweet = false
                    },
                    new Product
                    {
                        Name = "Chocolate Chip Cookies",
                        Price = 8.00M,
                        Description = "Homemade style chocolate chip cookies.",
                        Category = Categories["Cakes"], // Using 'Cakes' for baked goods for simplicity or create Bakery category
                        ImageUrl = "https://images.unsplash.com/photo-1499636138143-bd630f5cf38b?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60",
                        InStock = true,
                        IsPreferredSweet = true
                    }
                );
                context.SaveChanges();
            }
        }
    }

    private static Dictionary<string, Category>? _categories;

    public static Dictionary<string, Category> Categories
    {
        get
        {
            if (_categories == null)
            {
                var list = new Category[]
                {
                    new Category { Name = "Cakes", Description = "Delicious cakes" },
                    new Category { Name = "Candies", Description = "Sweet candies" }
                };

                _categories = new Dictionary<string, Category>();

                foreach (var genre in list)
                {
                    _categories.Add(genre.Name, genre);
                }
            }

            return _categories;
        }
    }
}
