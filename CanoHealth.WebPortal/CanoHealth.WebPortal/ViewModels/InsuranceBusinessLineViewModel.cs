using System;

namespace CanoHealth.WebPortal.ViewModels
{
    public class InsuranceBusinessLineViewModel
    {
        public Guid InsuranceBusinessLineId { get; set; }

        public Guid InsuranceId { get; set; }

        public Guid PlanTypeId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool? Active { get; set; }
    }
}