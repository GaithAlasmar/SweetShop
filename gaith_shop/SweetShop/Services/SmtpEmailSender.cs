using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace SweetShop.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public SmtpEmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // 1. قراءة إعدادات الـ SMTP من ملف appsettings.json
        var smtpSettings = _configuration.GetSection("SmtpSettings");

        string host = smtpSettings["Host"] ?? string.Empty;
        int port = smtpSettings.GetValue<int>("Port");
        bool enableSsl = smtpSettings.GetValue<bool>("EnableSSL");
        string userName = smtpSettings["UserName"] ?? string.Empty;
        string password = smtpSettings["Password"] ?? string.Empty;

        // 2. محاكاة الإرسال (تجاوز الخطأ) أثناء التطوير إذا لم يتم تزويد إيميل حقيقي
        if (string.IsNullOrEmpty(host) || userName == "your_email@gmail.com" || host == "smtp.example.com")
        {
            Console.WriteLine($"[Mock Email] Sent to: {email}");
            Console.WriteLine($"[Mock Email] Subject: {subject}");
            Console.WriteLine($"[Mock Email] Body: {htmlMessage}");
            return;
        }

        // 3. الإعداد الفعلي لخادم البريد وتمرير بيانات الاعتماد
        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(userName, password),
            EnableSsl = enableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false
        };

        // 4. إعداد محتوى الرسالة
        using var mailMessage = new MailMessage
        {
            From = new MailAddress(userName, "حلويات الاسمر"),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        mailMessage.To.Add(email);

        // 5. عملية الإرسال
        await client.SendMailAsync(mailMessage);
    }
}
