using CanoHealth.WebPortal.Core.Domain;
using System;

namespace CanoHealth.WebPortal.ViewModels
{
    public class BusinessLineViewModel
    {
        public Guid PlanTypeId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public static BusinessLineViewModel Wrap(PlanType businessLine)
        {
            return new BusinessLineViewModel
            {
                PlanTypeId = businessLine.PlanTypeId,
                Name = businessLine.Name,
                Code = businessLine.Code
            };
        }
    }
}