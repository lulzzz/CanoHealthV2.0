namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddColumnsAddressAndPhoneNumberToCorporationTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Corporations", "Address", c => c.String(nullable: false, maxLength: 255));
            AddColumn("dbo.Corporations", "PhoneNumber", c => c.String(maxLength: 20));
        }

        public override void Down()
        {
            DropColumn("dbo.Corporations", "PhoneNumber");
            DropColumn("dbo.Corporations", "Address");
        }
    }
}
