namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateStoredProcedureGetContractLineofBusinessByInsurance : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure("[dbo].[GetContractLineofBusinessByInsurance]", parameter => new
            {
                InsuranceId = parameter.Guid()
            },
            @"
                BEGIN
                    SELECT     CLB.* 
                    FROM       Insurances I (NOLOCK)
                    INNER JOIN Contracts C (NOLOCK)
		                       ON I.InsuranceId = C.InsuranceId
                    INNER JOIN ContractLineofBusinesses CLB (NOLOCK)
                               ON C.ContractId = CLB.ContractId
                    WHERE      I.InsuranceId = @InsuranceId
                    AND        I.Active = 1
                    AND		   C.Active = 1
                    AND		   CLB.Active = 1
                END
            ");
        }
        
        public override void Down()
        {
            Sql("DROP PROCEDURE [dbo].[GetContractLineofBusinessByInsurance]");
        }
    }
}
