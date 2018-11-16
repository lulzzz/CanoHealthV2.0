namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateStoredProcedureGetContractCorporationDoctorByInsuranceAndLineofBusiness : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure("[dbo].[GetContractCorporationDoctorByInsuranceAndLineofBusiness]", parameter => new
            {
                InsuranceId = parameter.Guid(),
                PlanTypeId = parameter.Guid()
            },
            @"
                BEGIN
                    SELECT     DCCL.* 
                    FROM       Contracts C
                    INNER JOIN InsuranceBusinessLines IBL
		                       ON C.InsuranceId = IBL.InsuranceId
                    INNER JOIN ContractLineofBusinesses CLB
		                       ON IBL.PlanTypeId = CLB.PlanTypeId AND C.ContractId = CLB.ContractId
                    INNER JOIN [dbo].[DoctorCorporationContractLinks] DCCL
                               ON CLB.ContractLineofBusinessId = DCCL.ContractLineofBusinessId
                    WHERE      C.Active = 1
                    AND        IBL.Active = 1
                    AND		   CLB.Active = 1
                    AND        IBL.InsuranceId = @InsuranceId
                    AND        IBL.PlanTypeId = @PlanTypeId
                    AND        DCCL.Active = 1
                END
            ");
        }

        public override void Down()
        {
            Sql("DROP PROCEDURE [dbo].[GetContractCorporationDoctorByInsuranceAndLineofBusiness]");
        }
    }
}
