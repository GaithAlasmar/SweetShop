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
        using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
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
                    Category = Categories["Cookies"],
                    ImageUrl = "https://images.unsplash.com/photo-1499636138143-bd630f5cf38b?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60",
                    InStock = true,
                    IsPreferredSweet = true
                },
                // 10 New Sweet Products
                new Product
                {
                    Name = "Vanilla Ice Cream",
                    Price = 6.50M,
                    Description = "Creamy vanilla ice cream made with real vanilla beans.",
                    Category = Categories["Ice Cream"],
                    ImageUrl = "https://images.unsplash.com/photo-1570197788417-0e82375c9371?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60",
                    InStock = true,
                    IsPreferredSweet = true
                },
                new Product
                {
                    Name = "Tiramisu",
                    Price = 14.00M,
                    Description = "Classic Italian dessert with coffee-soaked ladyfingers and mascarpone.",
                    Category = Categories["Cakes"],
                    ImageUrl = "https://images.unsplash.com/photo-1571877227200-a0d98ea607e9?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60",
                    InStock = true,
                    IsPreferredSweet = true
                },
                new Product
                {
                    Name = "Red Velvet Cake",
                    Price = 13.50M,
                    Description = "Moist red velvet cake with cream cheese frosting.",
                    Category = Categories["Cakes"],
                    ImageUrl = "https://images.unsplash.com/photo-1586985289688-ca3cf47d3e6e?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60",
                    InStock = true,
                    IsPreferredSweet = true
                },
                new Product
                {
                    Name = "Baklava",
                    Price = 10.00M,
                    Description = "Traditional Middle Eastern pastry with honey and pistachios.",
                    Category = Categories["Pastries"],
                    ImageUrl = "https://images.unsplash.com/photo-1519676867240-f03562e64548?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60",
                    InStock = true,
                    IsPreferredSweet = true
                },
                new Product
                {
                    Name = "Croissant",
                    Price = 5.00M,
                    Description = "Buttery, flaky French croissant.",
                    Category = Categories["Pastries"],
                    ImageUrl = "https://images.unsplash.com/photo-1555507036-ab1f4038808a?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60",
                    InStock = true,
                    IsPreferredSweet = false
                },
                new Product
                {
                    Name = "Macaron",
                    Price = 18.00M,
                    Description = "Elegant French macarons in assorted flavors.",
                    Category = Categories["Pastries"],
                    ImageUrl = "https://images.unsplash.com/photo-1569864358642-9d1684040f43?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60",
                    InStock = true,
                    IsPreferredSweet = true
                },
                new Product
                {
                    Name = "Lollipops",
                    Price = 3.50M,
                    Description = "Colorful fruit-flavored lollipops.",
                    Category = Categories["Candies"],
                    ImageUrl = "https://images.unsplash.com/photo-1514517220017-8ce97a34a7b6?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60",
                    InStock = true,
                    IsPreferredSweet = false
                },
                new Product
                {
                    Name = "Chocolate Truffles",
                    Price = 16.00M,
                    Description = "Premium handcrafted chocolate truffles.",
                    Category = Categories["Candies"],
                    ImageUrl = "https://images.unsplash.com/photo-1548848864-005a6b7f2e21?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60",
                    InStock = true,
                    IsPreferredSweet = true
                },
                new Product
                {
                    Name = "Brownies",
                    Price = 9.00M,
                    Description = "Fudgy chocolate brownies with walnuts.",
                    Category = Categories["Cookies"],
                    ImageUrl = "https://images.unsplash.com/photo-1607920591413-4ec007e70023?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60",
                    InStock = true,
                    IsPreferredSweet = true
                },
                new Product
                {
                    Name = "Cinnamon Rolls",
                    Price = 7.50M,
                    Description = "Warm cinnamon rolls with cream cheese icing.",
                    Category = Categories["Pastries"],
                    ImageUrl = "https://images.unsplash.com/photo-1509440159596-0249088772ff?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60",
                    InStock = true,
                    IsPreferredSweet = true
                },
                // 10 Additional Products without Images
                new Product
                {
                    Name = "Caramel Fudge",
                    Price = 11.00M,
                    Description = "Smooth and creamy caramel fudge with a buttery finish.",
                    Category = Categories["Candies"],
                    ImageUrl = "",
                    InStock = true,
                    IsPreferredSweet = false
                },
                new Product
                {
                    Name = "Lemon Tart",
                    Price = 9.50M,
                    Description = "Tangy lemon tart with a crispy pastry crust.",
                    Category = Categories["Pastries"],
                    ImageUrl = "",
                    InStock = true,
                    IsPreferredSweet = false
                },
                new Product
                {
                    Name = "Peanut Butter Cookies",
                    Price = 7.00M,
                    Description = "Classic peanut butter cookies with a soft center.",
                    Category = Categories["Cookies"],
                    ImageUrl = "",
                    InStock = true,
                    IsPreferredSweet = false
                },
                new Product
                {
                    Name = "Mint Chocolate Ice Cream",
                    Price = 7.50M,
                    Description = "Refreshing mint ice cream with chocolate chips.",
                    Category = Categories["Ice Cream"],
                    ImageUrl = "",
                    InStock = true,
                    IsPreferredSweet = false
                },
                new Product
                {
                    Name = "Carrot Cake",
                    Price = 12.00M,
                    Description = "Moist carrot cake with cream cheese frosting and walnuts.",
                    Category = Categories["Cakes"],
                    ImageUrl = "",
                    InStock = true,
                    IsPreferredSweet = false
                },
                new Product
                {
                    Name = "Cotton Candy",
                    Price = 4.00M,
                    Description = "Light and fluffy cotton candy in various flavors.",
                    Category = Categories["Candies"],
                    ImageUrl = "",
                    InStock = true,
                    IsPreferredSweet = false
                },
                new Product
                {
                    Name = "Oatmeal Raisin Cookies",
                    Price = 6.50M,
                    Description = "Hearty oatmeal cookies with sweet raisins.",
                    Category = Categories["Cookies"],
                    ImageUrl = "",
                    InStock = true,
                    IsPreferredSweet = false
                },
                new Product
                {
                    Name = "Pistachio Ice Cream",
                    Price = 8.00M,
                    Description = "Rich pistachio ice cream made with real nuts.",
                    Category = Categories["Ice Cream"],
                    ImageUrl = "",
                    InStock = true,
                    IsPreferredSweet = false
                },
                new Product
                {
                    Name = "Apple Pie",
                    Price = 10.50M,
                    Description = "Traditional apple pie with cinnamon and a flaky crust.",
                    Category = Categories["Pastries"],
                    ImageUrl = "",
                    InStock = true,
                    IsPreferredSweet = false
                },
                new Product
                {
                    Name = "Salted Caramels",
                    Price = 12.50M,
                    Description = "Gourmet salted caramels with a perfect sweet-savory balance.",
                    Category = Categories["Candies"],
                    ImageUrl = "",
                    InStock = true,
                    IsPreferredSweet = false
                }
            );
            context.SaveChanges();
        }

        if (!context.Products.Any(p => p.Name == "Blueberry Muffin"))
        {
            context.Products.AddRange(
                new Product
                {
                    Name = "Blueberry Muffin",
                    Price = 5.50M,
                    Description = "Freshly baked muffin bursting with blueberries.",
                    Category = Categories["Pastries"],
                    ImageUrl = "https://images.unsplash.com/photo-1555507036-ab1f4038808a?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60",
                    InStock = true,
                    IsPreferredSweet = true
                },
                new Product
                {
                    Name = "Berry Tart",
                    Price = 11.00M,
                    Description = "Crispy tart shell filled with custard and topped with mixed berries.",
                    Category = Categories["Pastries"],
                    ImageUrl = "https://images.unsplash.com/photo-1533134242443-d4fd215305ad?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60",
                    InStock = true,
                    IsPreferredSweet = true
                },
                new Product
                {
                    Name = "Dark Chocolate Bar",
                    Price = 6.00M,
                    Description = "Rich 70% dark chocolate bar.",
                    Category = Categories["Candies"],
                    ImageUrl = "https://images.unsplash.com/photo-1548848864-005a6b7f2e21?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60",
                    InStock = true,
                    IsPreferredSweet = true
                },
                new Product
                {
                    Name = "Cookies and Cream Cake",
                    Price = 14.50M,
                    Description = "Layers of chocolate cake with cookies and cream filling.",
                    Category = Categories["Cakes"],
                    ImageUrl = "https://images.unsplash.com/photo-1578985545062-69928b1d9587?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60",
                    InStock = true,
                    IsPreferredSweet = true
                },
                new Product
                {
                    Name = "Glazed Donut",
                    Price = 3.50M,
                    Description = "Classic fluffy donut with a sweet glaze.",
                    Category = Categories["Pastries"],
                    ImageUrl = "https://images.unsplash.com/photo-1509440159596-0249088772ff?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60",
                    InStock = true,
                    IsPreferredSweet = true
                }
            );
            context.SaveChanges();
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
                    new Category { Name = "Candies", Description = "Sweet candies" },
                    new Category { Name = "Ice Cream", Description = "Frozen desserts" },
                    new Category { Name = "Pastries", Description = "Baked pastries" },
                    new Category { Name = "Cookies", Description = "Delightful cookies" }
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
