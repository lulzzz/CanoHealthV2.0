namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateUniqueConstraintLicenseTypePlaceOfService : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[PosLicenses] ADD CONSTRAINT UK_PlaceOfServiceId_LicenseTypeId UNIQUE([PlaceOfServiceId], [LicenseTypeId])");
        }

        public override void Down()
        {
            Sql("ALTER TABLE [dbo].[PosLicenses] DROP CONSTRAINT UK_PlaceOfServiceId_LicenseTypeId");
        }
    }
}
