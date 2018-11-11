using CanoHealth.WebPortal.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CanoHealth.WebPortal.ViewModels
{
    /*This class is used to map the values obtained when the stored procedure
     [dbo].GetDoctorLinkedContractsInfo is executed*/
    public class LinkedContractViewModel
    {
        public Guid DoctorCorporationContractLinkId { get; set; } //Contract at doctor level
        public Guid DoctorId { get; set; }
        public Guid ContractLineofBusinessId { get; set; }
        public string Note { get; set; }
        public DateTime EffectiveDate { get; set; }

        public string CorporationName { get; set; } //Corporation
        public string InsuranceName { get; set; } //Insurance
        public Guid ContractId { get; set; } //Contract at corporation level
        public string GroupNumber { get; set; } //Contract at corporation level

        public Guid PlanTypeId { get; set; } //Line of Business
        public string BusinessLine { get; set; } //Line of Business


        public IEnumerable<LinkedContractDto> ConvertToLinkedContractDto(List<LinkedContractViewModel> linkedContractViewModels)
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