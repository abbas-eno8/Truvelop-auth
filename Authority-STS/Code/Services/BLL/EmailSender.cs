using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using AuthoritySTS.Models;
using AuthoritySTS.Services.Factory.Interface;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using AuthoritySTS.Services.Shared;

namespace AuthoritySTS.Services.BLL
{    
    /// <summary>
    /// Class Name      : EmailSender
    /// Author          : Nehal Patel
    /// Creation Date   : 11-06-2018
    /// Purpose         : Class to send email
    /// Revision        : 
    /// </summary>  
    public class EmailSender : IEmailSender
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMailMessageFactory _mailMessageFactory;
        private readonly IConfiguration _iConfiguration;
        private readonly string CatalogConnectionString;

        public EmailSender(IServiceFactory serviceFactory, IMailMessageFactory mailMessageFactory, IConfiguration iConfiguration)
        {
            _serviceFactory = serviceFactory;
            _mailMessageFactory = mailMessageFactory;
            _iConfiguration = iConfiguration.GetSection("SmtpSettings");
            CatalogConnectionString = iConfiguration.GetConnectionString("1AuthorityConnection");
        }

        /// <summary>        
        /// Method Name     : SendEmailAsync
        /// Author          : Nehal Patel
        /// Creation Date   : 11-06-2018
        /// Purpose         : Method to send email
        /// Revision        :         
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        //public Task SendEmailAsync(string email, string subject, string message)
        //{
        //    var _smtpSettings = new SmtpSettings();
        //    _iConfiguration.Bind(_smtpSettings);

        //    using (MailMessage mm = _mailMessageFactory.CreateMailMessage(_smtpSettings.From, email))
        //    {
        //        mm.Subject = subject;
        //        mm.Body = message;
        //        mm.IsBodyHtml = true;

        //        var networkCredential = _serviceFactory.CreateInstance<NetworkCredential>();
        //        networkCredential.UserName = _smtpSettings.UserName;
        //        networkCredential.Password = _smtpSettings.Password;

        //        var smtp = _serviceFactory.CreateInstance<SmtpClient>();
        //        smtp.Host = _smtpSettings.Host;
        //        smtp.EnableSsl = _smtpSettings.EnableSsl;
        //        smtp.UseDefaultCredentials = _smtpSettings.UseDefaultCredentials;
        //        smtp.Port = _smtpSettings.Port;
        //        smtp.Credentials = networkCredential;

        //        smtp.Send(mm);
        //    }

        //    return Task.CompletedTask;
        //}

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var _smtpSettings = new SmtpSettings();
            _iConfiguration.Bind(_smtpSettings);

            MailMessage mailMessage = new MailMessage();
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = message;
            mailMessage.Subject = subject;
            mailMessage.From = new MailAddress(_smtpSettings.UserName);
            mailMessage.To.Add(email);

            var mailClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password),
                EnableSsl = true
            };

            mailClient.Send(mailMessage);

            return Task.CompletedTask;
        }

        //public string GetHTMLBody(string url)
        //{
        //    string strBody = string.Empty;
        //    try
        //    {
        //        using (StreamReader reader = new StreamReader("UserResetPassword.html"))
        //        {
        //            strBody = reader.ReadToEnd();
        //        }
        //        strBody = strBody.Replace("{ResetPasswordURL}", url);

        //        return strBody;
        //    }
        //    catch (System.Exception)
        //    {
        //        return strBody;
        //    }
        //}
        public string GetTemplateByName(string url,string subjectId)
        {
            string template = string.Empty;
            int companyId = 0;
            string templateName = "UserResetPassword";

            SqlConnection sqlConnection = new SqlConnection(CatalogConnectionString);
            try
            {
                using (var sqlCommand = new SqlCommand())
                {
                    if (sqlConnection.State != ConnectionState.Open)
                    {
                        sqlConnection.Open();
                    }
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = Messages.GetTemplateByName;
                    sqlCommand.Parameters.AddWithValue("@companyId", companyId);
                    sqlCommand.Parameters.AddWithValue("@templateName", templateName);
                    sqlCommand.Parameters.AddWithValue("@subjectId", subjectId);
                    
                    template = (string)sqlCommand.ExecuteScalar();
                    sqlCommand.Parameters.Clear();
                }
            }
            finally
            {
                sqlConnection.Close();
            }

            template = template.Replace("{ResetPasswordURL}", url);
            return template;
        }
    }
}
