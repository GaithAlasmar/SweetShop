using Microsoft.EntityFrameworkCore;
using SweetShop.Data;

namespace SweetShop.Services.Reports;

/// <summary>
/// Queries the database and builds a DailyFinancialReport for a given date.
/// Registered as Scoped and resolved inside a DI scope created by the worker.
/// </summary>
public class FinancialReportService(ApplicationDbContext context)
{
    /// <summary>
    /// Aggregates all orders placed on <paramref name="reportDate"/> into
    /// a structured DailyFinancialReport object.
    /// </summary>
    public async Task<DailyFinancialReport> BuildReportAsync(
        DateOnly reportDate,
        CancellationToken ct)
    {
        var start = reportDate.ToDateTime(TimeOnly.MinValue);   // 00:00:00
        var end = reportDate.ToDateTime(TimeOnly.MaxValue);   // 23:59:59

        // Single DB query — load all orders for the day with their details
        var orders = await context.Orders
            .Where(o => o.OrderPlaced >= start && o.OrderPlaced <= end)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .AsNoTracking()
            .ToListAsync(ct);

        if (orders.Count == 0)
        {
            return new DailyFinancialReport
            {
                ReportDate = reportDate,
                TotalOrders = 0,
                GrossRevenue = 0
            };
        }

        // ── Top products by units sold ─────────────────────────────────
        var topProducts = orders
            .SelectMany(o => o.OrderDetails)
            .GroupBy(od => od.Product.Name)
            .Select(g => new ProductSalesSummary(
                ProductName: g.Key,
                UnitsSold: g.Sum(od => od.Amount),
                Revenue: g.Sum(od => od.Amount * od.Price)))
            .OrderByDescending(p => p.UnitsSold)
            .Take(5)
            .ToList();

        // ── Hour-by-hour breakdown ─────────────────────────────────────
        var hourlyBreakdown = orders
            .GroupBy(o => o.OrderPlaced.Hour)
            .Select(g => new HourlySummary(
                Hour: g.Key,
                OrderCount: g.Count(),
                Revenue: g.Sum(o => o.OrderTotal)))
            .OrderBy(h => h.Hour)
            .ToList();

        return new DailyFinancialReport
        {
            ReportDate = reportDate,
            TotalOrders = orders.Count,
            GrossRevenue = orders.Sum(o => o.OrderTotal),
            TopOrderValue = orders.Max(o => o.OrderTotal),
            TopProducts = topProducts,
            HourlyBreakdown = hourlyBreakdown
        };
    }
}
