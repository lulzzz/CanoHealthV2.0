namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateDoctorClinicRelationshipTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DoctorClinics",
                c => new
                {
                    DoctorClinicId = c.Guid(nullable: false),
                    DoctorId = c.Guid(nullable: false),
                    PlaceOfServiceId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.DoctorClinicId)
                .ForeignKey("dbo.PlaceOfServices", t => t.PlaceOfServiceId)//, cascadeDelete: true
                .ForeignKey("dbo.Doctors", t => t.DoctorId)//, cascadeDelete: true
                .Index(t => t.DoctorId)
                .Index(t => t.PlaceOfServiceId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.DoctorClinics", "DoctorId", "dbo.Doctors");
            DropForeignKey("dbo.DoctorClinics", "PlaceOfServiceId", "dbo.PlaceOfServices");
            DropIndex("dbo.DoctorClinics", new[] { "PlaceOfServiceId" });
            DropIndex("dbo.DoctorClinics", new[] { "DoctorId" });
            DropTable("dbo.DoctorClinics");
        }
    }
}
