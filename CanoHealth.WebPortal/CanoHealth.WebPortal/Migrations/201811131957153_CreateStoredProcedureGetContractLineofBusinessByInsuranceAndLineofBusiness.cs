namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateStoredProcedureGetContractLineofBusinessByInsuranceAndLineofBusiness : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure("[dbo].[GetContractLineofBusinessByInsuranceAndLineofBusiness]", parameter => new
            {
                InsuranceId = parameter.Guid(),
                PlanTypeId = parameter.Guid()
            },
            @"
                BEGIN
                    SELECT     CLB.* 
                    FROM       InsuranceBusinessLines IBL (NOLOCK)
                    INNER JOIN Contracts C (NOLOCK)
                               ON IBL.InsuranceId = C.InsuranceId
                    INNER JOIN ContractLineofBusinesses CLB (NOLOCK)
                               ON C.ContractId = CLB.ContractId AND CLB.PlanTypeId = IBL.PlanTypeId
                    WHERE      IBL.InsuranceId = @InsuranceId
                    AND        CLB.PlanTypeId = @PlanTypeId
                    AND        IBL.PlanTypeId = @PlanTypeId
                    AND        IBL.Active = 1
                    AND        C.Active = 1
                    AND        CLB.Active = 1
                END
            ");
        }
        
        public override void Down()
        {
            Sql("DROP PROCEDURE [dbo].[GetContractLineofBusinessByInsuranceAndLineofBusiness]");
        }
    }
}
