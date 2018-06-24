namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateDoctorIndividualProviderTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DoctorIndividualProviders",
                c => new
                {
                    DoctorIndividualProviderId = c.Guid(nullable: false),
                    DoctorId = c.Guid(nullable: false),
                    InsuranceId = c.Guid(nullable: false),
                    ProviderNumber = c.String(nullable: false),
                })
                .PrimaryKey(t => t.DoctorIndividualProviderId)
                .ForeignKey("dbo.Doctors", t => t.DoctorId)//, cascadeDelete: true
                .ForeignKey("dbo.Insurances", t => t.InsuranceId)//, cascadeDelete: true
                .Index(t => t.DoctorId)
                .Index(t => t.InsuranceId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.DoctorIndividualProviders", "InsuranceId", "dbo.Insurances");
            DropForeignKey("dbo.DoctorIndividualProviders", "DoctorId", "dbo.Doctors");
            DropIndex("dbo.DoctorIndividualProviders", new[] { "InsuranceId" });
            DropIndex("dbo.DoctorIndividualProviders", new[] { "DoctorId" });
            DropTable("dbo.DoctorIndividualProviders");
        }
    }
}
