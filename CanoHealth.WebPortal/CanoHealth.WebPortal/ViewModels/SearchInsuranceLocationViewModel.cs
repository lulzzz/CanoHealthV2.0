using System;

namespace CanoHealth.WebPortal.ViewModels
{
    public class SearchInsuranceLocationViewModel
    {
        //Corporation Info
        public Guid CorporationId { get; set; }

        public string CorporationName { get; set; }

        //Insurance Info
        public Guid InsuranceId { get; set; }

        public string InsuranceName { get; set; }

        //Contract Info
        public string Contract { get; set; }

        //Doctor Info
        public string Degree { get; set; }

        public string FullName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string NpiNumber { get; set; }

        public string CaqhNumber { get; set; }

        //Line of Business Info
        public string LineOfBusiness { get; set; }

        /*Key: Doctor, Details: List of Line of Business*/
    }
}