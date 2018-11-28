using AutoMapper;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Dtos;
using Elmah;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    public class ProviderByLocationsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProviderByLocationsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult GetActiveProvidersByLocation(
            [DataSourceRequest] DataSourceRequest request,
            Guid doctorCorporationContractLinkId)
        {
            var locationProviders = _unitOfWork.ProviderByLocationRepository
                .GetActiveProvidersByLocation(doctorCorporationContractLinkId)
                .ToList();

            var result = (from a in locationProviders
                          join b in _unitOfWork.PlaceOfServices.GetAll()
                              on a.PlaceOfServiceId equals b.PlaceOfServiceId
                          select (new ProviderByLocationDto
                          {
                              ProviderByLocationId = a.ProviderByLocationId,
                              DoctorCorporationContractLinkId = a.DoctorCorporationContractLinkId,
                              PlaceOfServiceId = a.PlaceOfServiceId,
                              PlaceOfServiceName = b.Name,
                              LocacionProviderNumber = a.LocacionProviderNumber,
                              ProviderEffectiveDate = a.ProviderEffectiveDate,
                              Active = a.Active
                          })).ToList();

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateProvider([DataSourceRequest] DataSourceRequest request,
            ProviderByLocationDto providerByLocationDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var providerStoredInDb = _unitOfWork.ProviderByLocationRepository
                        .Get(providerByLocationDto.ProviderByLocationId);
                    if (providerStoredInDb == null)
                    {
                        ModelState.AddModelError("", "This item was not found in our system, please try again.");
                        return Json(new[] { providerByLocationDto }.ToDataSourceResult(request, ModelState));
                    }

                    var updateProviderLogs = providerStoredInDb.Modify(Mapper.Map(providerByLocationDto, new ProviderByLocation()));
                    _unitOfWork.AuditLogs.AddRange(updateProviderLogs);
                    _unitOfWork.Complete();
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    ModelState.AddModelError("", "We are sorry, but somenthing went wrong. Please try again!");
                }
            }
            return Json(new[] { providerByLocationDto }.ToDataSourceResult(request, ModelState));
        }
    }
}