namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateStoredProcedureGetContractAddendumByInsurance : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure("[dbo].[GetContractAddendumByInsurance]", parameter => new
            {
                InsuranceId = parameter.Guid()
            },
            @"
                BEGIN
	                SELECT     CA.* 
	                FROM       Insurances I (NOLOCK)
	                INNER JOIN Contracts C (NOLOCK)
			                   ON I.InsuranceId = C.InsuranceId
	                INNER JOIN ContractAddendums CA (NOLOCK)
			                   ON C.ContractId = CA.ContractId
	                WHERE      I.InsuranceId = @InsuranceId
	                AND        I.Active = 1
	                AND		   C.Active = 1
	                AND		   CA.Active = 1
                END
            ");
        }
        
        public override void Down()
        {
            Sql("DROP PROCEDURE [dbo].[GetContractAddendumByInsurance]");
        }
    }
}
