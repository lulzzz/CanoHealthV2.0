using CanoHealth.WindowsService.Persistance;
using System.Collections.Generic;
using System.Linq;

namespace CanoHealth.WindowsService.BLL
{
    public class ExpirationDates
    {
        private static CanoHealthEntities db = new CanoHealthEntities();

        public static IEnumerable<GetLicenseExpirationDates_Result> GetLicenseExpirationDates()
        {
            using (var db = new CanoHealthEntities())
            {
                return db.GetLicenseExpirationDates().ToList();
            }
        }
    }
}
