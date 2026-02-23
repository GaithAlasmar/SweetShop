namespace SweetShop.Services.Reports;

/// <summary>
/// ASP.NET Core BackgroundService that fires the financial report pipeline
/// every night at a configurable time (default: 02:00 AM local time).
///
/// Key design decisions:
///  • BackgroundService (not IHostedService) — cleaner lifecycle hooks.
///  • IServiceScopeFactory for resolving Scoped services (EF Core DbContext)
///    from a Singleton-lifetime hosted service.
///  • Graceful shutdown via CancellationToken — never blocks SIGTERM.
///  • Retry logic with exponential back-off: 1 min → 5 min → 15 min.
///  • Full structured logging at every stage.
/// </summary>
public class NightlyReportWorker(
    IServiceScopeFactory scopeFactory,
    IConfiguration config,
    ILogger<NightlyReportWorker> logger)
    : BackgroundService
{
    // ── Configuration ──────────────────────────────────────────────────
    // Default: run at 02:00 AM every night.
    // Override in appsettings.json → "ReportWorker:RunAtHour": 3
    private TimeSpan RunAt => TimeSpan.FromHours(
        config.GetValue("ReportWorker:RunAtHour", 2));

    private static readonly TimeSpan[] RetryDelays =
    [
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(5),
        TimeSpan.FromMinutes(15)
    ];

    // ── Entry Point ────────────────────────────────────────────────────
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "[ReportWorker] Started. Scheduled to run daily at {Hour:00}:00.",
            RunAt.Hours);

        while (!stoppingToken.IsCancellationRequested)
        {
            // ── Calculate delay until next scheduled run ───────────────
            var delay = CalculateDelayUntilNextRun();

            logger.LogInformation(
                "[ReportWorker] Next report run in {Delay:hh\\:mm\\:ss} at {Time:HH:mm}.",
                delay, DateTime.Now.Add(delay));

            try
            {
                // Wait until scheduled time — wakes up immediately on shutdown
                await Task.Delay(delay, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Host is shutting down — exit cleanly
                logger.LogInformation("[ReportWorker] Shutdown requested while waiting. Exiting.");
                break;
            }

            // ── Run the report with retry logic ───────────────────────
            if (!stoppingToken.IsCancellationRequested)
                await RunReportWithRetryAsync(stoppingToken);
        }

        logger.LogInformation("[ReportWorker] Stopped.");
    }

    // ── Retry Wrapper ──────────────────────────────────────────────────
    private async Task RunReportWithRetryAsync(CancellationToken ct)
    {
        var reportDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)); // Yesterday

        for (int attempt = 1; attempt <= RetryDelays.Length + 1; attempt++)
        {
            try
            {
                logger.LogInformation(
                    "[ReportWorker] Attempt {Attempt} — Generating report for {Date}.",
                    attempt, reportDate);

                await GenerateAndSendReportAsync(reportDate, ct);

                logger.LogInformation(
                    "[ReportWorker] ✅ Report for {Date} completed successfully on attempt {Attempt}.",
                    reportDate, attempt);

                return; // Success — exit retry loop
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("[ReportWorker] Report cancelled (shutdown). Stopping retries.");
                return;
            }
            catch (Exception ex) when (attempt <= RetryDelays.Length)
            {
                var retryDelay = RetryDelays[attempt - 1];

                logger.LogError(ex,
                    "[ReportWorker] ❌ Attempt {Attempt} failed. Retrying in {Delay}.",
                    attempt, retryDelay);

                try { await Task.Delay(retryDelay, ct); }
                catch (OperationCanceledException) { return; }
            }
            catch (Exception ex)
            {
                // All retries exhausted
                logger.LogCritical(ex,
                    "[ReportWorker] 🚨 All {MaxAttempts} attempts failed for report {Date}. " +
                    "Manual intervention required.",
                    RetryDelays.Length + 1, reportDate);
            }
        }
    }

    // ── Core Pipeline ──────────────────────────────────────────────────
    private async Task GenerateAndSendReportAsync(DateOnly reportDate, CancellationToken ct)
    {
        // Create a new DI scope — required to resolve Scoped services
        // (ApplicationDbContext) from this Singleton-lifetime hosted service
        await using var scope = scopeFactory.CreateAsyncScope();

        var reportService = scope.ServiceProvider.GetRequiredService<FinancialReportService>();
        var emailService = scope.ServiceProvider.GetRequiredService<IReportEmailService>();

        // ── Step 1: Build report from DB ──────────────────────────────
        logger.LogDebug("[ReportWorker] Querying orders for {Date}...", reportDate);
        var report = await reportService.BuildReportAsync(reportDate, ct);

        logger.LogInformation(
            "[ReportWorker] Report built: {Orders} orders, Revenue={Revenue:C2}",
            report.TotalOrders, report.GrossRevenue);

        // ── Step 2: Send email ─────────────────────────────────────────
        logger.LogDebug("[ReportWorker] Sending report email...");
        await emailService.SendReportAsync(report, ct);
    }

    // ── Scheduling Helper ──────────────────────────────────────────────
    /// <summary>
    /// Calculates the precise TimeSpan to wait until the next scheduled run.
    /// If the scheduled time has already passed today, targets tomorrow.
    /// </summary>
    private TimeSpan CalculateDelayUntilNextRun()
    {
        var now = DateTime.Now;
        var todayRun = DateTime.Today.Add(RunAt);
        var nextRun = now < todayRun ? todayRun : todayRun.AddDays(1);
        return nextRun - now;
    }

    // ── Graceful Shutdown ──────────────────────────────────────────────
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("[ReportWorker] StopAsync called — finishing gracefully.");
        await base.StopAsync(cancellationToken);
        logger.LogInformation("[ReportWorker] Stopped cleanly.");
    }
}
