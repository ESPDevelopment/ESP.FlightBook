using System.Threading.Tasks;

namespace ESP.FlightBook.Identity.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
