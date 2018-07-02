using CanoHealth.WindowsService.Infrastructure;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;

namespace CanoHealth.WindowsService.BLL
{
    public class NotificationEmails
    {
        private static EventLog eventLog1;

        public static object ConsigurationSettings { get; private set; }

        public static void SendNotifications(EventLog EL)
        {
            try
            {
                eventLog1 = EL;

                //Get the list of licenses that ar going to expire in the next 2 months
                var expireLicenses = ExpirationDates.GetLicenseExpirationDates();

                //I there is not any of them  finish the execution
                if (!expireLicenses.Any())
                {
                    eventLog1.WriteEntry("Nothing to notify.");
                    return;
                }

                eventLog1.WriteEntry("Sending Emails.");

                var style = "<style> table, th, td {border: 1px solid black;border-collapse: collapse;} th, td {padding: 5px;} .section-title {border-bottom: solid 1px #ddd;padding-bottom: 15px;margin-top: 39px;color: #000;}</style>";
                var body1 = "<h2 class='section-title'>" + "List of Licenses are going to expire in the next 2 months" + "</h2></br>";

                //Get the list of Medical Licenses
                var medicalLicenses = expireLicenses.Where(x => x.Source.Equals(ConfigureSettings.GetMedicalLicense, System.StringComparison.InvariantCultureIgnoreCase))
                    .ToList();

                //Get the list of Location Licenses
                var locationLicenses = expireLicenses.Where(x => x.Source.Equals(ConfigureSettings.GetLocationLicense, System.StringComparison.InvariantCultureIgnoreCase))
                    .ToList();

                //Build the message's body
                if (medicalLicenses.Any())
                {
                    body1 += "<table style='width:50%; border: 1px solid black;border-collapse: collapse;'>" +
                                "<tr>" +
                                    "<th style='border: 1px solid black;border-collapse: collapse; padding: 5px;'>Source</th>" +
                                    "<th style='border: 1px solid black;border-collapse: collapse; padding: 5px;'>License Type</th>" +
                                    "<th style='border: 1px solid black;border-collapse: collapse; padding: 5px;'>Doctor</th>" +
                                    "<th style='border: 1px solid black;border-collapse: collapse; padding: 5px;'>Expire Date</th>" +
                                "</tr>";

                    foreach (var medicalLicense in medicalLicenses)
                    {
                        body1 += "<tr>" +
                                    "<td style='border: 1px solid black;border-collapse: collapse; padding: 5px;'>" + medicalLicense.Source + "</td>" +
                                    "<td style='border: 1px solid black;border-collapse: collapse; padding: 5px;'>" + medicalLicense.LicenseType + "</td>" +
                                    "<td style='border: 1px solid black;border-collapse: collapse; padding: 5px;'>" + medicalLicense.DoctorFullName + "</td>" +
                                    "<td style='border: 1px solid black;border-collapse: collapse; padding: 5px;'>" + medicalLicense.ExpirationDate + "</td>" +
                                "</tr>" +
                                "</table></br>";

                    }
                }

                if (locationLicenses.Any())
                {
                    body1 += "<table style='width:50%; border: 1px solid black;border-collapse: collapse;'>" +
                                "<tr>" +
                                    "<th style='border: 1px solid black;border-collapse: collapse; padding: 5px;'>Source</th>" +
                                    "<th style='border: 1px solid black;border-collapse: collapse; padding: 5px;'>License Type</th>" +
                                    "<th style='border: 1px solid black;border-collapse: collapse; padding: 5px;'>Location</th>" +
                                    "<th style='border: 1px solid black;border-collapse: collapse; padding: 5px;'>Expire Date</th>" +
                                "</tr>";

                    foreach (var locationLicense in locationLicenses)
                    {
                        body1 += "<tr>" +
                                   "<td style='border: 1px solid black;border-collapse: collapse; padding: 5px;'>" + locationLicense.Source + "</td>" +
                                   "<td style='border: 1px solid black;border-collapse: collapse; padding: 5px;'>" + locationLicense.LicenseType + "</td>" +
                                   "<td style='border: 1px solid black;border-collapse: collapse; padding: 5px;'>" + locationLicense.Location + "</td>" +
                                   "<td style='border: 1px solid black;border-collapse: collapse; padding: 5px;'>" + locationLicense.ExpirationDate + "</td>" +
                               "</tr>" +
                               "</table></br>";

                    }
                }

                //Send the message
                var msg = new MailMessage();
                msg. = new MailAddress(ConfigureSettings.GetCredentialingContact);
                msg.From = new MailAddress(ConfigureSettings.EmailSettingsFrom);
                msg.Subject = "New Expire Dates to review.";
                msg.Body = style + body1;
                msg.IsBodyHtml = true;

                using (var smtp = new Smtp())
                {
                    // connect to SMTP server
                    smtp.Connect(ServerName, SslMode.Explicit);

                    // authenticate with your email address and password
                    smtp.Login(UserName, Password);

                    // register the RejectedRecipient handler
                    smtp.RejectedRecipient += smtp_RejectedRecipient;

                    // send mail
                    smtp.Send(msg);

                    // disconnect (not required, but polite)
                    smtp.Disconnect();
                }
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry("Error Sending the emails: " + ex.Message + Environment.NewLine +
                                    "Inner Exception: " + ex.InnerException + Environment.NewLine + Environment.NewLine +
                                    ex.ToString());
            }

        }
    }

    static void smtp_RejectedRecipient(object sender, SmtpRejectedRecipientEventArgs e)
    {
        var message = new MailMessage
        {
            Subject = "Invalid email address.",
            To = NotInvalidEmailAddr,
            From = UserName
        };
        message.BodyHtml = string.Format(@"Recipient {0} rejected by the server ('{1}').", e.Recipient, e.Response.Description);
        Smtp.Send(message, SmtpConfiguration.Default);

        // don't send to this recipient
        e.Ignore = true;
    }
}
