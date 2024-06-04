/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
#nullable disable

using System.Net;
using System.Net.Mail;

using WebApi.Email;
using WebApi.Exceptions;

namespace WebApi.Services;

public interface IEmailService
{
    Task SendEmail(string recipientEmail, EmailTemplate template);
}

public class EmailService(IConfiguration appConfiguration, ILogger<EmailService> logger)
    : IEmailService
{
    private readonly string _senderEmail = appConfiguration["EmailSettings:SenderEmail"];
    private readonly string _senderPassword = appConfiguration["EmailSettings:SenderPassword"];
    private readonly string _smtpServer = appConfiguration["EmailSettings:SmtpServer"];
    private readonly int _smtpPort = appConfiguration.GetValue<int>("EmailSettings:SmtpPort");
    private readonly ILogger<EmailService> _logger = logger;

    public async Task SendEmail(string recipientEmail, EmailTemplate template)
    {
        try
        {
            using MailMessage mail = new(_senderEmail, recipientEmail);
            using SmtpClient smtpClient = new(_smtpServer, _smtpPort);

            mail.Subject = template.Subject;
            mail.Body = template.Body;

            smtpClient.Credentials = new NetworkCredential(_senderEmail, _senderPassword);
            smtpClient.EnableSsl = true;

            _logger.LogInformation("Sending an email.");
            await smtpClient.SendMailAsync(mail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending the email.");
            throw new ServiceException(
                "An error occurred while sending the email.",
                ex);
        }
    }
}
