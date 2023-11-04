using MailKit.Net.Smtp;
using MimeKit;

namespace WebApplication1.Services;

public class MailService
{
    private readonly SmtpClient _smtpClient = new SmtpClient();
    private readonly IConfiguration _configuration;
    
    public MailService(IConfiguration configuration)
    {
        _configuration = configuration;
        _smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
        _smtpClient.Connect(_configuration["MailerSettings:Host"], int.Parse(_configuration["MailerSettings:Port"]!), MailKit.Security.SecureSocketOptions.StartTls);
        _smtpClient.Authenticate(_configuration["MailerSettings:Username"], _configuration["MailerSettings:Password"]);
    }
    
    public async Task Send(string emailTo, string subject, BodyBuilder bodyBuilder)
    {
        var message = new MimeMessage();
        
        message.From.Add(new MailboxAddress(_configuration["MailerSettings:SenderName"], _configuration["MailerSettings:SenderEmail"]));
        message.To.Add(new MailboxAddress("", emailTo));
        
        message.Subject = subject;
        message.Body = bodyBuilder.ToMessageBody();
        
        await _smtpClient.SendAsync(message);
    }
}