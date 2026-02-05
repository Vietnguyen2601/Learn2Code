using System.Net;
using System.Net.Mail;
using Learn2Code.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Learn2Code.Application.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var host = smtpSettings["Host"] ?? "smtp.gmail.com";
            var port = int.Parse(smtpSettings["Port"] ?? "587");
            var username = smtpSettings["Username"] ?? throw new InvalidOperationException("SMTP Username not configured");
            var password = smtpSettings["Password"] ?? throw new InvalidOperationException("SMTP Password not configured");
            var fromEmail = smtpSettings["FromEmail"] ?? username;
            var fromName = smtpSettings["FromName"] ?? "Learn2Code";

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(username, password)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent successfully to {Email}", to);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", to);
            return false;
        }
    }

    public async Task<bool> SendOtpEmailAsync(string to, string otpCode)
    {
        var subject = "Learn2Code - Email Verification OTP";
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #4F46E5; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background: #f9fafb; padding: 30px; border-radius: 0 0 8px 8px; }}
        .otp-code {{ font-size: 32px; font-weight: bold; color: #4F46E5; text-align: center; 
                     padding: 20px; background: white; border-radius: 8px; margin: 20px 0; 
                     letter-spacing: 8px; }}
        .footer {{ text-align: center; color: #666; font-size: 12px; margin-top: 20px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Learn2Code</h1>
        </div>
        <div class='content'>
            <h2>Email Verification</h2>
            <p>Hello,</p>
            <p>Your OTP code for email verification is:</p>
            <div class='otp-code'>{otpCode}</div>
            <p>This code will expire in <strong>5 minutes</strong>.</p>
            <p>If you did not request this code, please ignore this email.</p>
        </div>
        <div class='footer'>
            <p>&copy; 2026 Learn2Code. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

        return await SendEmailAsync(to, subject, body);
    }
}
