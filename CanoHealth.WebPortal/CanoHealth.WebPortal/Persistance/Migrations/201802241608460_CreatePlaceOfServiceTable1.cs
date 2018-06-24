namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreatePlaceOfServiceTable1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlaceOfServices",
                c => new
                {
                    PlaceOfServiceId = c.Guid(nullable: false),
                    CorporationId = c.Guid(nullable: false),
                    Name = c.String(nullable: false, maxLength: 100),
                    Address = c.String(maxLength: 100),
                    PhoneNumber = c.String(maxLength: 20),
                    FaxNumber = c.String(maxLength: 20),
                    Active = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.PlaceOfServiceId)
                .ForeignKey("dbo.Corporations", t => t.CorporationId, cascadeDelete: true)
                .Index(t => t.CorporationId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.PlaceOfServices", "CorporationId", "dbo.Corporations");
            DropIndex("dbo.PlaceOfServices", new[] { "CorporationId" });
            DropTable("dbo.PlaceOfServices");
        }
    }
}
