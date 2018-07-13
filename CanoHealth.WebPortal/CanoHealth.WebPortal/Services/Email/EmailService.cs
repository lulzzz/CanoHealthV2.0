using CanoHealth.WebPortal.Infraestructure;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CanoHealth.WebPortal.Services.Email
{
    public class EmailService : IEmailService
    {
        private const string HtmlEmailHeader = "<html><head><title></title></head><body style='font-family:arial; font-size:14px;'>";
        private const string HtmlEmailFooter = "";

        // properties

        public List<string> To { get; set; }
        public List<string> CC { get; set; }
        public List<string> BCC { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public EmailService()
        {
            To = new List<string>();
            CC = new List<string>();
            BCC = new List<string>();
        }

        public async Task SendSmtpEmailAsync()
        {
            // Configure the client:
            var client = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(ConfigurationDAL.GetSendGridPort));

            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            // Creatte the credentials:
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(ConfigurationDAL.GetSendGridUser, ConfigurationDAL.GetSendGridPassword);

            //client.EnableSsl = true;
            client.Credentials = credentials;

            MailMessage message = new MailMessage();

            foreach (var x in To)
            {
                message.To.Add(x);
            }
            foreach (var x in CC)
            {
                message.CC.Add(x);
            }
            foreach (var x in BCC)
            {
                message.Bcc.Add(x);
            }

            message.Subject = Subject;
            message.Body = Body;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.From = new MailAddress(ConfigurationDAL.GetSendGridFromAddress);
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;

            await client.SendMailAsync(message);
        }

        public async Task SendApiEmailAsync()
        {
            var apiKey = Environment.GetEnvironmentVariable("SendGrid");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("tikikon10@gmail.com", "Example User");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress("suarezhar@gmail.com", "Example User");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}