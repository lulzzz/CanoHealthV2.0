namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class OverrideProviderEffectiveDateColumnForProviderByLocationTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProviderByLocations", "ProviderEffectiveDate", c => c.DateTime());
        }

        public override void Down()
        {
            AlterColumn("dbo.ProviderByLocations", "ProviderEffectiveDate", c => c.DateTime(nullable: false));
        }
    }
}
