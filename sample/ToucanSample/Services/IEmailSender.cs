using System.Threading.Tasks;

namespace ToucanSample.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
