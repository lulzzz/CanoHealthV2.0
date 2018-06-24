using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.ViewModels;
using Elmah;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class IndividualProvidersController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndividualProvidersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult GetIndividualProviderByDoctorAndInsurance(
            Guid doctorId, Guid insuranceId)
        {
            var result = _unitOfWork.IndividualProviderRepository
                .ExistIndividualProvider(doctorId, insuranceId);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult SaveIndividualProvider(IndividualProviderFormViewModel individualProvider)
        {
            try
            {
                //check if the model is valid
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var doctorIndividualProvider = individualProvider.Convert();

                //check if there is an individual provider with the same provider number.
                var individualProviderWithSameNumber = _unitOfWork.IndividualProviderRepository
                    .GetIndividualProviderByProviderNumber(doctorIndividualProvider.DoctorIndividualProviderId, individualProvider.ProviderNumber);

                if (individualProviderWithSameNumber != null)
                {
                    ModelState.AddModelError("ProviderNumber", @"Duplicate Provider Number. Please try again.");
                    return BadRequest(ModelState);
                }

                //check if there is an individual provider for the same doctor and same insurance
                var duplicateIndividualProvider = _unitOfWork.IndividualProviderRepository
                    .GetIndividualProviderByDoctorAndInsurance(doctorIndividualProvider);

                if (duplicateIndividualProvider != null)
                {
                    ModelState.AddModelError("", @"Duplicate individual Provider. Please try again!");
                    return BadRequest(ModelState);
                }

                var auditLogs = new List<AuditLog>();

                //check if there is a current (ExpirationDate = null) out of network contract for that doctor and insurance
                var outOfNetworkContract = _unitOfWork.OutofNetworkContractRepository
                    .GetOutOfNetworkContractByDoctorAndInsurnace(individualProvider.DoctorId, individualProvider.InsuranceId);

                if (outOfNetworkContract != null)
                {
                    //se va a poner la fecha de efectividad del individual provider como la fecha de expiracion del out of network
                    var ooLogs = AuditLog.AddLog("OutOfNetworkContracts", "ExpirationDate",
                        outOfNetworkContract.ExpirationDate.ToString(),
                        individualProvider.IndividualProviderEffectiveDate.ToString(),
                        outOfNetworkContract.OutOfNetworkContractId, "Update");
                    outOfNetworkContract.ExpirationDate = individualProvider.IndividualProviderEffectiveDate;
                    auditLogs.Add(ooLogs);
                }

                var saveIndividualProviderLogs = _unitOfWork.IndividualProviderRepository
                   .SaveIndividualProviders(new List<DoctorIndividualProvider> { doctorIndividualProvider });
                auditLogs.AddRange(saveIndividualProviderLogs);

                _unitOfWork.AuditLogs.AddRange(auditLogs);

                individualProvider.InsuranceName = _unitOfWork.Insurances.GetInsuranceById(individualProvider.InsuranceId).Name;

                _unitOfWork.Complete();

                individualProvider.DoctorIndividualProviderId = doctorIndividualProvider.DoctorIndividualProviderId;
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
            return Ok(individualProvider);
        }
    }
}