using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class Insurance
    {
        public Guid InsuranceId { get; set; }

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

        //Navegation Properties
        public ICollection<Contract> Contracts { get; set; }

        public ICollection<InsuranceBusinessLine> InsuranceBusinessLines { get; set; }

        public Insurance()
        {
            Contracts = new Collection<Contract>();
            InsuranceBusinessLines = new Collection<InsuranceBusinessLine>();
        }

        #region InsuranceBusinessLogic

        public IEnumerable<AuditLog> InactivateInsurance()
        {
            var logs = Contracts.Where(c => c.Active).Select(contract => contract.InactivateContract()).ToList();
            logs.Add(AuditLog.AddLog("Insurances", "Active", true.ToString(), false.ToString(), InsuranceId, "Update"));
            Active = false;
            return logs;
        }

        public IEnumerable<AuditLog> ModifyInsurance(Insurance insurance)
        {
            var auditLogs = new List<AuditLog>();
            if (Code != insurance.Code)
            {
                auditLogs.Add(AuditLog.AddLog("Insurance", "Code", Code, insurance.Code, InsuranceId, "Update"));
                Code = insurance.Code;
            }
            if (Name != insurance.Name)
            {
                auditLogs.Add(AuditLog.AddLog("Insurance", "Name", Name, insurance.Name, InsuranceId, "Update"));
                Name = insurance.Name;
            }
            if (PhoneNumber != insurance.PhoneNumber)
            {
                auditLogs.Add(AuditLog.AddLog("Insurance", "PhoneNumber", PhoneNumber, insurance.PhoneNumber, InsuranceId, "Update"));
                PhoneNumber = insurance.PhoneNumber;
            }
            if (Address != insurance.Address)
            {
                auditLogs.Add(AuditLog.AddLog("Insurance", "Address", Address, insurance.Address, InsuranceId, "Update"));
                Address = insurance.Address;
            }
            if (Active != insurance.Active)
            {
                if (Active)
                {
                    auditLogs.AddRange(InactivateInsurance());
                }
                else
                {
                    auditLogs.Add(AuditLog.AddLog("Insurances", "Active", true.ToString(), false.ToString(), InsuranceId, "Update"));
                    Active = insurance.Active;
                }
            }
            return auditLogs;
        }

        public Contract GetContract(Guid corporationId)
        {
            return Contracts.SingleOrDefault(x => x.CorporationId == corporationId && x.InsuranceId == InsuranceId);
        }

        #endregion
    }
}