using System.Threading.Tasks;

namespace AuthoritySTS.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        string GetTemplateByName(string url, string subjectId);
    }
}
