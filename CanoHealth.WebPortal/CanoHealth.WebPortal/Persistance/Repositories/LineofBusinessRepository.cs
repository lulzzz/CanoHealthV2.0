using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System.Collections.Generic;
using System.Linq;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class LineofBusinessRepository : Repository<PlanType>, ILineofBusinessRepository
    {
        public LineofBusinessRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<PlanType> GetBusinessLines()
        {
            return QueryableGetAll(filter: x => x.Active.HasValue && x.Active.Value,
                orderBy: types => types.OrderBy(x => x.Name)).ToList();
        }
    }
}