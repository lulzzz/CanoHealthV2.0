using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Linq.Expressions;

namespace CanoHealth.WebPortal.Core.Specifications.Insurances
{
    public class InsuranceContractSpecification : BaseSpecification<Insurance>
    {
        public InsuranceContractSpecification(Guid insuranceId) : base(filter: i => i.Active && i.InsuranceId == insuranceId)
        {
            AddRangeInclude(new Expression<Func<Insurance, object>>[] {
                i => i.Contracts
            });
        }
    }
}