using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Dtos;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Services.Email
{
    public interface IEmailFactory
    {
        Emails GetExpireLicenseTemplate(IEnumerable<NotificationDto> notifications);
    }
}
