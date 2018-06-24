using CanoHealth.WebPortal.Core.Domain;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface ICorporationRepository : IRepository<Corporation>
    {
        void SaveCorporations(IEnumerable<Corporation> corporations);

        IEnumerable<Corporation> GetActiveCorporations();
    }
}
