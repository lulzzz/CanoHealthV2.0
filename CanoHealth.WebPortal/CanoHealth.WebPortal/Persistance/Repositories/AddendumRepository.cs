using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class AddendumRepository : Repository<ContractAddendum>, IAddendumRepository
    {

        public AddendumRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<ContractAddendum> GetActiveAddendums(string contractId = null)
        {
            var iqueryableresult = QueryableGetAll(filter: ca => ca.Active, orderBy: io => io.OrderByDescending(a => a.UploadDateTime));
            if (!String.IsNullOrEmpty(contractId))
            {
                var contractIdGuid = Guid.Parse(contractId);
                iqueryableresult = iqueryableresult.Where(a => a.ContractId == contractIdGuid);
            }

            return iqueryableresult.ToList();
        }

        public IEnumerable<AuditLog> SaveAddendums(IEnumerable<ContractAddendum> addendums)
        {
            return SaveItems(addendums, (addendumCollection, addendum) =>
            addendumCollection.Any(a => a.ContractAddendumId == addendum.ContractAddendumId));
        }

        private IEnumerable<AuditLog> SaveItems(IEnumerable<ContractAddendum> addendums, Func<DbSet<ContractAddendum>, ContractAddendum, bool> existAddendum)
        {
            var auditLogs = new List<AuditLog>();
            foreach (var addendum in addendums)
            {
                if (existAddendum(Entities, addendum))
                {
                    var addendumStoredInDb = Get(addendum.ContractAddendumId);
                    auditLogs.AddRange(addendumStoredInDb.ModifyAddendum(addendum));
                }
                else
                {
                    Add(addendum);
                    auditLogs.AddRange(new List<AuditLog>
                    {
                        AuditLog.AddLog("ContractAddendums", "ContractId",null,addendum.ContractId.ToString(),addendum.ContractAddendumId,"Create"),
                        AuditLog.AddLog("ContractAddendums", "UniqueFileName",null,addendum.UniqueFileName,addendum.ContractAddendumId,"Create"),
                        AuditLog.AddLog("ContractAddendums", "OriginalFileName",null,addendum.OriginalFileName,addendum.ContractAddendumId,"Create"),
                        AuditLog.AddLog("ContractAddendums", "FileExtension",null,addendum.FileExtension,addendum.ContractAddendumId,"Create"),
                        AuditLog.AddLog("ContractAddendums", "FileSize",null,addendum.FileSize,addendum.ContractAddendumId,"Create"),
                        AuditLog.AddLog("ContractAddendums", "ContentType",null,addendum.ContentType,addendum.ContractAddendumId,"Create"),
                        AuditLog.AddLog("ContractAddendums", "ServerLocation",null,addendum.ServerLocation,addendum.ContractAddendumId,"Create"),
                        AuditLog.AddLog("ContractAddendums", "UploadDateTime",null,addendum.UploadDateTime.ToString(CultureInfo.InvariantCulture),addendum.ContractAddendumId,"Create"),
                        AuditLog.AddLog("ContractAddendums", "UploadBy",null,addendum.UploadBy,addendum.ContractAddendumId,"Create"),
                        AuditLog.AddLog("ContractAddendums", "Description",null,addendum.Description,addendum.ContractAddendumId,"Create"),
                        AuditLog.AddLog("ContractAddendums", "Active",null,addendum.Active.ToString(),addendum.ContractAddendumId,"Create")
                    });
                }
            }
            return auditLogs;
        }
    }
}