using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class DoctorFile
    {
        public Guid DoctorFileId { get; set; }

        public Guid DoctorId { get; set; }

        public Guid DoctorFileTypeId { get; set; }

        [StringLength(100)]
        public string OriginalFileName { get; set; }

        [StringLength(100)]
        public string UniqueFileName { get; set; }

        [StringLength(10)]
        public string FileExtension { get; set; }

        [StringLength(10)]
        public string FileSize { get; set; }

        [StringLength(255)]
        public string ContentType { get; set; }

        public DateTime UploadDateTime { get; set; }

        public string UploadBy { get; set; }

        [StringLength(100)]
        public string ServerLocation { get; set; }

        public bool Active { get; set; }

        //Navegation Properties

        public DoctorFileType DoctorFileType { get; set; }

        public Doctor Doctor { get; set; }

        public AuditLog Inactivate()
        {
            var log = AuditLog.AddLog("DoctorFiles", "Active", "true", "false", DoctorFileId, "Delete");
            Active = false;
            return log;
        }

        public IEnumerable<AuditLog> Modify(DoctorFile file)
        {
            var auditLogs = new List<AuditLog>();

            if (DoctorFileTypeId != file.DoctorFileTypeId)
            {
                auditLogs.Add(AuditLog.AddLog(
                    "DoctorFiles",
                    "DoctorFileTypeId",
                    DoctorFileTypeId.ToString(),
                    file.DoctorFileTypeId.ToString(),
                    DoctorFileId,
                    "Update"));
                DoctorFileTypeId = file.DoctorFileTypeId;
            }

            if (OriginalFileName != file.OriginalFileName)
            {
                auditLogs.Add(AuditLog.AddLog(
                    "DoctorFiles",
                    "OriginalFileName",
                    OriginalFileName,
                    file.OriginalFileName,
                    file.DoctorFileId,
                    "Update"));
                OriginalFileName = file.OriginalFileName;
            }

            if (UniqueFileName != file.UniqueFileName)
            {
                auditLogs.Add(AuditLog.AddLog(
                   "DoctorFiles",
                   "UniqueFileName",
                   UniqueFileName,
                   file.UniqueFileName,
                   file.DoctorFileId,
                   "Update"));
                UniqueFileName = file.UniqueFileName;
            }

            if (FileExtension != file.FileExtension)
            {
                auditLogs.Add(AuditLog.AddLog(
                   "DoctorFiles",
                   "FileExtension",
                   FileExtension,
                   file.FileExtension,
                   file.DoctorFileId,
                   "Update"));
                FileExtension = file.FileExtension;
            }

            if (FileSize != file.FileSize)
            {
                auditLogs.Add(AuditLog.AddLog(
                   "DoctorFiles",
                   "FileSize",
                   FileSize,
                   file.FileSize,
                   file.DoctorFileId,
                   "Update"));
                FileSize = file.FileSize;
            }

            if (ContentType != file.ContentType)
            {
                auditLogs.Add(AuditLog.AddLog(
                  "DoctorFiles",
                  "ContentType",
                  ContentType,
                  file.ContentType,
                  file.DoctorFileId,
                  "Update"));
                ContentType = file.ContentType;
            }

            if (UploadDateTime != file.UploadDateTime)
            {
                auditLogs.Add(AuditLog.AddLog(
                  "DoctorFiles",
                  "UploadDateTime",
                  UploadDateTime.ToString(),
                  file.UploadDateTime.ToString(),
                  file.DoctorFileId,
                  "Update"));
                UploadDateTime = file.UploadDateTime;
            }

            if (UploadBy != file.UploadBy)
            {
                auditLogs.Add(AuditLog.AddLog(
                 "DoctorFiles",
                 "UploadBy",
                 UploadBy,
                 file.UploadBy,
                 file.DoctorFileId,
                 "Update"));
                UploadBy = file.UploadBy;
            }

            if (Active != file.Active)
            {
                auditLogs.Add(AuditLog.AddLog(
                "DoctorFiles",
                "Active",
                Active.ToString(),
                file.Active.ToString(),
                file.DoctorFileId,
                "Update"));
                Active = file.Active;
            }

            return auditLogs;
        }
    }
}