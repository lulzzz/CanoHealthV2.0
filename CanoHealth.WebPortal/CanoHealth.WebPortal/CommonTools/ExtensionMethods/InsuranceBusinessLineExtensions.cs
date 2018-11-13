using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CanoHealth.WebPortal.CommonTools.ExtensionMethods
{
    public static class InsuranceBusinessLineExtensions
    {
        public static IEnumerable<InsuranceBusinessLine> ConvertToInsuranceBusinessLineEntity(
            this IEnumerable<InsuranceBusinessLineViewModel> viewModels)
        {
            return viewModels.Select(x => new InsuranceBusinessLine
            {
                InsuranceBusinessLineId = x.InsuranceBusinessLineId == Guid.Empty ? Guid.NewGuid() : x.InsuranceBusinessLineId,
                InsuranceId = x.InsuranceId,
                PlanTypeId = x.PlanTypeId,
                Active = x.Active == null ? true : x.Active
            }).ToList();
        }
    }
}