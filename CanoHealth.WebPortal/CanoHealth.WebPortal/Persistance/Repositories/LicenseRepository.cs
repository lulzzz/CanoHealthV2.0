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
    public class LicenseRepository : Repository<PosLicense>, ILicenseRepository
    {
        public LicenseRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<PosLicense> GetActiveLicenses(string placeOfServiceId = null)
        {
            var result = QueryableGetAll(l => l.Active.HasValue && l.Active.Value,
                 includeProperties: new Expression<Func<PosLicense, object>>[] { t => t.LicenseType });

            if (!String.IsNullOrWhiteSpace(placeOfServiceId))
            {
                var guidPlaceOfServiceId = Guid.Parse(placeOfServiceId);
                result = result.Where(l => l.PlaceOfServiceId == guidPlaceOfServiceId);
            }

            return result.ToList();
        }

        public PosLicense GetByLicenseNumber(string licenseNumber)
        {
            return SingleOrDefault(l => l.LicenseNumber.Equals(licenseNumber, StringComparison.InvariantCultureIgnoreCase));
        }

        public PosLicense GetByTypeAndPlaceOfService(Guid typeId, Guid placeOfServiceId)
        {
            return SingleOrDefault(l => l.LicenseTypeId == typeId &&
                                        l.PlaceOfServiceId == placeOfServiceId);
        }

        public IEnumerable<AuditLog> SaveLocationLicenses(IEnumerable<PosLicense> posLicenses)
        {
            var auditLogs = SaveItems(posLicenses,
                (collectionLicense, license) =>
                    collectionLicense.Any(ll => ll.PosLicenseId == license.PosLicenseId));
            return auditLogs;
        }

        private IEnumerable<AuditLog> SaveItems(IEnumerable<PosLicense> posLicenses, Func<DbSet<PosLicense>, PosLicense, bool> existLicense)
        {
            var auditLogs = new List<AuditLog>();

            foreach (var license in posLicenses)
            {
                if (existLicense(Entities, license))
                {
                    var licenseStoredInDb = Get(license.PosLicenseId);
                    if (licenseStoredInDb.LicenseTypeId != license.PosLicenseId)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                            "PosLicenses",
                            "LicenseTypeId",
                            licenseStoredInDb.LicenseTypeId.ToString(),
                            license.LicenseTypeId.ToString(),
                            license.PosLicenseId,
                            "Update"));

                        licenseStoredInDb.LicenseTypeId = license.PosLicenseId;
                    }

                    if (licenseStoredInDb.LicenseNumber != license.LicenseNumber)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                           "PosLicenses",
                           "LicenseNumber",
                           licenseStoredInDb.LicenseNumber,
                           license.LicenseNumber,
                           license.PosLicenseId,
                           "Update"));

                        licenseStoredInDb.LicenseNumber = license.LicenseNumber;
                    }

                    if (licenseStoredInDb.EffectiveDate != license.EffectiveDate)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                          "PosLicenses",
                          "EffectiveDate",
                          licenseStoredInDb.EffectiveDate.ToString(),
                          license.EffectiveDate.ToString(),
                          license.PosLicenseId,
                          "Update"));
                        licenseStoredInDb.EffectiveDate = license.EffectiveDate;
                    }

                    if (licenseStoredInDb.ExpireDate != license.ExpireDate)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                          "PosLicenses",
                          "ExpireDate",
                          licenseStoredInDb.ExpireDate.ToString(),
                          license.ExpireDate.ToString(),
                          license.PosLicenseId,
                          "Update"));
                        licenseStoredInDb.ExpireDate = license.ExpireDate;
                    }

                    if (licenseStoredInDb.Note != license.Note)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                         "PosLicenses",
                         "Note",
                         licenseStoredInDb.Note,
                         license.Note,
                         license.PosLicenseId,
                         "Update"));
                        licenseStoredInDb.Note = license.Note;
                    }

                    if (licenseStoredInDb.OriginalFileName != license.OriginalFileName)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                        "PosLicenses",
                        "OriginalFileName",
                        licenseStoredInDb.OriginalFileName,
                        license.OriginalFileName,
                        license.PosLicenseId,
                        "Update"));
                        licenseStoredInDb.OriginalFileName = license.OriginalFileName;
                    }

                    if (licenseStoredInDb.UniqueFileName != license.UniqueFileName)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                       "PosLicenses",
                       "UniqueFileName",
                       licenseStoredInDb.UniqueFileName,
                       license.UniqueFileName,
                       license.PosLicenseId,
                       "Update"));
                        licenseStoredInDb.UniqueFileName = license.UniqueFileName;
                    }

                    if (licenseStoredInDb.FileExtension != license.FileExtension)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                          "PosLicenses",
                          "FileExtension",
                          licenseStoredInDb.FileExtension,
                          license.FileExtension,
                          license.PosLicenseId,
                          "Update"));
                        licenseStoredInDb.FileExtension = license.FileExtension;
                    }

                    if (licenseStoredInDb.FileSize != license.FileSize)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                         "PosLicenses",
                         "FileSize",
                         licenseStoredInDb.FileSize,
                         license.FileSize,
                         license.PosLicenseId,
                         "Update"));
                        licenseStoredInDb.FileSize = license.FileSize;
                    }

                    if (licenseStoredInDb.ContentType != license.ContentType)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                        "PosLicenses",
                        "ContentType",
                        licenseStoredInDb.ContentType,
                        license.ContentType,
                        license.PosLicenseId,
                        "Update"));
                        licenseStoredInDb.ContentType = license.ContentType;
                    }

                    if (licenseStoredInDb.UploadBy != license.UploadBy)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                       "PosLicenses",
                       "UploadBy",
                       licenseStoredInDb.UploadBy,
                       license.UploadBy,
                       license.PosLicenseId,
                       "Update"));
                        licenseStoredInDb.UploadBy = license.UploadBy;
                    }

                    if (licenseStoredInDb.UploaDateTime != license.UploaDateTime)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                          "PosLicenses",
                          "UploaDateTime",
                          licenseStoredInDb.UploaDateTime.ToString(),
                          license.UploaDateTime.ToString(),
                          license.PosLicenseId,
                          "Update"));
                        licenseStoredInDb.UploaDateTime = license.UploaDateTime;
                    }

                    if (licenseStoredInDb.Active != license.Active)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                         "PosLicenses",
                         "Active",
                         licenseStoredInDb.Active.ToString(),
                         license.Active.ToString(),
                         license.PosLicenseId,
                         "Update"));
                        licenseStoredInDb.Active = license.Active;
                    }
                }
                else
                {
                    Add(license);

                    auditLogs.Add(AuditLog.AddLog("PosLicenses", "Active", null, license.Active.ToString(), license.PosLicenseId, "Insert"));

                    auditLogs.Add(AuditLog.AddLog("PosLicenses", "UploaDateTime", null, license.UploaDateTime.ToString(), license.PosLicenseId, "Insert"));

                    auditLogs.Add(AuditLog.AddLog("PosLicenses", "UploadBy", null, license.UploadBy, license.PosLicenseId, "Insert"));

                    auditLogs.Add(AuditLog.AddLog("PosLicenses", "ContentType", null, license.ContentType, license.PosLicenseId, "Insert"));

                    auditLogs.Add(AuditLog.AddLog("PosLicenses", "FileSize", null, license.FileSize, license.PosLicenseId, "Insert"));

                    auditLogs.Add(AuditLog.AddLog("PosLicenses", "FileExtension", null, license.FileExtension, license.PosLicenseId, "Insert"));

                    auditLogs.Add(AuditLog.AddLog("PosLicenses", "UniqueFileName", null, license.UniqueFileName, license.PosLicenseId, "Insert"));

                    auditLogs.Add(AuditLog.AddLog("PosLicenses", "OriginalFileName", null, license.OriginalFileName, license.PosLicenseId, "Insert"));

                    auditLogs.Add(AuditLog.AddLog("PosLicenses", "Note", null, license.Note, license.PosLicenseId, "Insert"));

                    auditLogs.Add(AuditLog.AddLog("PosLicenses", "ExpireDate", null, license.ExpireDate.ToString(), license.PosLicenseId, "Insert"));

                    auditLogs.Add(AuditLog.AddLog("PosLicenses", "EffectiveDate", null, license.EffectiveDate.ToString(), license.PosLicenseId, "Insert"));

                    auditLogs.Add(AuditLog.AddLog("PosLicenses", "LicenseNumber", null, license.LicenseNumber, license.PosLicenseId, "Insert"));

                    auditLogs.Add(AuditLog.AddLog("PosLicenses", "LicenseTypeId", null, license.LicenseTypeId.ToString(), license.PosLicenseId, "Insert"));
                }
            }
            return auditLogs;
        }
    }
}