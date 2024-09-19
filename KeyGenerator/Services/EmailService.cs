//using KeyGenerator.Data;
//using MySqlX.XDevAPI.Relational;
//using System;
//using System.Net;
//using System.Net.Mail;
//using System.Runtime.Intrinsics.X86;

//namespace KeyGenerator.Services
//{


//    public class EmailService
//    {
//        private readonly KeyGeneratorDBContext _context;
//        private readonly ILoggerService _logger;
//        public EmailService(KeyGeneratorDBContext context,ILoggerService logger)
//        {

//            _context = context;
//            _logger = logger;

//        }

//        /*public static string SendEmail(string to, string subject, string emailBody)
//        {
//            string FROM = "CUPL199935@GMAIL.COM";
//            string FROMNAME = "Test Email";
//            string TO = to;
//            string SMTP_USERNAME = "AKIAVPTQXZJTH37MZ3CO";
//            string SMTP_PASSWORD = "BE2OamjqDFxItg//OkQ5oPplgz1mKGq7l8KDmJdty8Zi";
//            string HOST = "email-smtp.us-east-1.amazonaws.com";
//            int PORT = 587;
//            string SUBJECT = subject;
//            string BODY = emailBody;
//            MailMessage message = new MailMessage();
//            message.IsBodyHtml = true;
//            message.From = new MailAddress(FROM, FROMNAME);
//            message.To.Add(new MailAddress(TO));
//            message.Subject = SUBJECT;
//            message.Body = BODY;
//            using (var client = new System.Net.Mail.SmtpClient(HOST, PORT))
//            {
//                client.Credentials = new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);
//                client.EnableSsl = true;
//                try
//                {
//                    client.Send(message);
//                    return ("email sent");
//                }
//                catch (Exception ex)
//                {
//                    return (ex.Message);
//                }
//            }
//        }*/

//        private static readonly string SmtpServer = "smtp.gmail.com";
//        private static readonly int SmtpPort = 587;

//        //Smtp smtp 
//       // private static readonly string SenderEmail = "shivom.chandrakala@gmail.com";
//        //private static readonly string SenderPassword = "dqmdnxgvmyofntjj";

//        private static readonly string SenderEmail = "cupl199935@gmail.com";
//        private static readonly string SenderPassword = "svpkyvjwkdyvqlol";

//        //kartikeya smtp 
//        /*private static readonly string SenderEmail = "kartikey.kg.888@gmail.com";
//        private static readonly string SenderPassword = "isdjizkigsjvnpst";*/


//        public string SendEmail(string to, string subject, string body)
//        {
//            var smtpClient = new SmtpClient(SmtpServer)
//            {
//                Port = SmtpPort,
//                Credentials = new NetworkCredential(SenderEmail, SenderPassword),
//                EnableSsl = true,
//            };

//            var mailMessage = new MailMessage
//            {
//                From = new MailAddress(SenderEmail),
//                Subject = subject,
//                Body = body,
//                IsBodyHtml = true,
//            };

//            mailMessage.To.Add(to);
//            try
//            {
//                smtpClient.Send(mailMessage);
//                return ("email sent");
//            }
//            catch(Exception ex)
//            {
//                _logger.LogError($"{ex}", $"{ex.Message}", "EmailService");
//                return (ex.Message);
//            }

//        }
//    }
//}

using KeyGenerator.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;

namespace KeyGenerator.Services
{
    public class EmailService
    {
        private readonly KeyGeneratorDBContext _context;
        private readonly ILoggerService _logger;
        private readonly IConfiguration _configuration;

        public EmailService(KeyGeneratorDBContext context, ILoggerService logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public string SendEmail(string to, string subject, string body)
        {
            string senderEmail = _configuration["EmailSettings:Email"];
            string senderPassword = _configuration["EmailSettings:Password"];
            string smtpServer = _configuration["EmailSettings:Host"];
            int smtpPort = _configuration.GetValue<int>("EmailSettings:Port");

            var smtpClient = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(to);
            try
            {
                smtpClient.Send(mailMessage);
                return ("email sent");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}", $"{ex.Message}", "EmailService");
                return (ex.Message);
            }
        }
    }
}
