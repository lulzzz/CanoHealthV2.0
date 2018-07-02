using CanoHealth.WebPortal.Core.Dtos;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System.Collections.Generic;
using System.Linq;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class ExpireDateNotificationRepository : Repository<NotificationDto>, IExpireDateNotificationRepository
    {
        public ExpireDateNotificationRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<NotificationDto> GetNotifications()
        {
            var query = "EXEC [dbo].[GetLicenseExpirationDates]";
            var result = GetWithRawSqlForTypesAreNotEntities(query).ToList();
            return result;
        }
    }
}