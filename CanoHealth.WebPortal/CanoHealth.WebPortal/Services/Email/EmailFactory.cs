using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Dtos;
using CanoHealth.WebPortal.Infraestructure;
using System.Collections.Generic;
using System.Linq;

namespace CanoHealth.WebPortal.Services.Email
{
    public class EmailFactory : IEmailFactory
    {
        public string Style { get; private set; }

        public EmailFactory()
        {
            Style = @"<style>
	                    table {
                          border-collapse: collapse;
                          border-spacing: 0;
                        }
                        td,
                        th {
                          padding: 0;
                        }
                        .table {
	                        border-collapse: collapse !important;
                          }
                          .table td,
                          .table th {
	                        background-color: #fff !important;
                          }
                          .table-bordered th,
                          .table-bordered td {
	                        border: 1px solid #ddd !important;
                          }
  
                          table {
                          background-color: transparent;
                        }

                        th {
                          text-align: left;
                        }
                        .table {
                          width: 100%;
                          max-width: 100%;
                          margin-bottom: 20px;
                        }
                        .table > thead > tr > th,
                        .table > tbody > tr > th,
                        .table > tfoot > tr > th,
                        .table > thead > tr > td,
                        .table > tbody > tr > td,
                        .table > tfoot > tr > td {
                          padding: 8px;
                          line-height: 1.42857143;
                          vertical-align: top;
                          border-top: 1px solid #ddd;
                        }
                        .table > thead > tr > th {
                          vertical-align: bottom;
                          border-bottom: 2px solid #ddd;
                        }
                        .table > caption + thead > tr:first-child > th,
                        .table > colgroup + thead > tr:first-child > th,
                        .table > thead:first-child > tr:first-child > th,
                        .table > caption + thead > tr:first-child > td,
                        .table > colgroup + thead > tr:first-child > td,
                        .table > thead:first-child > tr:first-child > td {
                          border-top: 0;
                        }
                        .table > tbody + tbody {
                          border-top: 2px solid #ddd;
                        }
                        .table .table {
                          background-color: #fff;
                        }
                        .table-bordered {
                          border: 1px solid #ddd;
                        }
                        .table-bordered > thead > tr > th,
                        .table-bordered > tbody > tr > th,
                        .table-bordered > tfoot > tr > th,
                        .table-bordered > thead > tr > td,
                        .table-bordered > tbody > tr > td,
                        .table-bordered > tfoot > tr > td {
                          border: 1px solid #ddd;
                        }
                        .table-bordered > thead > tr > th,
                        .table-bordered > thead > tr > td {
                          border-bottom-width: 2px;
                        }
                        .section-title {border-bottom: solid 1px #ddd;padding-bottom: 15px;margin-top: 39px;color: #000;}
                        </style>";
        }

        public Emails GetExpireLicenseTemplate(IEnumerable<NotificationDto> notifications)
        {
            var body1 = "<h2 class='section-title'>" + "List of Licenses are going to expire in the next 2 months" + "</h2></br>";

            //Get the list of Medical Licenses
            var medicalLicenses = notifications.Where(x => x.Source.Equals(ConfigureSettings.GetMedicalLicense, System.StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            //Get the list of Location Licenses
            var locationLicenses = notifications.Where(x => x.Source.Equals(ConfigureSettings.GetLocationLicense, System.StringComparison.InvariantCultureIgnoreCase))
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

            var email = new Emails
            {
                Subject = "Licenses to expire",
                From = ConfigurationDAL.GetSendGridFromAddress,
                To = new List<string> { ConfigurationDAL.GetCredentialingContact },
                Body = Style + body1
            };

            return email;
        }
    }
}