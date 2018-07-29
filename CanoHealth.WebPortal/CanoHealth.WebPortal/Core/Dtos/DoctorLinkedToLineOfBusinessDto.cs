using System;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class DoctorLinkedToLineOfBusinessDto
    {
        //Line of business
        public string LineOfBusinessName { get; set; }

        //Date when the doctor was linked to the line of business
        public DateTime EffectiveDate { get; set; }

        //Note releated to the linked action.
        public string Note { get; set; }

        //When there is a Provider number at Location level by line of business. Ex: Care Plus insurance
        public string ProviderNumberByLocation { get; set; }

    }
}