using System.Threading.Tasks;

namespace ToucanSample.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
