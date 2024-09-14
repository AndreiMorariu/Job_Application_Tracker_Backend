using System.Net;
using System.Net.Mail;

namespace Application_Tracker.Services
{
  public interface IEmailService
  {
    Task SendEmailAsync(string toEmail, string subject, string message);
  }

  public class EmailService : IEmailService
  {
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
      try
      {
        using (
            var client = new SmtpClient(
                _configuration["EmailSettings:Host"],
                int.Parse(_configuration["EmailSettings:Port"])
            )
        )
        {
          client.Credentials = new NetworkCredential
          {
            UserName = _configuration["EmailSettings:Username"],
            Password = _configuration["EmailSettings:Password"]
          };

          client.EnableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]);

          using (var emailMessage = new MailMessage())
          {
            emailMessage.To.Add(new MailAddress(toEmail));
            emailMessage.From = new MailAddress(_configuration["EmailSettings:From"]);
            emailMessage.Subject = subject;
            emailMessage.Body = message;
            emailMessage.IsBodyHtml = true;

            await client.SendMailAsync(emailMessage);
          }
        }
      }
      catch (Exception)
      {
        throw;
      }
    }
  }
}
