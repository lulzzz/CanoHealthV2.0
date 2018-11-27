using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.ViewModels
{
    public class InsuranceFormViewModel
    {
        public Guid? InsuranceId { get; set; }

        [StringLength(10)]
        public string Code { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        public bool Active { get; set; }

        [Required]
        [Display(Name = "Line of Business")]
        public List<BusinessLineViewModel> LineofBusiness { get; set; }

        public InsuranceFormViewModel()
        {
            LineofBusiness = new List<BusinessLineViewModel>();
        }
    }
}