using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Dtos;
using Elmah;
using System;
using System.Linq;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class PlaceOfServicesController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlaceOfServicesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult GetPlaceOfService(string placeOfServiceId)
        {
            if (String.IsNullOrEmpty(placeOfServiceId))
                return BadRequest();

            var placeOfService = _unitOfWork.PlaceOfServices.GetPlaceOfService(Guid.Parse(placeOfServiceId));

            if (placeOfService == null)
                return NotFound();

            return Ok(PlaceOfServiceDto.Wrap(placeOfService));
        }

        [HttpGet]
        public IHttpActionResult GetPlaceOfServices(Guid? corporationId = null)
        {
            try
            {
                var result = _unitOfWork.PlaceOfServices.GetAllActivePlaceOfServices(corporationId)
                                        .Select(PlaceOfServiceDto.Wrap)
                                        .ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
        }
    }
}
