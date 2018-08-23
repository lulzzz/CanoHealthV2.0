using CanoHealth.WebPortal.CommonTools.ExpireLicense;
using CanoHealth.WebPortal.CommonTools.Roles;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Dtos;
using CanoHealth.WebPortal.Infraestructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Http;


namespace CanoHealth.WebPortal.Controllers.Api
{
    [Authorize]
    public class NotificationsController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult GetNotifications()
        {
            var result = new List<NotificationDto>();
            if (User.IsInRole(RoleName.Admin))
            {
                result = _unitOfWork.ExpireDateNotificationRepository
                    .GetNotifications()
                    .Where(x => !x.Source.Equals(ExpireLicenseSource.OutOfNetworkContracts, System.StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IHttpActionResult> GetExpirationDates()
        {
            var expireLicenses = _unitOfWork.ExpireDateNotificationRepository
                                .GetNotifications()
                                .ToList();

            var style = "<style> table, th, td {border: 1px solid black;border-collapse: collapse;} th, td {padding: 5px;} .section-title {border-bottom: solid 1px #ddd;padding-bottom: 15px;margin-top: 39px;color: #000;} .table {border-collapse: collapse !important;}.table td,.table th{background-color: #fff !important;}.table-bordered th,.table-bordered td{border: 1px solid #ddd !important;}" +
                ".table-bordered {" +
                    "border: 1px solid #ddd;" +
                "}" +
                ".table-bordered > thead > tr > th," +
                ".table-bordered > tbody > tr > th," +
                ".table-bordered > tfoot > tr > th,  " +
                ".table-bordered > thead > tr > td,  " +
                ".table-bordered > tbody > tr > td,  " +
                ".table-bordered > tfoot > tr > td { " +
                    "border: 1px solid #ddd;           " +
                "}                                   " +
                ".table-bordered > thead > tr > th,   " +
                ".table-bordered > thead > tr > td {  " +
                    "border-bottom-width: 2px;  " +
                "}" +
                "</style>";
            var body1 = "<h2 class='section-title'>" + "List of Licenses are going to expire in the next 2 months" + "</h2></br>";

            /*
             
                <table class="table table-bordered"> 
                    <thead> 
                        <tr> 
                            <th>#</th> 
                            <th>First Name</th> 
                            <th>Last Name</th> 
                            <th>Username</th> 
                        </tr> 
                    </thead> 
                    <tbody> 
                        <tr> 
                            <th scope="row">1</th> 
                            <td>Mark</td> 
                            <td>Otto</td> 
                            <td>@mdo</td> 
                        </tr> 
                        <tr> 
                            <th scope="row">2</th> 
                            <td>Jacob</td> 
                            <td>Thornton</td> 
                            <td>@fat</td> 
                        </tr> 
                        <tr> 
                            <th scope="row">3</th> 
                            <td>Larry</td> 
                            <td>the Bird</td> 
                            <td>@twitter</td> 
                        </tr> 
                    </tbody> 
                </table>
            
             */

            //Get the list of Medical Licenses
            var medicalLicenses = expireLicenses.Where(x => x.Source.Equals(ConfigureSettings.GetMedicalLicense, System.StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            //Get the list of Location Licenses
            var locationLicenses = expireLicenses.Where(x => x.Source.Equals(ConfigureSettings.GetLocationLicense, System.StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            //Build the message's body border-collapse: collapse; padding: 5px;
            if (medicalLicenses.Any())
            {
                body1 += "<table class='table table-bordered'>" +
                            "<thead><tr>" +
                                "<th>Source</th>" +
                                "<th>License Type</th>" +
                                "<th>Doctor</th>" +
                                "<th>Expire Date</th>" +
                            "</tr></thead>";

                foreach (var medicalLicense in medicalLicenses)
                {
                    body1 += "<tbody><tr>" +
                                "<td>" + medicalLicense.Source + "</td>" +
                                "<td>" + medicalLicense.LicenseType + "</td>" +
                                "<td>" + medicalLicense.DoctorFullName + "</td>" +
                                "<td>" + medicalLicense.ExpirationDate + "</td>" +
                            "</tr></tbody>";

                }
                body1 += "</table></br></br></br>";
            }

            if (locationLicenses.Any())
            {
                body1 += "<table class='table table-bordered'" +
                            "<tr>" +
                                "<th>Source</th>" +
                                "<th>License Type</th>" +
                                "<th>Location</th>" +
                                "<th>Expire Date</th>" +
                            "</tr>";

                foreach (var locationLicense in locationLicenses)
                {
                    body1 += "<tr>" +
                               "<td>" + locationLicense.Source + "</td>" +
                               "<td>" + locationLicense.LicenseType + "</td>" +
                               "<td>" + locationLicense.Location + "</td>" +
                               "<td>" + locationLicense.ExpirationDate + "</td>" +
                           "</tr>";
                }
                body1 += "</table></br></br></br>";
            }

            // Plug in your email service here to send an email.
            // Configure the client:
            var client = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(ConfigurationDAL.GetSendGridPort));

            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            // Creatte the credentials:
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(ConfigurationDAL.GetSendGridUser, ConfigurationDAL.GetSendGridPassword);

            //client.EnableSsl = true;
            client.Credentials = credentials;

            MailMessage message = new MailMessage(new MailAddress(ConfigurationDAL.GetSendGridFromAddress),
                new MailAddress("suarezhar@gmail.com"));

            message.Subject = "Expiration Licenses";
            message.Body = style + body1;
            message.BodyEncoding = System.Text.Encoding.UTF8;

            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;

            await client.SendMailAsync(message);

            return Ok();
        }
    }
}
