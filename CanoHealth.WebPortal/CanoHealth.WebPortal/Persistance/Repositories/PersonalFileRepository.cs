using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class PersonalFileRepository : Repository<DoctorFile>, IPersonalFileRepository
    {
        public PersonalFileRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<DoctorFile> GetActivePersonalFiles(Guid? doctorId)
        {
            var activePersonalFiles = QueryableGetAll(ml => ml.Active,
                includeProperties: new Expression<Func<DoctorFile, object>>[]
                {
                    t => t.DoctorFileType
                });
            if (doctorId != null)
                activePersonalFiles = activePersonalFiles.Where(ml => ml.DoctorId == doctorId);
            return activePersonalFiles;
        }

        public DoctorFile GetDoctorFileByType(Guid doctorId, Guid doctorFileTypeId, Guid? doctorFileId = null)
        {
            if (doctorFileId != null)
                return SingleOrDefault(df => df.DoctorId == doctorId &&
                             df.DoctorFileTypeId == doctorFileTypeId &&
                             df.DoctorFileId != doctorFileId);
            return SingleOrDefault(df => df.DoctorId == doctorId && df.DoctorFileTypeId == doctorFileTypeId);
        }

        public IEnumerable<AuditLog> SavePersonalFiles(IEnumerable<DoctorFile> doctorFiles)
        {
            var auditLogs = SaveItems(doctorFiles, (fileCollection, file) => fileCollection.Any(df => df.DoctorFileId == file.DoctorFileId));
            return auditLogs;
        }

        private IEnumerable<AuditLog> SaveItems(IEnumerable<DoctorFile> doctorFiles, Func<DbSet<DoctorFile>, DoctorFile, bool> existFile)
        {
            var auditLogs = new List<AuditLog>();
            foreach (var file in doctorFiles)
            {
                if (existFile(Entities, file))
                {
                    var doctorFileStoredInDb = Get(file.DoctorFileId);

                    if (doctorFileStoredInDb.DoctorFileTypeId != file.DoctorFileTypeId)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                            "DoctorFiles",
                            "DoctorFileTypeId",
                            doctorFileStoredInDb.DoctorFileTypeId.ToString(),
                            file.DoctorFileTypeId.ToString(),
                            file.DoctorFileId,
                            "Update"));
                        doctorFileStoredInDb.DoctorFileTypeId = file.DoctorFileTypeId;
                    }

                    if (doctorFileStoredInDb.OriginalFileName != file.OriginalFileName)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                            "DoctorFiles",
                            "OriginalFileName",
                            doctorFileStoredInDb.OriginalFileName,
                            file.OriginalFileName,
                            file.DoctorFileId,
                            "Update"));
                        doctorFileStoredInDb.OriginalFileName = file.OriginalFileName;
                    }

                    if (doctorFileStoredInDb.UniqueFileName != file.UniqueFileName)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                           "DoctorFiles",
                           "UniqueFileName",
                           doctorFileStoredInDb.UniqueFileName,
                           file.UniqueFileName,
                           file.DoctorFileId,
                           "Update"));
                        doctorFileStoredInDb.UniqueFileName = file.UniqueFileName;
                    }

                    if (doctorFileStoredInDb.FileExtension != file.FileExtension)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                           "DoctorFiles",
                           "FileExtension",
                           doctorFileStoredInDb.FileExtension,
                           file.FileExtension,
                           file.DoctorFileId,
                           "Update"));
                        doctorFileStoredInDb.FileExtension = file.FileExtension;
                    }

                    if (doctorFileStoredInDb.FileSize != file.FileSize)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                           "DoctorFiles",
                           "FileSize",
                           doctorFileStoredInDb.FileSize,
                           file.FileSize,
                           file.DoctorFileId,
                           "Update"));
                        doctorFileStoredInDb.FileSize = file.FileSize;
                    }

                    if (doctorFileStoredInDb.ContentType != file.ContentType)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                          "DoctorFiles",
                          "ContentType",
                          doctorFileStoredInDb.ContentType,
                          file.ContentType,
                          file.DoctorFileId,
                          "Update"));
                        doctorFileStoredInDb.ContentType = file.ContentType;
                    }

                    if (doctorFileStoredInDb.UploadDateTime != file.UploadDateTime)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                          "DoctorFiles",
                          "UploadDateTime",
                          doctorFileStoredInDb.UploadDateTime.ToString(),
                          file.UploadDateTime.ToString(),
                          file.DoctorFileId,
                          "Update"));
                        doctorFileStoredInDb.UploadDateTime = file.UploadDateTime;
                    }

                    if (doctorFileStoredInDb.UploadBy != file.UploadBy)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                         "DoctorFiles",
                         "UploadBy",
                         doctorFileStoredInDb.UploadBy,
                         file.UploadBy,
                         file.DoctorFileId,
                         "Update"));
                        doctorFileStoredInDb.UploadBy = file.UploadBy;
                    }

                    if (doctorFileStoredInDb.Active != file.Active)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                        "DoctorFiles",
                        "Active",
                        doctorFileStoredInDb.Active.ToString(),
                        file.Active.ToString(),
                        file.DoctorFileId,
                        "Update"));
                        doctorFileStoredInDb.Active = file.Active;
                    }
                }
                else
                {
                    Add(file);
                    auditLogs.Add(AuditLog.AddLog(
                           "DoctorFiles",
                           "DoctorFileTypeId",
                           null,
                           file.DoctorFileTypeId.ToString(),
                           file.DoctorFileId,
                           "Insert"));

                    auditLogs.Add(AuditLog.AddLog(
                           "DoctorFiles",
                           "OriginalFileName",
                           null,
                           file.OriginalFileName,
                           file.DoctorFileId,
                           "Insert"));

                    auditLogs.Add(AuditLog.AddLog(
                          "DoctorFiles",
                          "UniqueFileName",
                          null,
                          file.UniqueFileName,
                          file.DoctorFileId,
                          "Insert"));

                    auditLogs.Add(AuditLog.AddLog(
                           "DoctorFiles",
                           "FileExtension",
                           null,
                           file.FileExtension,
                           file.DoctorFileId,
                           "Insert"));

                    auditLogs.Add(AuditLog.AddLog(
                           "DoctorFiles",
                           "FileSize",
                           null,
                           file.FileSize,
                           file.DoctorFileId,
                           "Insert"));

                    auditLogs.Add(AuditLog.AddLog(
                         "DoctorFiles",
                         "ContentType",
                         null,
                         file.ContentType,
                         file.DoctorFileId,
                         "Insert"));

                    auditLogs.Add(AuditLog.AddLog(
                          "DoctorFiles",
                          "UploadDateTime",
                          null,
                          file.UploadDateTime.ToString(),
                          file.DoctorFileId,
                          "Insert"));

                    auditLogs.Add(AuditLog.AddLog(
                        "DoctorFiles",
                        "UploadBy",
                        null,
                        file.UploadBy,
                        file.DoctorFileId,
                        "Insert"));

                    auditLogs.Add(AuditLog.AddLog(
                       "DoctorFiles",
                       "Active",
                       null,
                       file.Active.ToString(),
                       file.DoctorFileId,
                       "Insert"));
                }
            }

            return auditLogs;
        }
    }
}