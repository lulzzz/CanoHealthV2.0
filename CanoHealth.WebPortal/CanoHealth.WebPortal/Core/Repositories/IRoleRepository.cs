using CanoHealth.WebPortal.Core.Domain;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IRoleRepository : IRepository<ApplicationRole>
    {
        IEnumerable<ApplicationRole> GetRoles();
    }
}
