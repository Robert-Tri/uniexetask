using System.Net.Mail;
using System.Net;

namespace uniexetask.api.Extensions
{
    public class Email : IEmail
    {
        private readonly IConfiguration _configuration;
        public Email(IConfiguration configuration)
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
