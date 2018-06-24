namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateUniqueConstraintCorporationIdInsuranceIdinContractTable : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[Contracts] ADD CONSTRAINT UK_CorporationId_InsuranceId UNIQUE([CorporationId], [InsuranceId])");
        }

        public override void Down()
        {
            Sql("ALTER TABLE [dbo].[Contracts] DROP CONSTRAINT UK_CorporationId_InsuranceId");
        }
    }
}
