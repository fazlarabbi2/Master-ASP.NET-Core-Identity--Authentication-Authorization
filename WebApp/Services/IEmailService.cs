
namespace WebApp.Services
{
    public interface IEmailService
    {
        Task SendAsync(string form, string to, string subject, string body);
    }
}