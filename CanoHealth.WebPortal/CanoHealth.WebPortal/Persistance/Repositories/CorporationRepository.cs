using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class CorporationRepository : Repository<Corporation>, ICorporationRepository
    {
        public CorporationRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<Corporation> GetActiveCorporations()
        {
            return EnumarableGetAll(c => c.Active, orderBy: iqob => iqob.OrderBy(c => c.CorporationName));
        }

        public void SaveCorporations(IEnumerable<Corporation> corporations)
        {
            SaveItem(corporations, (corporationList, corporation) => corporationList
                .Any(c => c.CorporationId == corporation.CorporationId));
        }

        private void SaveItem(IEnumerable<Corporation> corporations, Func<DbSet<Corporation>, Corporation, bool> expression)
        {
            foreach (var corporation in corporations)
            {
                if (expression(Entities, corporation))
                    UpdateByGeneric(corporation);
                else
                    Add(corporation);
            }
        }
    }
}