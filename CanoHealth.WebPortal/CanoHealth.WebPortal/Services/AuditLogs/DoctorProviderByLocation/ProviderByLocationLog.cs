using CanoHealth.WebPortal.CommonTools.CurrentDateTime;
using CanoHealth.WebPortal.CommonTools.User;
using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Services.AuditLogs.DoctorProviderByLocation
{
    public class ProviderByLocationLog : IProviderByLocationLog
    {
        private readonly IUserService _user;
        private readonly ICurrentDateTimeService _date;

        public ProviderByLocationLog(IUserService user, ICurrentDateTimeService date)
        {
            _user = user;
            _date = date;
        }

        public IEnumerable<AuditLog> GenerateLogsWhenDelete(IEnumerable<ProviderByLocation> providerByLocations)
        {
            var auditLogs = new List<AuditLog>();
            foreach (var provider in providerByLocations)
            {
                auditLogs.AddRange(new List<AuditLog>
                {
                    new AuditLog{
                        TableName = "ProviderByLocations",
                        ColumnName = "DoctorCorporationContractLinkId",
                        OldValue = provider.DoctorCorporationContractLinkId.ToString(),
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime(),
                        AuditAction = "Delete",
                        ObjectId = provider.ProviderByLocationId
                    },
                    new AuditLog{
                        TableName = "ProviderByLocations",
                        ColumnName = "PlaceOfServiceId",
                        OldValue = provider.PlaceOfServiceId.ToString(),
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime(),
                        AuditAction = "Delete",
                        ObjectId = provider.ProviderByLocationId
                    },
                    new AuditLog{
                        TableName = "ProviderByLocations",
                        ColumnName = "Active",
                        OldValue = provider.Active.ToString(),
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime(),
                        AuditAction = "Delete",
                        ObjectId = provider.ProviderByLocationId
                    }
                });

                if (provider.ProviderEffectiveDate != null)
                {
                    auditLogs.Add(new AuditLog
                    {
                        TableName = "ProviderByLocations",
                        ColumnName = "ProviderEffectiveDate",
                        OldValue = provider.ProviderEffectiveDate.ToString(),
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime(),
                        AuditAction = "Delete",
                        ObjectId = provider.ProviderByLocationId
                    });
                }

                if (!String.IsNullOrEmpty(provider.LocacionProviderNumber))
                {
                    auditLogs.Add(new AuditLog
                    {
                        TableName = "ProviderByLocations",
                        ColumnName = "LocacionProviderNumber",
                        OldValue = provider.LocacionProviderNumber.ToString(),
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime(),
                        AuditAction = "Delete",
                        ObjectId = provider.ProviderByLocationId
                    });
                }
            }
            return auditLogs;
        }
    }
}