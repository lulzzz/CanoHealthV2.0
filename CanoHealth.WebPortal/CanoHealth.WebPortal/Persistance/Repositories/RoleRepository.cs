using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System.Collections.Generic;
using System.Linq;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class RoleRepository : Repository<ApplicationRole>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<ApplicationRole> GetRoles()
        {
            var roles = EnumarableGetAll().ToList();
            return roles;
        }
    }
}