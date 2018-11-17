namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateStoredProcedureGetDoctorProviderByLocationAndLineofbusiness : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure("[dbo].[GetDoctorProviderByLocationAndLineofbusiness]", parameter => new
            {
                InsuranceId = parameter.Guid(),
                PlanTypeId = parameter.Guid()
            },
            @"
                BEGIN
                    SELECT     PBL.* 
                    FROM       Contracts C (NOLOCK)
                    INNER JOIN InsuranceBusinessLines IBL (NOLOCK)
		                       ON C.InsuranceId = IBL.InsuranceId
                    INNER JOIN ContractLineofBusinesses CLB (NOLOCK)
		                       ON IBL.PlanTypeId = CLB.PlanTypeId AND C.ContractId = CLB.ContractId
                    INNER JOIN [dbo].[DoctorCorporationContractLinks] DCCL (NOLOCK)
                               ON CLB.ContractLineofBusinessId = DCCL.ContractLineofBusinessId
                    INNER JOIN [dbo].[ProviderByLocations] PBL (NOLOCK)
                               ON DCCL.DoctorCorporationContractLinkId = PBL.DoctorCorporationContractLinkId
                    WHERE      C.Active = 1
                    AND        IBL.Active = 1
                    AND		   CLB.Active = 1
                    AND        IBL.InsuranceId = @InsuranceId
                    AND        IBL.PlanTypeId = @PlanTypeId
                    AND        DCCL.Active = 1
                    AND        PBL.Active = 1
                END
            ");
        }

        public override void Down()
        {
            Sql("DROP PROCEDURE [dbo].[GetDoctorProviderByLocationAndLineofbusiness]");
        }
    }
}
