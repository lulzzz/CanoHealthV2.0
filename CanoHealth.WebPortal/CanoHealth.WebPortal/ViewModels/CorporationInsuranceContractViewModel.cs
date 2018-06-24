using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.ViewModels
{
    public class CorporationInsuranceContractViewModel
    {
        public Guid CorporationId { get; set; }

        [Required]
        public string InsuranceName { get; set; }

        [Required]
        public string GroupNumber { get; set; }

        public IEnumerable<ClinicLineOfBusinessViewModel> ClinicLineOfBusiness { get; set; }

        public CorporationInsuranceContractViewModel()
        {
            ClinicLineOfBusiness = new List<ClinicLineOfBusinessViewModel>();
        }

        //public Guid PlanTypeId { get; set; }

        //[Required]
        //public IEnumerable<ClinicViewModel> Clinics { get; set; }

        //public CorporationInsuranceContractViewModel()
        //{
        //    Clinics = new List<ClinicViewModel>();
        //}
    }

    public class ClinicLineOfBusinessViewModel
    {
        public Guid PlanTypeId { get; set; }

        public IEnumerable<ClinicViewModel> Clinics { get; set; }

        public ClinicLineOfBusinessViewModel()
        {
            Clinics = new List<ClinicViewModel>();
        }
    }
}

