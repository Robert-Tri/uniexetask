using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace uniexetask.api.Extensions
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var mail = _configuration["EmailSettings:Email"];
                var password = _configuration["EmailSettings:Password"];
                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(mail, password)
                };
                await client.SendMailAsync(new MailMessage(from: mail, to: email, subject, message));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
