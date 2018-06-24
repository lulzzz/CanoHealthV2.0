using System;

namespace CanoHealth.WebPortal.ViewModels
{
    public class SearchInsuranceDoctorViewModel
    {
        public Guid CorporationId { get; set; }
        public string CorporationName { get; set; }
        public Guid InsuranceId { get; set; }
        public string InsuranceName { get; set; }
        public string GroupNumber { get; set; }
        public Guid PlanTypeId { get; set; }
        public string LineOfBusiness { get; set; }
        public Guid PlaceOfServiceId { get; set; }
        public string Location { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string Address { get; set; }

        /*Key: Place of Service, Details: List of Line of Business*/
    }
}