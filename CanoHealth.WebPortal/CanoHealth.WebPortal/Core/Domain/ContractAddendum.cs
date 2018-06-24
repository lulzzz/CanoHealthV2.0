using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class ContractAddendum
    {
        public Guid ContractAddendumId { get; set; }

        public Guid ContractId { get; set; }

        [StringLength(100)]
        public string UniqueFileName { get; set; }

        [StringLength(100)]
        public string OriginalFileName { get; set; }

        [StringLength(10)]
        public string FileExtension { get; set; }

        [StringLength(10)]
        public string FileSize { get; set; }

        [StringLength(255)]
        public string ContentType { get; set; }

        [StringLength(100)]
        public string ServerLocation { get; set; }

        public DateTime UploadDateTime { get; set; }

        public string UploadBy { get; set; }

        [StringLength(3000)]
        public string Description { get; set; }

        public bool Active { get; set; }

        public IEnumerable<AuditLog> ModifyAddendum(ContractAddendum addendum)
        {
            var auditLogs = new List<AuditLog>();
            if (ContractId != addendum.ContractId)
            {
                auditLogs.Add(AuditLog.AddLog("ContractAddendums", "ContractId", ContractId.ToString(), addendum.ContractId.ToString(),
                    ContractAddendumId, "Update"));
                ContractId = addendum.ContractId;
            }
            if (UniqueFileName != addendum.UniqueFileName)
            {
                auditLogs.Add(AuditLog.AddLog("ContractAddendums", "UniqueFileName", UniqueFileName, addendum.UniqueFileName, ContractAddendumId, "Update"));
                UniqueFileName = addendum.UniqueFileName;
            }
            if (OriginalFileName != addendum.OriginalFileName)
            {
                auditLogs.Add(AuditLog.AddLog("ContractAddendums", "OriginalFileName", OriginalFileName, addendum.OriginalFileName, ContractAddendumId, "Update"));
                OriginalFileName = addendum.OriginalFileName;
            }
            if (FileExtension != addendum.FileExtension)
            {
                auditLogs.Add(AuditLog.AddLog("ContractAddendums", "FileExtension", FileExtension, addendum.FileExtension, ContractAddendumId, "Update"));
                FileExtension = addendum.FileExtension;
            }
            if (FileSize != addendum.FileSize)
            {
                auditLogs.Add(AuditLog.AddLog("ContractAddendums", "FileSize", FileSize, addendum.FileSize, ContractAddendumId, "Update"));
                FileSize = addendum.FileSize;
            }
            if (ContentType != addendum.ContentType)
            {
                auditLogs.Add(AuditLog.AddLog("ContractAddendums", "ContentType", ContentType, addendum.ContentType, ContractAddendumId, "Update"));
                ContentType = addendum.ContentType;
            }
            if (ServerLocation != addendum.ServerLocation)
            {
                auditLogs.Add(AuditLog.AddLog("ContractAddendums", "ServerLocation", ServerLocation, addendum.ServerLocation, ContractAddendumId, "Update"));
                ServerLocation = addendum.ServerLocation;
            }
            if (UploadDateTime != addendum.UploadDateTime)
            {
                auditLogs.Add(AuditLog.AddLog("ContractAddendums", "UploadDateTime", UploadDateTime.ToString(), addendum.UploadDateTime.ToString(), ContractAddendumId, "Update"));
                UploadDateTime = addendum.UploadDateTime;
            }
            if (UploadBy != addendum.UploadBy)
            {
                auditLogs.Add(AuditLog.AddLog("ContractAddendums", "UploadBy", UploadBy, addendum.UploadBy, ContractAddendumId, "Update"));
                UploadBy = addendum.UploadBy;
            }
            if (Description != addendum.Description)
            {
                auditLogs.Add(AuditLog.AddLog("ContractAddendums", "Description", Description, addendum.Description, ContractAddendumId, "Update"));
                Description = addendum.Description;
            }
            if (Active != addendum.Active)
            {
                auditLogs.Add(AuditLog.AddLog("ContractAddendums", "Active", Active.ToString(), addendum.Active.ToString(), ContractAddendumId, "Update"));
                Active = addendum.Active;
            }
            return auditLogs;
        }

        public AuditLog InactiveLicense()
        {
            var auditlog = AuditLog.AddLog("ContractAddendums", "Active", Active.ToString(), false.ToString(), ContractAddendumId, "Update");
            Active = false;
            return auditlog;
        }
    }
}