using CanoHealth.WebPortal.CommonTools.ExpireLicense;
using CanoHealth.WebPortal.CommonTools.Roles;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Dtos;
using CanoHealth.WebPortal.Services.Email;
using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    [Authorize]
    public class NotificationsController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomEmailService _emailService;
        private readonly IEmailFactory _emailFactory;

        public NotificationsController(IUnitOfWork unitOfWork, ICustomEmailService emailService, IEmailFactory emailFactory)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _emailFactory = emailFactory;
        }

        [HttpGet]
        public IHttpActionResult GetNotifications()
        {
            var result = new List<NotificationDto>();
            if (User.IsInRole(RoleName.Admin) || User.IsInRole(RoleName.Credentialing))
            {
                result = _unitOfWork.ExpireDateNotificationRepository
                    .GetNotifications()
                    .Where(x => !x.Source.Equals(ExpireLicenseSource.OutOfNetworkContracts, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IHttpActionResult> GetExpirationDates()
        {
            try
            {
                //get licenses to expire
                var expireLicenses = _unitOfWork.ExpireDateNotificationRepository
                                .GetNotifications()
                                .ToList();

                //if any build the email tamplate and send it
                if (expireLicenses.Any())
                {
                    var email = _emailFactory.GetExpireLicenseTemplate(expireLicenses);
                    await _emailService.SendSmtpEmailAsync(email);
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
            return Ok();
        }
    }
}