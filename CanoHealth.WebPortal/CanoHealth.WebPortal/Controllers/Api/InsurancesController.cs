﻿using AutoMapper;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Dtos;
using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class InsurancesController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public InsurancesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult GetInsurances()
        {
            var result = _unitOfWork.Insurances.GetActiveInsurances()
                .Select(Mapper.Map<Insurance, InsuranceDto>)
                .ToList();
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetAllInsurances()
        {
            var result = _unitOfWork
                .Insurances
                .EnumarableGetAll(orderBy: iqo => iqo.OrderBy(i => i.Name))
                .ToList();
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetInsurance(string insuranceName)
        {
            var insurance = _unitOfWork.Insurances.GetByName(insuranceName);
            if (insurance != null)
            {
                return Ok(InsuranceWithContractsDto.Wrap(insurance));
            }

            return Ok(new InsuranceWithContractsDto());
        }

        [HttpPut]
        public IHttpActionResult UpdateInsurance(InsuranceFormDto insurance)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var insuranceStoredInDb = _unitOfWork.Insurances.Get(insurance.InsuranceId);
                if (insuranceStoredInDb == null)
                {
                    return NotFound();
                }

                var logs = insuranceStoredInDb.ModifyInsurance(Mapper.Map(insurance, new Insurance()));

                _unitOfWork.AuditLogs.AddRange(logs);
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
            return Ok(insurance);
        }

        //Inactivate an Insurance means that Cano doesn't have business anymore with that insurance, not the insurance was out of the market
        [HttpDelete]
        public IHttpActionResult InactivateInsurance(InsuranceFormDto insurance)
        {
            try
            {
                if (!ModelState.IsValid)               
                    return BadRequest(ModelState);

                //Get the insurance and all the active contracts associated to it.
                var insuranceStoredInDb = _unitOfWork.Insurances.GetWithContracts(insurance.InsuranceId);
                if (insuranceStoredInDb == null)
                    return NotFound();

                var logs = new List<AuditLog>();

                //get active provider by locations associated to insurance
                var activeProvidersByLocation = _unitOfWork.ProviderByLocationRepository.ProviderByLocations(insurance.InsuranceId).ToList();
                var inactivateProviderLogs = activeProvidersByLocation.ConvertAll(x => x.Inactivate()).ToList();
                logs.AddRange(inactivateProviderLogs);

                //get active doctor corporation contract links to insurance
                var activedoctorLinkedContracts = _unitOfWork.DoctorLinkedContracts.DoctorCorporationContractLinks(insurance.InsuranceId).ToList();
                var inactivatedoccontractsLogs = activedoctorLinkedContracts.ConvertAll(x => x.InactiveDoctorCorporationContractLinkRecord()).ToList();
                logs.AddRange(inactivatedoccontractsLogs);

                //get active Contract->Line of Business->Location associtated to insurance
                var activeContractLineofBusinessLocations = _unitOfWork.ContracBusinessLineClinicRepository.GetContractLineofBusinessLocations(insurance.InsuranceId).ToList();
                var inactivateLogs = activeContractLineofBusinessLocations.ConvertAll(x => x.InactivateClinicLineofBusinessContract()).ToList();
                logs.AddRange(inactivateLogs);

                //get active contract's line of business associated to insurance
                var activecontracLineofbusiness = _unitOfWork.ContracBusinessLineRepository.ContractLineofBusinesses(insurance.InsuranceId).ToList();
                var inactivatelineofbusinessLogs = activecontracLineofbusiness.ConvertAll(x => x.InactivateContractLineofBusinessRecord()).ToList();
                logs.AddRange(inactivatelineofbusinessLogs);

                //get active contract's addendums associated to insurance
                var activecontractAddendums = _unitOfWork.Addendums.GetContractAddendumsByInsurance(insurance.InsuranceId).ToList();
                var inactivateaddendumsLogs = activecontractAddendums.ConvertAll(x => x.InactiveContractAddendum()).ToList();
                logs.AddRange(inactivateaddendumsLogs);

                //Inactivate insurance and all the active contracts associated to it.
                var inactivatecontractLogs = insuranceStoredInDb.InactivateInsurance().ToList();
                logs.AddRange(inactivatecontractLogs);

                //Inactivate all active Insurance Line of Business relationships
                var insuranceLineOfBusiness = _unitOfWork.InsuranceBusinessLineRepository.GetBusinessLines(insurance.InsuranceId).ToList();
                var insurancelineofbusinessLogs = insuranceLineOfBusiness.Select(item => item.InactivateInsuranceLineofBusinessRelation()).ToList();
                logs.AddRange(insurancelineofbusinessLogs);

                //Inactivate all Doctor Insurances relationships(DoctorIndividualProviders)
                var individualProviders = _unitOfWork.IndividualProviderRepository.
                    GetIndividualProvidersByInsurance(insurance.InsuranceId).ToList();
                var insurancedoctorLogs = individualProviders.ConvertAll(dip => dip.InactivateDoctorInsuranceRelationship());
                logs.AddRange(insurancedoctorLogs);

                _unitOfWork.AuditLogs.AddRange(logs);

                _unitOfWork.Complete();

                insurance.Active = false;
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
            return Ok(insurance);
        }

        //[HttpDelete]
        //public IHttpActionResult InactivateInsurance_old(InsuranceFormDto insurance)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        //Get the insurance and all the active contracts associated to it.
        //        var insuranceStoredInDb = _unitOfWork.Insurances.GetWithContracts(insurance.InsuranceId);
        //        if (insuranceStoredInDb == null)
        //        {
        //            return NotFound();
        //        }

        //        //Inactivate insurance and all the active contracts associated to it.
        //        var logs = insuranceStoredInDb.InactivateInsurance().ToList();

        //        //PENDING: for each inactivated contract inactivate active addendums

        //        //Inactivate all active Insurance Line of Business relationships
        //        var insuranceLineOfBusiness = _unitOfWork.InsuranceBusinessLineRepository.GetBusinessLines(insurance.InsuranceId).ToList();
        //        var insurancelineofbusinessLogs = insuranceLineOfBusiness.Select(item => item.InactivateInsuranceLineofBusinessRelation()).ToList();
        //        logs.AddRange(insurancelineofbusinessLogs);

        //        //Inactivate all Doctor Insurances relationships(DoctorIndividualProviders)
        //        var individualProviders = _unitOfWork.IndividualProviderRepository.
        //            GetIndividualProvidersByInsurance(insurance.InsuranceId).ToList();
        //        var insurancedoctorLogs = individualProviders.ConvertAll(dip => dip.InactivateDoctorInsuranceRelationship());
        //        logs.AddRange(insurancedoctorLogs);

        //        //Inactivate all active Corporation -> Insurance -> Line of business relationships (ContractLineofBusiness)
        //        var contractLineofBusiness = new List<ContractLineofBusiness>();
        //        foreach (var contract in insuranceStoredInDb.Contracts)
        //        {
        //            contractLineofBusiness = _unitOfWork.ContracBusinessLineRepository
        //                .GetContractBusinessLinesWithClinics(contract.ContractId.ToString())
        //                .ToList();

        //            foreach (var contractlineofbusiness in contractLineofBusiness)
        //            {
        //                contractlineofbusiness.InactivateRelationAmongContractLineofBusinessLocation();
        //                var doctorcorporationContractLinks = _unitOfWork.DoctorLinkedContracts.GetDoctorsLinkedToLineOfBusiness(contractlineofbusiness.ContractLineofBusinessId).ToList();
        //                doctorcorporationContractLinks.ForEach(x => x.InactivateRelationAmongContractLineofBusinessDoctor());
        //            }
        //        }

        //        _unitOfWork.AuditLogs.AddRange(logs);

        //        _unitOfWork.Complete();

        //        insurance.Active = false;

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorSignal.FromCurrentContext().Raise(ex);
        //        return InternalServerError(ex);
        //    }
        //    return Ok(insurance);
        //}
    }
}
