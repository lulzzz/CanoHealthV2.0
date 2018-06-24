namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateMedicalLicenseTypeTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MedicalLicenseTypes",
                c => new
                {
                    MedicalLicenseTypeId = c.Guid(nullable: false),
                    Classification = c.String(),
                })
                .PrimaryKey(t => t.MedicalLicenseTypeId);

        }

        public override void Down()
        {
            DropTable("dbo.MedicalLicenseTypes");
        }
    }
}
