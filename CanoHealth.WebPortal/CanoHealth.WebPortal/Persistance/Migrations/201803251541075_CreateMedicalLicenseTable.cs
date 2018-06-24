namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateMedicalLicenseTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MedicalLicenses",
                c => new
                {
                    MedicalLicenseId = c.Guid(nullable: false),
                    DoctorId = c.Guid(nullable: false),
                    MedicalLicenseTypeId = c.Guid(nullable: false),
                    LicenseNumber = c.String(nullable: false, maxLength: 20),
                    EffectiveDate = c.DateTime(nullable: false),
                    ExpireDate = c.DateTime(nullable: false),
                    Note = c.String(maxLength: 255),
                    ServerLocation = c.String(maxLength: 100),
                    OriginalFileName = c.String(maxLength: 100),
                    UniqueFileName = c.String(maxLength: 100),
                    FileExtension = c.String(maxLength: 10),
                    FileSize = c.String(maxLength: 10),
                    ContentType = c.String(maxLength: 255),
                    UploadBy = c.String(maxLength: 128),
                    UploaDateTime = c.DateTime(),
                    Active = c.Boolean(),
                })
                .PrimaryKey(t => t.MedicalLicenseId)
                .ForeignKey("dbo.Doctors", t => t.DoctorId)//, cascadeDelete: true
                .ForeignKey("dbo.MedicalLicenseTypes", t => t.MedicalLicenseTypeId)//, cascadeDelete: true
                .Index(t => t.DoctorId)
                .Index(t => t.MedicalLicenseTypeId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.MedicalLicenses", "MedicalLicenseTypeId", "dbo.MedicalLicenseTypes");
            DropForeignKey("dbo.MedicalLicenses", "DoctorId", "dbo.Doctors");
            DropIndex("dbo.MedicalLicenses", new[] { "MedicalLicenseTypeId" });
            DropIndex("dbo.MedicalLicenses", new[] { "DoctorId" });
            DropTable("dbo.MedicalLicenses");
        }
    }
}
