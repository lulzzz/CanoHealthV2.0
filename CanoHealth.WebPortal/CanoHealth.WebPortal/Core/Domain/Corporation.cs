using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class Corporation
    {
        public Guid CorporationId { get; set; }

        [Required]
        [StringLength(100)]
        public string CorporationName { get; set; }

        [StringLength(100)]
        public string Npi { get; set; }

        [StringLength(100)]
        public string TaxId { get; set; }

        [Required]
        [StringLength(255)]
        public string Address { get; set; }

        [Phone]
        [DataType(DataType.PhoneNumber)]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        public bool Active { get; set; }

        //Navegation Properties
        public ICollection<Contract> Contracts { get; set; }

        public Corporation()
        {
            Contracts = new Collection<Contract>();
        }
    }
}