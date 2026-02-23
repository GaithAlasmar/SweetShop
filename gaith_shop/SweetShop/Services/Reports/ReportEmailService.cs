using System.Net;
using System.Net.Mail;
using System.Text;

namespace SweetShop.Services.Reports;

public interface IReportEmailService
{
    Task SendReportAsync(DailyFinancialReport report, CancellationToken ct);
}

/// <summary>
/// Sends the nightly financial report via SMTP.
/// Uses SmtpClient (built-in, no extra packages).
/// For production, replace with SendGrid / Mailgun for reliability.
/// </summary>
public class ReportEmailService(
    IConfiguration config,
    ILogger<ReportEmailService> logger)
    : IReportEmailService
{
    public async Task SendReportAsync(DailyFinancialReport report, CancellationToken ct)
    {
        var smtpSection = config.GetSection("Email:Smtp");
        var host = smtpSection["Host"] ?? throw new InvalidOperationException("Email:Smtp:Host not configured");
        var port = int.Parse(smtpSection["Port"] ?? "587");
        var user = smtpSection["Username"] ?? throw new InvalidOperationException("Email:Smtp:Username not configured");
        var pass = smtpSection["Password"] ?? throw new InvalidOperationException("Email:Smtp:Password not configured");
        var from = smtpSection["From"] ?? user;
        var recipients = config.GetSection("Email:ReportRecipients").Get<string[]>()
                          ?? throw new InvalidOperationException("Email:ReportRecipients not configured");

        var subject = $"📊 SweetShop Daily Report — {report.ReportDate:dd MMM yyyy}";
        var body = BuildHtmlBody(report);

        using var message = new MailMessage
        {
            From = new MailAddress(from, "SweetShop Reports"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        foreach (var recipient in recipients)
            message.To.Add(recipient);

        using var smtp = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(user, pass),
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Timeout = 30_000 // 30 s
        };

        logger.LogInformation(
            "[ReportEmail] Sending report for {Date} to {Count} recipient(s)",
            report.ReportDate, recipients.Length);

        // SmtpClient doesn't natively support CancellationToken — wrap it
        await Task.Run(() => smtp.Send(message), ct);

        logger.LogInformation(
            "[ReportEmail] Report sent successfully for {Date}", report.ReportDate);
    }

    // ── HTML Report Builder ────────────────────────────────────────────
    private static string BuildHtmlBody(DailyFinancialReport r)
    {
        var sb = new StringBuilder();

        // $$""" — double-dollar raw string: {{ }} are literal braces, {expr} is interpolation
        sb.Append($$"""
            <!DOCTYPE html>
            <html dir="ltr" lang="en">
            <head>
              <meta charset="UTF-8">
              <style>
                body    { font-family: Arial, sans-serif; color: #333; background: #f4f4f4; }
                .card   { background: #fff; border-radius: 8px; padding: 20px; margin: 12px 0; box-shadow: 0 2px 4px rgba(0,0,0,.08); }
                h1      { color: #1A8CB0; }
                h2      { color: #C7AE6A; border-bottom: 2px solid #C7AE6A; padding-bottom: 4px; }
                table   { width: 100%; border-collapse: collapse; }
                th      { background: #1A8CB0; color: #fff; padding: 8px 12px; text-align: left; }
                td      { padding: 8px 12px; border-bottom: 1px solid #eee; }
                .metric { font-size: 2em; font-weight: bold; color: #1A8CB0; }
                .label  { color: #888; font-size: .9em; }
              </style>
            </head>
            <body>
              <div style="max-width:700px; margin:auto; padding:24px;">
                <h1>🍬 SweetShop Financial Report</h1>
                <p style="color:#888;">{{r.ReportDate:dddd, dd MMMM yyyy}}</p>

                <!-- KPIs -->
                <div class="card">
                  <h2>Daily Summary</h2>
                  <table>
                    <tr>
                      <td><span class="metric">{{r.TotalOrders:N0}}</span><br><span class="label">Total Orders</span></td>
                      <td><span class="metric">{{r.GrossRevenue:C2}}</span><br><span class="label">Gross Revenue</span></td>
                      <td><span class="metric">{{r.AverageOrderValue:C2}}</span><br><span class="label">Avg Order Value</span></td>
                      <td><span class="metric">{{r.TopOrderValue:C2}}</span><br><span class="label">Top Order</span></td>
                    </tr>
                  </table>
                </div>
            """);

        // Top products table
        if (r.TopProducts.Count > 0)
        {
            sb.Append("""
                <div class="card">
                  <h2>🏆 Top Products</h2>
                  <table>
                    <tr><th>Product</th><th>Units Sold</th><th>Revenue</th></tr>
                """);
            foreach (var p in r.TopProducts)
                sb.Append($"<tr><td>{p.ProductName}</td><td>{p.UnitsSold:N0}</td><td>{p.Revenue:C2}</td></tr>");
            sb.Append("</table></div>");
        }

        // Hourly breakdown table
        if (r.HourlyBreakdown.Count > 0)
        {
            sb.Append("""
                <div class="card">
                  <h2>⏰ Orders by Hour</h2>
                  <table>
                    <tr><th>Hour</th><th>Orders</th><th>Revenue</th></tr>
                """);
            foreach (var h in r.HourlyBreakdown)
                sb.Append($"<tr><td>{h.Hour:00}:00</td><td>{h.OrderCount:N0}</td><td>{h.Revenue:C2}</td></tr>");
            sb.Append("</table></div>");
        }

        // No orders fallback
        if (r.TotalOrders == 0)
        {
            sb.Append("""
                <div class="card" style="text-align:center; color:#888;">
                  <p>😴 No orders were placed yesterday.</p>
                </div>
                """);
        }

        sb.Append("""
              <p style="color:#bbb; font-size:.8em; text-align:center; margin-top:32px;">
                Auto-generated by SweetShop · Do not reply
              </p>
              </div>
            </body>
            </html>
            """);

        return sb.ToString();
    }
}
