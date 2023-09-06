using System.Threading.Tasks;

namespace AuthoritySTS.Services
{
    public interface ISmsSender
    {
        Task<string> SendSmsAsync(string number, string message);
    }
}
