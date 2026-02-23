using System.Threading.RateLimiting;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using SweetShop.Data;
using SweetShop.Middleware;
using SweetShop.Models;
using SweetShop.Models.Interfaces;
using SweetShop.Models.Repositories;
using SweetShop.Services;
using SweetShop.Services.Caching;
using SweetShop.Services.Reports;
using SweetShop.Validators;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

// ── Caching ────────────────────────────────────────────────────
// IMemoryCache is the in-process cache store (l2 for single-server).
// Swap to AddStackExchangeRedisCache() for multi-server deployments.
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

// ── Product Repository (Cache-Aside Decorator Pattern) ────────────
// 1. Register the real EF Core-backed repository as a named implementation.
// 2. Register IProductRepository as the Decorator (CachedProductRepository)
//    which wraps the real repo with cache logic.
// All consumers receive transparent caching — no handler/controller changes.
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<IProductRepository>(sp =>
    new CachedProductRepository(
        inner: sp.GetRequiredService<ProductRepository>(),
        cache: sp.GetRequiredService<ICacheService>(),
        logger: sp.GetRequiredService<ILogger<CachedProductRepository>>()));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ShoppingCart>(sp => ShoppingCart.GetCart(sp));

// Register MediatR – scans all handlers in this assembly
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    // MediatR pipeline: ValidationBehavior runs BEFORE every handler
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});
builder.Services.AddHttpContextAccessor();

// ── FluentValidation ────────────────────────────────────────────────────────
// Automatically scans and registers all AbstractValidator<T> in this assembly.
// Each validator is registered as IValidator<T> and injected by DI as needed.
builder.Services.AddValidatorsFromAssemblyContaining<RegisterViewModelValidator>();

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages();

// ── Nightly Financial Report Worker ───────────────────────────────────
// FinancialReportService + ReportEmailService are Scoped (per-request).
// NightlyReportWorker is Singleton (one instance for app lifetime).
// The worker creates its own DI scope to resolve Scoped dependencies safely.
builder.Services.AddScoped<FinancialReportService>();
builder.Services.AddScoped<IReportEmailService, ReportEmailService>();
builder.Services.AddHostedService<NightlyReportWorker>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ── Rate Limiting ──────────────────────────────────────────────────────
builder.Services.AddRateLimiter(options =>
{
    // ── Policy 1: LoginPolicy ──────────────────────────────────────────
    // Fixed Window: max 5 login attempts per minute, partitioned per IP.
    // Protects /Account/Login against credential-stuffing and brute-force.
    options.AddPolicy("LoginPolicy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0   // Reject immediately — no queuing
            }));

    // ── Policy 2: OrderPolicy ──────────────────────────────────────────
    // Sliding Window: max 10 orders per 5 minutes, partitioned per User ID.
    // Falls back to IP for unauthenticated requests.
    // Sliding window prevents burst abuse at window boundary.
    options.AddPolicy("OrderPolicy", context =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: context.User?.Identity?.IsAuthenticated == true
                ? context.User.Identity.Name ?? "auth-user"
                : context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(5),
                SegmentsPerWindow = 5,   // 1 segment per minute within the 5-min window
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    // ── Policy 3 (Named): OrderPolicy  — applied via [EnableRateLimiting] ────
    // Already defined above. See OrdersController.

    // ── GlobalApiPolicy — applied automatically to EVERY endpoint ─────────
    // Token Bucket: max 100 tokens/min per IP, auto-refilled every 60 s.
    // Acts as the last line of defence — no [EnableRateLimiting] needed.
    // Individual policies (LoginPolicy, OrderPolicy) are stricter and take
    // precedence on their own endpoints; this is the catch-all fallback.
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetTokenBucketLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new TokenBucketRateLimiterOptions
            {
                TokenLimit = 100,  // Max bucket capacity
                TokensPerPeriod = 100,  // Tokens refilled each period
                ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                AutoReplenishment = true,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0     // Reject excess immediately
            }));

    // ── Rejection Response — shared by ALL policies ────────────────────
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (ctx, cancellationToken) =>
    {
        ctx.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        ctx.HttpContext.Response.ContentType = "text/plain; charset=utf-8";

        // Add Retry-After header so the client knows when to retry
        if (ctx.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            ctx.HttpContext.Response.Headers.RetryAfter =
                ((int)retryAfter.TotalSeconds).ToString();
        }

        await ctx.HttpContext.Response.WriteAsync(
            "لقد تجاوزت الحد المسموح به من الطلبات. الرجاء المحاولة لاحقاً.",
            cancellationToken);
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// ── Security Headers ─────────────────────────────────────────────────
// Must be registered BEFORE UseStaticFiles and UseRouting so that
// every response — pages, assets, partials, and error pages — carries
// the hardened headers (CSP, HSTS, X-Frame-Options, etc.).
app.UseMiddleware<SecurityHeadersMiddleware>();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

// Rate limiting must come AFTER UseRouting (so endpoint metadata is available)
// and BEFORE UseAuthorization (so the User identity is populated for OrderPolicy).
app.UseRateLimiter();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

await DbSeeder.SeedAsync(app);

app.Run();

