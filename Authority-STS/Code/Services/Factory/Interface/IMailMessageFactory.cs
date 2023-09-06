using System.Net.Mail;

namespace AuthoritySTS.Services.Factory.Interface
{
    public interface IMailMessageFactory
    {
        MailMessage CreateMailMessage(string from, string to);
    }
}
