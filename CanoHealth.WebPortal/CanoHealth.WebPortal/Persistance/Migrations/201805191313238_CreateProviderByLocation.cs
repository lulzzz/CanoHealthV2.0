namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateProviderByLocation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProviderByLocations",
                c => new
                {
                    ProviderByLocationId = c.Guid(nullable: false),
                    DoctorCorporationContractLinkId = c.Guid(nullable: false),
                    PlaceOfServiceId = c.Guid(nullable: false),
                    ProviderEffectiveDate = c.DateTime(nullable: false),
                    LocacionProviderNumber = c.String(),
                    Active = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ProviderByLocationId)
                .ForeignKey("dbo.DoctorCorporationContractLinks", t => t.DoctorCorporationContractLinkId)
                .Index(t => t.DoctorCorporationContractLinkId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.ProviderByLocations", "DoctorCorporationContractLinkId", "dbo.DoctorCorporationContractLinks");
            DropIndex("dbo.ProviderByLocations", new[] { "DoctorCorporationContractLinkId" });
            DropTable("dbo.ProviderByLocations");
        }
    }
}
