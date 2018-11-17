namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    //get contract -> line of business -> locations(ClinicLineofBusinessContracts)
    public partial class CreateStoredProcedureGetContractLocationsByContract : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure("[dbo].[GetContractLocationsByContract]", parameters => new
            {
                ContractId = parameters.Guid()
            },
            @"
                BEGIN
                    SELECT     CLBC.* 
                    FROM       [dbo].[Contracts] C (NOLOCK)
                    INNER JOIN [dbo].[ContractLineofBusinesses] CLB (NOLOCK)
                               ON C.ContractId = CLB.ContractId
                    INNER JOIN ClinicLineofBusinessContracts CLBC (NOLOCK)
		                       ON CLB.ContractLineofBusinessId = CLBC.ContractLineofBusinessId
                    WHERE      C.ContractId = @ContractId
                    AND        C.Active = 1
                    AND        CLB.Active = 1
                    AND        CLBC.Active = 1
                END
            ");
        }

        public override void Down()
        {
            Sql("DROP PROCEDURE [dbo].[GetContractLocationsByContract]");
        }
    }
}
