namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateDoctorTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Doctors",
                c => new
                {
                    DoctorId = c.Guid(nullable: false),
                    FirstName = c.String(nullable: false, maxLength: 50),
                    LastName = c.String(nullable: false, maxLength: 50),
                    DateOfBirth = c.DateTime(nullable: false),
                    Degree = c.String(nullable: false, maxLength: 255),
                    SocialSecurityNumber = c.String(nullable: false, maxLength: 11),
                    NpiNumber = c.String(nullable: false, maxLength: 20),
                    CaqhNumber = c.String(maxLength: 20),
                })
                .PrimaryKey(t => t.DoctorId);

        }

        public override void Down()
        {
            DropTable("dbo.Doctors");
        }
    }
}
