#nullable disable

using System.Net;
using System.Net.Mail;
using WebApi.Exceptions;

namespace WebApi.Services;

public interface IEmailService
{
    Task SendEmailAsync(string recipientEmail, string filename);
}

public class EmailService(
    IConfiguration appConfiguration, IWebHostEnvironment environment)
    : IEmailService
{
    private readonly string _baseDirectory = environment.ContentRootPath;
    private readonly string _senderEmail = appConfiguration["EmailSettings:SenderEmail"];
    private readonly string _senderPassword = appConfiguration["EmailSettings:SenderPassword"];
    private readonly string _smtpServer = appConfiguration["EmailSettings:SmtpServer"];
    private readonly int _smtpPort = appConfiguration.GetValue<int>("EmailSettings:SmtpPort");

    public async Task SendEmailAsync(string recipientEmail, string filename)
    {
        try
        {
            string subjectFilePath = Path.Combine(_baseDirectory, $"{filename}.subject");
            string bodyFilePath = Path.Combine(_baseDirectory, $"{filename}.body");

            string subject = await File.ReadAllTextAsync(subjectFilePath);
            string body = await File.ReadAllTextAsync(bodyFilePath);

            using MailMessage mail = new(_senderEmail, recipientEmail);
            using SmtpClient smtpClient = new(_smtpServer, _smtpPort);

            mail.Subject = subject;
            mail.Body = body;

            smtpClient.Credentials = new NetworkCredential(_senderEmail, _senderPassword);
            smtpClient.EnableSsl = true;

            await smtpClient.SendMailAsync(mail);
        }
        catch (Exception ex)
        {
            throw new ServiceException(
                "An error occurred while sending the email.",
                ex);
        }
    }
}
