namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddActiveColumnToInsuranceLineofBusinessTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InsuranceBusinessLines", "Active", c => c.Boolean());
            Sql("ALTER TABLE [dbo].[InsuranceBusinessLines] ADD CONSTRAINT InsuranceBusinessLines_Active DEFAULT 1 FOR Active");
        }

        public override void Down()
        {
            DropColumn("dbo.InsuranceBusinessLines", "Active");
            Sql("ALTER TABLE [dbo].[InsuranceBusinessLines] DROP CONSTRAINT InsuranceBusinessLines_Active");
        }
    }
}
