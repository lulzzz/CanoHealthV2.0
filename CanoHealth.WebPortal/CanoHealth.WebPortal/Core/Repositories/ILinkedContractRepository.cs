using CanoHealth.WebPortal.Core.Dtos;
using CanoHealth.WebPortal.ViewModels;
using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    //Class to call some stored procedures in DB
    public interface ILinkedContractRepository : IRepository<LinkedContractViewModel>
    {
        //Obtener la informacion de los contratos donde el doctor ya esta linkeado.
        IEnumerable<LinkedContractDto> GetDoctorLinkedContractInfo(Guid doctorId);

        IEnumerable<LinkedContractViewModel> GetLinkedContractByDoctor(Guid doctorId, string insuranceName);

    }
}
