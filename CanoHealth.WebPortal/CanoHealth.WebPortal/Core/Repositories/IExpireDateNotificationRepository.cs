using CanoHealth.WebPortal.Core.Dtos;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IExpireDateNotificationRepository : IRepository<NotificationDto>
    {
        IEnumerable<NotificationDto> GetNotifications();
    }
}
