namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateStoredProcedureContractLocationsByInsuranceAndLineofbusiness : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure("[dbo].[GetContractLocationsByInsuranceAndLineofbusiness]", parameter => new
            {
                InsuranceId = parameter.Guid(),
                PlanTypeId = parameter.Guid()
            },
           @"
            BEGIN
                SELECT     CLBC.* 
                FROM       Contracts C
                INNER JOIN InsuranceBusinessLines IBL
		                   ON C.InsuranceId = IBL.InsuranceId
                INNER JOIN ContractLineofBusinesses CLB
		                   ON IBL.PlanTypeId = CLB.PlanTypeId AND C.ContractId = CLB.ContractId
                INNER JOIN [dbo].[ClinicLineofBusinessContracts] CLBC
                           ON CLB.ContractLineofBusinessId = CLBC.ContractLineofBusinessId
                WHERE      C.Active = 1
                AND        IBL.Active = 1
                AND		   CLB.Active = 1
                AND        IBL.InsuranceId = @InsuranceId
                AND        IBL.PlanTypeId = @PlanTypeId
                AND        CLBC.Active = 1
            END
            ");
        }

        public override void Down()
        {
            Sql("DROP PROCEDURE [dbo].[GetContractLocationsByInsuranceAndLineofbusiness]");
        }
    }
}
