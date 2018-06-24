using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class PersonalFileTypeRepository : Repository<DoctorFileType>, IPersonalFileTypeRepository
    {
        public PersonalFileTypeRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<DoctorFileType> GetFileTypes()
        {
            return EnumarableGetAll();
        }

        public DoctorFileType GetFileTypeByName(string name)
        {
            return SingleOrDefault(dft => dft.DoctorFileTypeName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }

        public IEnumerable<AuditLog> SaveFileTypes(IEnumerable<DoctorFileType> doctorFileTypes)
        {
            var logs = SaveItems(doctorFileTypes,
                (collectionFileTypes, fileType) =>
                    collectionFileTypes.Any(cft => cft.DoctorFileTypeId == fileType.DoctorFileTypeId)
            );

            return logs;
        }

        private IEnumerable<AuditLog> SaveItems(IEnumerable<DoctorFileType> doctorFileTypes, Func<DbSet<DoctorFileType>, DoctorFileType, bool> existFileType)
        {
            var logs = new List<AuditLog>();

            foreach (var doctorFileType in doctorFileTypes)
            {
                if (existFileType(Entities, doctorFileType))
                {
                    var fileTypeStoreInDb = Get(doctorFileType.DoctorFileTypeId);
                    if (fileTypeStoreInDb.DoctorFileTypeName != doctorFileType.DoctorFileTypeName)
                    {
                        logs.Add(AuditLog.AddLog("DoctorFileTypes", "DoctorFileTypeName", fileTypeStoreInDb.DoctorFileTypeName, doctorFileType.DoctorFileTypeName, fileTypeStoreInDb.DoctorFileTypeId, "Update"));
                        fileTypeStoreInDb.DoctorFileTypeName = doctorFileType.DoctorFileTypeName;
                    }
                }
                else
                {
                    Add(doctorFileType);
                    logs.Add(AuditLog.AddLog("DoctorFileTypes", "DoctorFileTypeName", null, doctorFileType.DoctorFileTypeName, doctorFileType.DoctorFileTypeId, "Insert"));
                }
            }
            return logs;
        }
    }
}