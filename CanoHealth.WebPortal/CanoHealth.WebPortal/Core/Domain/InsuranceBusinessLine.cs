using System;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class InsuranceBusinessLine
    {
        public Guid InsuranceBusinessLineId { get; set; }

        public Guid InsuranceId { get; set; }

        public Guid PlanTypeId { get; set; }

        //Navegation Properties
        public Insurance Insurance { get; set; }

        public PlanType BusinessLine { get; set; }
    }
}