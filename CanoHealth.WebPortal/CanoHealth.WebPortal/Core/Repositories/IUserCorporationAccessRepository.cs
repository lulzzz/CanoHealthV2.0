using CanoHealth.WebPortal.Core.Domain;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IUserCorporationAccessRepository : IRepository<UserCorporationAccess>
    {
        IEnumerable<Corporation> GetCorporationAccessByUser(string userId);
    }
}
