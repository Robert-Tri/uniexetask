namespace uniexetask.api.Extensions
{
    public interface IEmail
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
