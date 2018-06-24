namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateUniqueConstraintNameInsuranceTable : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[Insurances] ADD CONSTRAINT UK_Insurance_Name UNIQUE([Name])");
        }

        public override void Down()
        {
            Sql("ALTER TABLE [dbo].[Insurances] DROP CONSTRAINT UK_Insurance_Name");
        }
    }
}
