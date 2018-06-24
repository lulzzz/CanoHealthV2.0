namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class OverrideContentTypeColumnConventionPosLicensesTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PosLicenses", "ContentType", c => c.String(maxLength: 255));
        }

        public override void Down()
        {
            AlterColumn("dbo.PosLicenses", "ContentType", c => c.String(maxLength: 50));
        }
    }
}
