namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class OverrrideConventionsForCorporationTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Corporations", "CorporationName", c => c.String(nullable: false, maxLength: 100));
        }

        public override void Down()
        {
            AlterColumn("dbo.Corporations", "CorporationName", c => c.String());
        }
    }
}
