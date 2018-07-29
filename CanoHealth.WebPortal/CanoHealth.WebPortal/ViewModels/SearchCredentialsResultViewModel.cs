using System;

namespace CanoHealth.WebPortal.ViewModels
{
    public class SearchCredentialsResultViewModel
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

        public DateTime? DateOfBirth { get; set; }

        public string NpiNumber { get; set; }

        public string CaqhNumber { get; set; }

        //Doctor Provider Number by Insurance
        public string ProviderNumberByInsurance { get; set; }

        //Location Info
        public Guid? PlaceOfServiceId { get; set; }

        public string Location { get; set; }

        public string PhoneNumber { get; set; }

        public string FaxNumber { get; set; }

        public string Address { get; set; }

        //Line of Business Info
        public Guid PlanTypeId { get; set; }

        public string LineOfBusiness { get; set; }

        //Date when the doctor was linked to the line of business
        public DateTime EffectiveDate { get; set; }

        //Note releated to the linked action.
        public string Note { get; set; }

        //When there is a Provider number at Location level by line of business. Ex: Care Plus insurance
        public string ProviderNumberByLocation { get; set; }

        /*Key: Doctor, Details: List of Line of Business*/
    }
}