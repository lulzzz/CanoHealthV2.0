using System;

namespace CanoHealth.WebPortal.Core.Dtos
{
    /*Class used to link doctors with corporation contracts*/
    public class ContractBusinessLineDto
    {
        //PK from ContractLineOfBusinesses Entity
        public Guid ContractLineofBusinessId { get; set; }

        //PK from PlanType Entity
        public Guid PlanTypeId { get; set; }

        // Property of PlanType Entity
        public string Name { get; set; }
    }
}