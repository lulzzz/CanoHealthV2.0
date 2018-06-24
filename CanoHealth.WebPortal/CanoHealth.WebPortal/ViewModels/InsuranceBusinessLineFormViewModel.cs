using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.ViewModels
{
    public class InsuranceBusinessLineFormViewModel
    {
        public Guid InsuranceBusinessLineId { get; set; }

        public Guid InsuranceId { get; set; }

        public IEnumerable<BusinessLineViewModel> BusinessLines { get; set; }

        public InsuranceBusinessLineFormViewModel()
        {
            BusinessLines = new List<BusinessLineViewModel>();
        }
    }
}