using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<ApplicationUser> GetUsers()
        {
            return QueryableGetAll(includeProperties: new Expression<Func<ApplicationUser, object>>[]
            {
                rol => rol.Roles
            }).ToList();
        }

        public ApplicationUser GetByUserName(string username)
        {
            return SingleOrDefault(u => u.Email.Equals(username, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}