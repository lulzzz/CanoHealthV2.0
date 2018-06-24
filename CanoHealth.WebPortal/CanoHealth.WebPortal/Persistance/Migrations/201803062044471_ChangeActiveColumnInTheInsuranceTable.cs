namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class ChangeActiveColumnInTheInsuranceTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Insurances", "Active", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            AlterColumn("dbo.Insurances", "Active", c => c.Boolean());
        }
    }
}
