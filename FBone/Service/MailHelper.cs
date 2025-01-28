using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;


namespace FBone.Service
{
    public static class MailHelper
    {
        public static void SendMail(List<MailAddress> mailto, List<MailAddress> mailcc, string subject, string body)
        {
            if (Config.emailEnabled)
            {
                string mailfrom = "noreply-fbone@tengizchevroil.com";
                if (mailcc == null)
                {
                    mailcc = new List<MailAddress> { new MailAddress(Config.SupportContactEmail) };
                }
                else
                {
                    mailcc.Add(new MailAddress(Config.SupportContactEmail));
                }
                
                if (Config.isProduction)
                    mailcc = null;

                var email = new MailMessage
                {
                    From = new MailAddress(mailfrom)
                };

                try
                {
                    //создадим сообщение...
                    AddEmailAdresses(mailto, mailcc, email);

                    email.Subject = subject;
                    email.IsBodyHtml = true;
                    email.Body = body;
                    email.BodyEncoding = System.Text.Encoding.UTF8;

                    //создадим клиента
                    var client = new SmtpClient
                    {
                        Host = Config.MailSMTPserver,
                        Port = Convert.ToInt16(Config.MailSmtpPort),
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        EnableSsl = true,
                        UseDefaultCredentials = true
                    };
                    if (Config.MailLoginName.Length != 0 && Config.MailLoginPassword.Length != 0)
                    {
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(Config.MailLoginName,
                            MyEncryption.DecryptString(Config.MailLoginPassword));
                    }

                    client.Send(email);
                    email.Dispose();
                    client.Dispose();
                }
                catch (SmtpException ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }
        public static void SendMail(List<MailAddress> mailto, string subject, string body)
        {
            SendMail(mailto, null, subject, body);            
        }        

        private static void AddEmailAdresses(List<MailAddress> mailto, List<MailAddress> mailcc, MailMessage email)
        {
            foreach (MailAddress item in mailto)
            {
                email.To.Add(item);
            }

            if (mailcc != null)
            {
                foreach (MailAddress item in mailcc)
                {
                    email.CC.Add(item);
                }
            }
        }
    }
}
