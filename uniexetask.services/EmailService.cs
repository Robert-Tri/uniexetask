using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using uniexetask.services.Interfaces;

namespace uniexetask.services
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

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(mail),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true 
                };
                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

    }
}
