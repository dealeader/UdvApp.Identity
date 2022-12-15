using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using UdvApp.Identity.Models;

namespace UdvApp.Identity.Service.EmailService;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public void SendEmail(Email model, string password)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_configuration["EmailUserName"]));
        email.To.Add(MailboxAddress.Parse(model.To));
        email.Subject = model.Subject;
        email.Body = new TextPart(TextFormat.Html)
        {
            Text = $"{model.Body}: {password}"
        };

        using var smtp = new SmtpClient();
        smtp.Connect(_configuration["EmailHost"], Convert.ToInt32(_configuration["EmailPort"]), SecureSocketOptions.StartTls);
        smtp.Authenticate(_configuration["EmailUserName"], _configuration["EmailPassword"]);
        smtp.Send(email);
        smtp.Disconnect(true);
    }
}