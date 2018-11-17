namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;
    //--get doctor provider by location and contract(DoctorProviderByLocation)
    public partial class CreateStoredProcedureGetDoctorProviderByLocationAndContract : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure("[dbo].[GetDoctorProviderByLocationAndContract]", parameter => new
            {
                ContractId = parameter.Guid()
            },
            @"
            BEGIN
                SELECT     PBL.* 
                FROM       [dbo].[Contracts] C (NOLOCK)
                INNER JOIN [dbo].[ContractLineofBusinesses] CLB (NOLOCK)
                           ON C.ContractId = CLB.ContractId
                INNER JOIN [dbo].[DoctorCorporationContractLinks] DCCL (NOLOCK)
		                   ON CLB.ContractLineofBusinessId = DCCL.ContractLineofBusinessId
                INNER JOIN [dbo].[ProviderByLocations] PBL (NOLOCK)
                           ON DCCL.DoctorCorporationContractLinkId = PBL.DoctorCorporationContractLinkId
                WHERE      C.ContractId = @ContractId
                AND        C.Active = 1
                AND        CLB.Active = 1
                AND        DCCL.Active = 1
                AND        PBL.Active = 1
            END
");
        }

        public override void Down()
        {
            Sql("DROP PROCEDURE [dbo].[GetDoctorProviderByLocationAndContract]");
        }
    }
}
