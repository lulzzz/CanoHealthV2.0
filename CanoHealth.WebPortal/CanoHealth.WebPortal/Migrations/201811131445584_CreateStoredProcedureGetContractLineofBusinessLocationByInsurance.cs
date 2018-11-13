namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateStoredProcedureGetContractLineofBusinessLocationByInsurance : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure("[dbo].[GetContractLineofBusinessLocationByInsurance]", parameter => new
            {
                InsuranceId = parameter.Guid()
            },
            @"
                BEGIN
                    SELECT     CLBC.* 
                    FROM       Insurances I (NOLOCK)
                    INNER JOIN Contracts C (NOLOCK)
		                       ON I.InsuranceId = C.InsuranceId
                    INNER JOIN ContractLineofBusinesses CLB (NOLOCK)
                               ON C.ContractId = CLB.ContractId
                    INNER JOIN ClinicLineofBusinessContracts CLBC (NOLOCK)
                               ON CLB.ContractLineofBusinessId = CLBC.ContractLineofBusinessId
                    WHERE      I.InsuranceId = @InsuranceId
                    AND        I.Active = 1
                    AND		   C.Active = 1
                    AND		   CLB.Active = 1
                    AND        CLBC.Active = 1
                    ORDER BY   CLBC.Id
                END
            ");
        }
        
        public override void Down()
        {
            Sql("DROP PROCEDURE [dbo].[GetContractLineofBusinessLocationByInsurance]");
        }
    }
}
