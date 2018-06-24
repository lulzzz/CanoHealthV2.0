namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddNpiAndTaxIdColumnsToCorporationTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Corporations", "Npi", c => c.String(maxLength: 100));
            AddColumn("dbo.Corporations", "TaxId", c => c.String(maxLength: 100));
        }

        public override void Down()
        {
            DropColumn("dbo.Corporations", "TaxId");
            DropColumn("dbo.Corporations", "Npi");
        }
    }
}
