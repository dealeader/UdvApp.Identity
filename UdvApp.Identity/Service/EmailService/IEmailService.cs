using UdvApp.Identity.Models;

namespace UdvApp.Identity.Service.EmailService;

public interface IEmailService
{
    void SendEmail(Email email, string password);
}