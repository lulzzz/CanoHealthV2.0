using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class UserCorporationAccessRepository : Repository<UserCorporationAccess>,
        IUserCorporationAccessRepository
    {
        public UserCorporationAccessRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<Corporation> GetCorporationAccessByUser(string userId)
        {
            var corporations = EnumarableGetAll(filter: uca => uca.UserId == userId,
                includeProperties: new Expression<Func<UserCorporationAccess, object>>[]
                {
                    corp => corp.Corporation
                })
                .Select(c => c.Corporation)
                .Where(c => c.Active)
                .ToList();

            return corporations;
        }
    }
}