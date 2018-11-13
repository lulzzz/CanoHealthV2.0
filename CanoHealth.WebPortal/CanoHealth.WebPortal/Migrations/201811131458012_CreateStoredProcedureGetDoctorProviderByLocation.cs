namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateStoredProcedureGetDoctorProviderByLocation : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure("[dbo].[GetDoctorProviderByLocation]", parameter => new
            {
                InsuranceId = parameter.Guid()
            },
            @"
                BEGIN
                    SELECT     PBL.* 
                    FROM       Insurances I (NOLOCK)
                    INNER JOIN Contracts C (NOLOCK)
		                       ON I.InsuranceId = C.InsuranceId
                    INNER JOIN ContractLineofBusinesses CLB (NOLOCK)
                               ON C.ContractId = CLB.ContractId
                    INNER JOIN DoctorCorporationContractLinks DCCL (NOLOCK)
                               ON CLB.ContractLineofBusinessId = DCCL.ContractLineofBusinessId
                    INNER JOIN ProviderByLocations PBL (NOLOCK)
                               ON DCCL.DoctorCorporationContractLinkId = PBL.DoctorCorporationContractLinkId
                    WHERE      I.InsuranceId = @InsuranceId
                    AND        I.Active = 1
                    AND		   C.Active = 1
                    AND		   CLB.Active = 1
                    AND        DCCL.Active = 1
                    AND	       PBL.Active = 1
                    ORDER BY   PBL.ProviderByLocationId
                END
            ");
        }
        
        public override void Down()
        {
            Sql("DROP PROCEDURE [dbo].[GetDoctorProviderByLocation]");
        }
    }
}
