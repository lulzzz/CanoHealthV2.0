namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreatePosLicenseAndLicenseTypeTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LicenseTypes",
                c => new
                {
                    LicenseTypeId = c.Guid(nullable: false),
                    LicenseName = c.String(nullable: false, maxLength: 100),
                })
                .PrimaryKey(t => t.LicenseTypeId);

            CreateTable(
                "dbo.PosLicenses",
                c => new
                {
                    PosLicenseId = c.Guid(nullable: false),
                    PlaceOfServiceId = c.Guid(nullable: false),
                    LicenseTypeId = c.Guid(nullable: false),
                    LicenseNumber = c.String(nullable: false, maxLength: 20),
                    EffectiveDate = c.DateTime(nullable: false),
                    ExpireDate = c.DateTime(nullable: false),
                    Note = c.String(maxLength: 255),
                    ServerLocation = c.String(maxLength: 100),
                    OriginalFileName = c.String(maxLength: 100),
                    UniqueFileName = c.String(maxLength: 100),
                    FileExtension = c.String(maxLength: 10),
                    FileSize = c.String(maxLength: 10),
                    ContentType = c.String(maxLength: 50),
                    UploadBy = c.String(maxLength: 128),
                    UploaDateTime = c.DateTime(),
                    Active = c.Boolean(),
                })
                .PrimaryKey(t => t.PosLicenseId)
                .ForeignKey("dbo.LicenseTypes", t => t.LicenseTypeId, cascadeDelete: false)
                .ForeignKey("dbo.PlaceOfServices", t => t.PlaceOfServiceId, cascadeDelete: false)
                .Index(t => t.PlaceOfServiceId)
                .Index(t => t.LicenseTypeId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.PosLicenses", "PlaceOfServiceId", "dbo.PlaceOfServices");
            DropForeignKey("dbo.PosLicenses", "LicenseTypeId", "dbo.LicenseTypes");
            DropIndex("dbo.PosLicenses", new[] { "LicenseTypeId" });
            DropIndex("dbo.PosLicenses", new[] { "PlaceOfServiceId" });
            DropTable("dbo.PosLicenses");
            DropTable("dbo.LicenseTypes");
        }
    }
}
