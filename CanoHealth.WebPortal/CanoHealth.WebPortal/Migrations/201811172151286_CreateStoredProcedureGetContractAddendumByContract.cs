namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateStoredProcedureGetContractAddendumByContract : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure("[dbo].[GetContractAddendumByContract]", parameter => new
            {
                ContractId = parameter.Guid()
            },
            @"
                BEGIN
                    SELECT     CA.* 
                    FROM       [dbo].[Contracts] C (NOLOCK)
                    INNER JOIN [dbo].ContractAddendums CA
                               ON C.ContractId = CA.ContractId
                    WHERE      C.ContractId = @ContractId
                    AND        C.Active = 1
                    AND        CA.Active = 1
                END
            ");
        }

        public override void Down()
        {
            Sql("DROP PROCEDURE [dbo].[GetContractAddendumByContract]");
        }
    }
}
