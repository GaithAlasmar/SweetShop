namespace SweetShop.Services.Reports;

/// <summary>
/// Aggregated data for a single nightly financial report.
/// Populated by FinancialReportService and passed to IReportEmailService.
/// </summary>
public class DailyFinancialReport
{
    /// <summary>The date this report covers (yesterday's orders).</summary>
    public DateOnly ReportDate { get; init; }

    /// <summary>Total number of orders placed.</summary>
    public int TotalOrders { get; init; }

    /// <summary>Sum of all OrderTotal values.</summary>
    public decimal GrossRevenue { get; init; }

    /// <summary>Average order value for the day.</summary>
    public decimal AverageOrderValue => TotalOrders > 0
        ? Math.Round(GrossRevenue / TotalOrders, 2)
        : 0;

    /// <summary>The single highest-value order of the day.</summary>
    public decimal TopOrderValue { get; init; }

    /// <summary>Top 5 best-selling products by units sold.</summary>
    public IReadOnlyList<ProductSalesSummary> TopProducts { get; init; } = [];

    /// <summary>Hour-by-hour order breakdown (0-23).</summary>
    public IReadOnlyList<HourlySummary> HourlyBreakdown { get; init; } = [];
}

public record ProductSalesSummary(
    string ProductName,
    int UnitsSold,
    decimal Revenue);

public record HourlySummary(
    int Hour,
    int OrderCount,
    decimal Revenue);
