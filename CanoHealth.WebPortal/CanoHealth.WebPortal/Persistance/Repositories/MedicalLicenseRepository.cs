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
    public class MedicalLicenseRepository : Repository<MedicalLicense>, IMedicalLicenseRepository
    {
        public MedicalLicenseRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<MedicalLicense> GetActiveMedicalLicenses(Guid? doctorId)
        {
            var result = QueryableGetAll(ml => ml.Active.HasValue && ml.Active == true,
                includeProperties: new Expression<Func<MedicalLicense, object>>[] { t => t.LicenseType });
            if (doctorId != null)
                result = result.Where(ml => ml.DoctorId == doctorId);
            return result;
        }

        public MedicalLicense GetLicenseByNumber(string licenseNumber, Guid? medicalLicenseId = null)
        {
            if (medicalLicenseId != null)
                return SingleOrDefault(ml => ml.LicenseNumber.Equals(licenseNumber, StringComparison.InvariantCultureIgnoreCase) && ml.MedicalLicenseId != medicalLicenseId);
            return SingleOrDefault(ml => ml.LicenseNumber.Equals(licenseNumber, StringComparison.InvariantCultureIgnoreCase));
        }

        public MedicalLicense GetLicenseByDoctorAndType(Guid doctorId,
            Guid medicalLicenseTypeId,
            Guid? medicalLicenseId = null)
        {
            if (medicalLicenseId != null)
                return SingleOrDefault(ml => ml.DoctorId == doctorId && ml.MedicalLicenseTypeId == medicalLicenseTypeId && ml.MedicalLicenseId != medicalLicenseId);
            return SingleOrDefault(ml => ml.DoctorId == doctorId && ml.MedicalLicenseTypeId == medicalLicenseTypeId);
        }

        public IEnumerable<AuditLog> SaveMedicalLicenses(IEnumerable<MedicalLicense> licenseToStore)
        {
            return SaveItems(licenseToStore, (collection, item) => collection.Any(x => x.MedicalLicenseId == item.MedicalLicenseId));
        }

        private IEnumerable<AuditLog> SaveItems(IEnumerable<MedicalLicense> licenseToStore,
            Func<DbSet<MedicalLicense>, MedicalLicense, bool> existMedicalLicense)
        {
            var auditLogs = new List<AuditLog>();
            foreach (var license in licenseToStore)
            {
                if (existMedicalLicense(Entities, license))
                {
                    var medicalLicenseStoredInDb = Get(license.MedicalLicenseId);
                    if (medicalLicenseStoredInDb.DoctorId != license.DoctorId)
                    {
                        auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "DoctorId", medicalLicenseStoredInDb.DoctorId.ToString(), license.DoctorId.ToString(), license.MedicalLicenseId, "Update"));
                        medicalLicenseStoredInDb.DoctorId = license.DoctorId;
                    }
                    if (medicalLicenseStoredInDb.MedicalLicenseTypeId != license.MedicalLicenseTypeId)
                    {
                        auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "MedicalLicenseTypeId", medicalLicenseStoredInDb.MedicalLicenseTypeId.ToString(), license.MedicalLicenseTypeId.ToString(), license.MedicalLicenseId, "Update"));
                        medicalLicenseStoredInDb.MedicalLicenseTypeId = license.MedicalLicenseTypeId;
                    }
                    if (medicalLicenseStoredInDb.LicenseNumber != license.LicenseNumber)
                    {
                        auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "LicenseNumber", medicalLicenseStoredInDb.LicenseNumber, license.LicenseNumber, license.MedicalLicenseId, "Update"));
                        medicalLicenseStoredInDb.LicenseNumber = license.LicenseNumber;
                    }
                    if (medicalLicenseStoredInDb.EffectiveDate != license.EffectiveDate)
                    {
                        auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "EffectiveDate", medicalLicenseStoredInDb.EffectiveDate.ToString(), license.EffectiveDate.ToString(), license.MedicalLicenseId, "Update"));
                        medicalLicenseStoredInDb.EffectiveDate = license.EffectiveDate;
                    }
                    if (medicalLicenseStoredInDb.ExpireDate != license.ExpireDate)
                    {
                        auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "ExpireDate", medicalLicenseStoredInDb.ExpireDate.ToString(), license.ExpireDate.ToString(), license.MedicalLicenseId, "Update"));
                        medicalLicenseStoredInDb.ExpireDate = license.ExpireDate;
                    }
                    if (medicalLicenseStoredInDb.Note != license.Note)
                    {
                        auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "Note", medicalLicenseStoredInDb.Note, license.Note, license.MedicalLicenseId, "Update"));
                        medicalLicenseStoredInDb.Note = license.Note;
                    }
                    if (medicalLicenseStoredInDb.ServerLocation != license.ServerLocation)
                    {
                        auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "ServerLocation", medicalLicenseStoredInDb.ServerLocation, license.ServerLocation, license.MedicalLicenseId, "Update"));
                        medicalLicenseStoredInDb.ServerLocation = license.ServerLocation;
                    }
                    if (medicalLicenseStoredInDb.OriginalFileName != license.OriginalFileName)
                    {
                        auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "OriginalFileName", medicalLicenseStoredInDb.OriginalFileName, license.OriginalFileName, license.MedicalLicenseId, "Update"));
                        medicalLicenseStoredInDb.OriginalFileName = license.OriginalFileName;
                    }
                    if (medicalLicenseStoredInDb.UniqueFileName != license.UniqueFileName)
                    {
                        auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "UniqueFileName", medicalLicenseStoredInDb.UniqueFileName, license.UniqueFileName, license.MedicalLicenseId, "Update"));
                        medicalLicenseStoredInDb.UniqueFileName = license.UniqueFileName;
                    }
                    if (medicalLicenseStoredInDb.FileExtension != license.FileExtension)
                    {
                        auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "FileExtension", medicalLicenseStoredInDb.FileExtension, license.FileExtension, license.MedicalLicenseId, "Update"));
                        medicalLicenseStoredInDb.FileExtension = license.FileExtension;
                    }
                    if (medicalLicenseStoredInDb.FileSize != license.FileSize)
                    {
                        auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "FileSize", medicalLicenseStoredInDb.FileSize, license.FileSize, license.MedicalLicenseId, "Update"));
                        medicalLicenseStoredInDb.FileSize = license.FileSize;
                    }
                    if (medicalLicenseStoredInDb.ContentType != license.ContentType)
                    {
                        auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "ContentType", medicalLicenseStoredInDb.ContentType, license.ContentType, license.MedicalLicenseId, "Update"));
                        medicalLicenseStoredInDb.ContentType = license.ContentType;
                    }
                    if (medicalLicenseStoredInDb.UploadBy != license.UploadBy)
                    {
                        auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "UploadBy", medicalLicenseStoredInDb.UploadBy, license.UploadBy, license.MedicalLicenseId, "Update"));
                        medicalLicenseStoredInDb.UploadBy = license.UploadBy;
                    }
                    if (medicalLicenseStoredInDb.UploaDateTime != license.UploaDateTime)
                    {
                        auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "UploaDateTime", medicalLicenseStoredInDb.UploaDateTime.ToString(), license.UploaDateTime.ToString(), license.MedicalLicenseId, "Update"));
                        medicalLicenseStoredInDb.UploaDateTime = license.UploaDateTime;
                    }
                    if (medicalLicenseStoredInDb.Active != license.Active)
                    {
                        auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "Active", medicalLicenseStoredInDb.Active.ToString(), license.Active.ToString(), license.MedicalLicenseId, "Update"));
                        medicalLicenseStoredInDb.Active = license.Active;
                    }
                }
                else
                {
                    Add(license);

                    auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "DoctorId", null, license.DoctorId.ToString(), license.MedicalLicenseId, "Insert"));
                    auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "MedicalLicenseTypeId", null, license.MedicalLicenseTypeId.ToString(), license.MedicalLicenseId, "Insert"));
                    auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "LicenseNumber", null, license.LicenseNumber, license.MedicalLicenseId, "Insert"));
                    auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "EffectiveDate", null, license.EffectiveDate.ToString(), license.MedicalLicenseId, "Insert"));
                    auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "ExpireDate", null, license.ExpireDate.ToString(), license.MedicalLicenseId, "Insert"));
                    auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "Note", null, license.Note, license.MedicalLicenseId, "Insert"));
                    auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "ServerLocation", null, license.ServerLocation, license.MedicalLicenseId, "Insert"));
                    auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "OriginalFileName", null, license.OriginalFileName, license.MedicalLicenseId, "Insert"));
                    auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "UniqueFileName", null, license.UniqueFileName, license.MedicalLicenseId, "Insert"));
                    auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "FileExtension", null, license.FileExtension, license.MedicalLicenseId, "Insert"));
                    auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "FileSize", null, license.FileSize, license.MedicalLicenseId, "Insert"));
                    auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "ContentType", null, license.ContentType, license.MedicalLicenseId, "Insert"));
                    auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "UploadBy", null, license.UploadBy, license.MedicalLicenseId, "Insert"));
                    auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "UploadBy", null, license.UploaDateTime.ToString(), license.MedicalLicenseId, "Insert"));
                    auditLogs.Add(AuditLog.AddLog("MedicalLicenses", "UploadBy", null, license.Active.ToString(), license.MedicalLicenseId, "Insert"));
                }
            }
            return auditLogs;
        }
    }
}