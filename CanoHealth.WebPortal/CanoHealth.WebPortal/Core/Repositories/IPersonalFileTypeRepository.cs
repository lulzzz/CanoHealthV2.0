using CanoHealth.WebPortal.Core.Domain;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IPersonalFileTypeRepository : IRepository<DoctorFileType>
    {
        IEnumerable<DoctorFileType> GetFileTypes();

        DoctorFileType GetFileTypeByName(string name);

        IEnumerable<AuditLog> SaveFileTypes(IEnumerable<DoctorFileType> doctorFileTypes);
    }
}
