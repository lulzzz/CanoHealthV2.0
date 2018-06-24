using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class PlanType
    {
        public Guid PlanTypeId { get; set; }

        [StringLength(100)]
        public string Code { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        //Navegation Properties
        public ICollection<ContractLineofBusiness> LineofBusinesses { get; set; }

        public ICollection<InsuranceBusinessLine> InsuranceBusinessLines { get; set; }

        public PlanType()
        {
            LineofBusinesses = new Collection<ContractLineofBusiness>();
            InsuranceBusinessLines = new List<InsuranceBusinessLine>();
        }
    }
}