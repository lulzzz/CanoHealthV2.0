using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class DoctorIndividualProvider
    {
        public Guid DoctorIndividualProviderId { get; set; }

        public Guid DoctorId { get; set; }

        public Guid InsuranceId { get; set; }

        [Required]
        [StringLength(20)]
        public string ProviderNumber { get; set; }

        public DateTime IndividualProviderEffectiveDate { get; set; }

        //Navegation Properties
        public Doctor Doctor { get; set; }

        public Insurance Insurance { get; set; }

        public IEnumerable<AuditLog> Modify(DoctorIndividualProvider doctorIndividualProvider)
        {
            var auditLogs = new List<AuditLog>();
            if (DoctorId != doctorIndividualProvider.DoctorId)
            {
                auditLogs.Add(AuditLog.AddLog("DoctorIndividualProviders",
                    "DoctorId",
                    DoctorId.ToString(),
                    doctorIndividualProvider.DoctorId.ToString(),
                    DoctorIndividualProviderId,
                    "Update"));
                DoctorId = doctorIndividualProvider.DoctorId;
            }
            if (InsuranceId != doctorIndividualProvider.InsuranceId)
            {
                auditLogs.Add(AuditLog.AddLog("DoctorIndividualProviders",
                    "InsuranceId",
                    InsuranceId.ToString(),
                    doctorIndividualProvider.InsuranceId.ToString(),
                    DoctorIndividualProviderId,
                    "Update"));
                InsuranceId = doctorIndividualProvider.InsuranceId;
            }
            if (ProviderNumber != doctorIndividualProvider.ProviderNumber)
            {
                auditLogs.Add(AuditLog.AddLog("DoctorIndividualProviders",
                   "ProviderNumber",
                   ProviderNumber,
                   doctorIndividualProvider.ProviderNumber,
                   DoctorIndividualProviderId,
                   "Update"));
                ProviderNumber = doctorIndividualProvider.ProviderNumber;
            }
            if (IndividualProviderEffectiveDate != doctorIndividualProvider.IndividualProviderEffectiveDate)
            {
                auditLogs.Add(AuditLog.AddLog("DoctorIndividualProviders",
                    "IndividualProviderEffectiveDate",
                    IndividualProviderEffectiveDate.ToString(),
                    doctorIndividualProvider.IndividualProviderEffectiveDate.ToString(),
                    DoctorIndividualProviderId,
                    "Update"));
                IndividualProviderEffectiveDate = doctorIndividualProvider.IndividualProviderEffectiveDate;
            }

            return auditLogs;
        }
    }
}