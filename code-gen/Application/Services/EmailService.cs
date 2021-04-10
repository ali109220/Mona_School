using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Application.Services
{
    public class EmailService
    {
        IConfiguration _iConfiguration;
        SmtpClient _smtp;
        public EmailService(IConfiguration iConfiguration, SmtpClient smtp)
        {
            _iConfiguration = iConfiguration;
            _smtp = smtp;
        }
        public async Task<bool> SendEmail(string to, string subject, string htmlMessage)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.Subject = subject;
                mail.Body = htmlMessage;
                mail.IsBodyHtml = true;
                mail.To.Add(to);

                string fromAddress = _iConfiguration.GetValue<String>("Email:Smtp:From");
                string host = _iConfiguration.GetValue<String>("Email:Smtp:Host");
                int port = _iConfiguration.GetValue<int>("Email:Smtp:Port");
                bool ssl = _iConfiguration.GetValue<bool>("Email:Smtp:EnableSsl");
                string uname = _iConfiguration.GetValue<String>("Email:Smtp:Username");
                string pass = _iConfiguration.GetValue<String>("Email:Smtp:Password");
                string alias = _iConfiguration.GetValue<String>("Email:Smtp:Alias");
                //Setting From , To and CC
                mail.From = new MailAddress(fromAddress, alias);

                var client = new SmtpClient(host, port);

                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(uname, pass);
                client.EnableSsl = ssl;
                await client.SendMailAsync(mail);

                return true;

            }
            catch (Exception e)
            {
                return false;
            }
            

        }
    }
}
