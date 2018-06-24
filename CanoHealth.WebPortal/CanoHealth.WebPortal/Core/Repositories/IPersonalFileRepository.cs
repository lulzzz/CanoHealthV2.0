using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IPersonalFileRepository : IRepository<DoctorFile>
    {
        IEnumerable<DoctorFile> GetActivePersonalFiles(Guid? doctorId);

        DoctorFile GetDoctorFileByType(Guid doctorId, Guid doctorFileTypeId, Guid? doctorFileId = null);

        IEnumerable<AuditLog> SavePersonalFiles(IEnumerable<DoctorFile> doctorFiles);
    }
}