using System.Threading.Tasks;

namespace AuthoritySTS.Services.Interface
{
    public interface INotification
    {
        Task SendEmailConfirmationAsync(string email, string link);

        Task SendOtp(bool isemail, string email, string phoneNumber, string otp, bool isPasswordreset);

        Task SendCode(string provider, string email, string phoneNumber, string code);
    }
}