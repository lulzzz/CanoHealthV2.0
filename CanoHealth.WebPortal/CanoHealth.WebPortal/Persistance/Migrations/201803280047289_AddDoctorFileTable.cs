namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddDoctorFileTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DoctorFiles",
                c => new
                {
                    DoctorFileId = c.Guid(nullable: false),
                    DoctorId = c.Guid(nullable: false),
                    DoctorFileTypeId = c.Guid(nullable: false),
                    OriginalFileName = c.String(maxLength: 100),
                    UniqueFileName = c.String(maxLength: 100),
                    FileExtension = c.String(maxLength: 10),
                    FileSize = c.String(maxLength: 10),
                    ContentType = c.String(maxLength: 255),
                    UploadDateTime = c.DateTime(nullable: false),
                    UploadBy = c.String(),
                    ServerLocation = c.String(maxLength: 100),
                    Active = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.DoctorFileId)
                .ForeignKey("dbo.Doctors", t => t.DoctorId) //, cascadeDelete: true
                .ForeignKey("dbo.DoctorFileTypes", t => t.DoctorFileTypeId) //, cascadeDelete: true
                .Index(t => t.DoctorId)
                .Index(t => t.DoctorFileTypeId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.DoctorFiles", "DoctorFileTypeId", "dbo.DoctorFileTypes");
            DropForeignKey("dbo.DoctorFiles", "DoctorId", "dbo.Doctors");
            DropIndex("dbo.DoctorFiles", new[] { "DoctorFileTypeId" });
            DropIndex("dbo.DoctorFiles", new[] { "DoctorId" });
            DropTable("dbo.DoctorFiles");
        }
    }
}
