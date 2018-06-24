using System.Collections.Generic;
using CanoHealth.WebPortal.Core.Domain;

namespace CanoHealth.WebPortal.ViewModels
{
    public class ContractBusinessLineModifyViewModel
    {
        public IEnumerable<AuditLog> Logs { get; set; }
        public IEnumerable<ClinicLineofBusinessContract> Cliniccontractlineofbusiness { get; set; }
    }
}