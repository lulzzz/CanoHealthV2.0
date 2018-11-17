namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateStoredProcedureGetContractLineofBusinessByContract : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure("[dbo].[GetContractLineofBusinessByContract]", parameter => new
            {
                ContractId = parameter.Guid()
            },
           @"
            BEGIN
                SELECT     CLB.* 
                FROM       [dbo].[Contracts] C (NOLOCK)
                INNER JOIN [dbo].[ContractLineofBusinesses] CLB (NOLOCK)
                           ON C.ContractId = CLB.ContractId
                WHERE      C.ContractId = @ContractId
                AND        C.Active = 1
                AND        CLB.Active = 1
            END
            "
            );
        }

        public override void Down()
        {
            Sql("DROP PROCEDURE [dbo].[GetContractLineofBusinessByContract]");
        }
    }
}
