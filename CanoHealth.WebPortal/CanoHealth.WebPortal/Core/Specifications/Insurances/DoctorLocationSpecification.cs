using CanoHealth.WebPortal.Core.Domain;
using System;

namespace CanoHealth.WebPortal.Core.Specifications.Insurances
{
    public class DoctorLocationSpecification : BaseSpecification<DoctorClinic>
    {
        public DoctorLocationSpecification(Guid doctorId) : base(filter: dc => dc.DoctorId == doctorId && dc.Active)
        {

        }
    }
}