using CanoHealth.WebPortal.Core.Dtos;
using CanoHealth.WebPortal.Core.Repositories;
using CanoHealth.WebPortal.ViewModels;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class LinkedContractRepository : Repository<LinkedContractViewModel>, ILinkedContractRepository
    {
        public LinkedContractRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<LinkedContractDto> GetDoctorLinkedContractInfo(Guid doctorId)
        {
            var query = "EXEC [dbo].[GetDoctorLinkedContractsInfo] @DoctorID";
            var linkedContractViewModels = GetWithRawSqlForTypesAreNotEntities(query,
                    new SqlParameter("@DoctorID", SqlDbType.UniqueIdentifier) { Value = doctorId })
                .ToList();

            var linkedContractDtos = ConvertToLinkedContractDto(linkedContractViewModels).ToList();

            return linkedContractDtos;
        }

        public IEnumerable<LinkedContractViewModel> GetLinkedContractByDoctor(Guid doctorId, string insuranceName)
        {
            var query = "EXEC [dbo].[GetDoctorLinkedContractsInfo] @DoctorID";
            var linkedContractViewModels = GetWithRawSqlForTypesAreNotEntities(query,
                    new SqlParameter("@DoctorID", SqlDbType.UniqueIdentifier) { Value = doctorId })
                .ToList();
            return linkedContractViewModels.Where(x => x.InsuranceName.Equals(insuranceName, StringComparison.CurrentCultureIgnoreCase));
        }

        private IEnumerable<LinkedContractDto> ConvertToLinkedContractDto(List<LinkedContractViewModel> linkedContractViewModels)
        {
            var result = new List<LinkedContractDto>();
            //The groupNumber is a unique field in Contract Entity
            var groupNumbers = linkedContractViewModels.Select(x => x.GroupNumber)
                .Distinct()
                .ToList();
            //For each groupNumber(Contract) get the Line of Business linked to the doctor
            // and the contract details.
            foreach (var groupNumber in groupNumbers)
            {
                var contractBusinessLineDto = linkedContractViewModels
                    .Where(g => g.GroupNumber == groupNumber)
                    .Select(y => new ContractBusinessLineDto
                    {
                        ContractLineofBusinessId = y.ContractLineofBusinessId,
                        PlanTypeId = y.PlanTypeId,
                        Name = y.BusinessLine
                    })
                    .ToList();

                var firstContract = linkedContractViewModels
                    .FirstOrDefault(g => g.GroupNumber == groupNumber);

                if (firstContract != null)
                {
                    var linkedContractDto = new LinkedContractDto
                    {
                        DoctorId = firstContract.DoctorId,
                        GroupNumber = groupNumber,
                        CorporationName = firstContract.CorporationName,
                        InsuranceName = firstContract.InsuranceName,
                        LineofBusiness = contractBusinessLineDto
                    };
                    result.Add(linkedContractDto);
                }
            }
            return result;
        }
    }
}