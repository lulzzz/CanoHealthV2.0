namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateUniqueConstraintGroupNumberInContractTable : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[Contracts] ADD CONSTRAINT UK_GroupNumber UNIQUE([GroupNumber])");
        }

        public override void Down()
        {
            Sql("ALTER TABLE [dbo].[Contracts] DROP CONSTRAINT UK_GroupNumber");
        }
    }
}
