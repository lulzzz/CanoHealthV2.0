using CanoHealth.WebPortal.Core.Domain;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface ILineofBusinessRepository : IRepository<PlanType>
    {
        IEnumerable<PlanType> GetBusinessLines();
    }
}
